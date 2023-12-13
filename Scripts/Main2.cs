using Godot;
using System;

//Author : Dany
namespace Com.IsartDigital.WindowWar.Gameplay
{

    public partial class Main2 : Node2D
    {

        #region Singleton
        static private Main2 instance;

        static public Main2 GetInstance()
        {
            if (instance == null) instance = new Main2();
            return instance;
        }

        private Main2() : base() { }
        #endregion


        #region DEBUG

        [ExportGroup("DEBUG")]
        [Export] private bool _WindowPosition = false;
        [Export] private bool _WinRectSizePrint = false;
        [Export] private bool _WinRectPosPrint = false;
        [Export] private bool _PlayerPositionPrint = false;



        #endregion


        #region STATE MACHINE

        protected Action doAction;
        protected float processDelta;


        #endregion

        [ExportCategory("Exports")]
        [Export] private PackedScene _PackedPlayer;
        [Export] private PackedScene _PackedWin;

        Player _Player;
        public Camera2D MainCamera => GetNode<Camera2D>("MainCamera");
        public Window MainWindow => GetWindow();

        public Node ShotContainer => GetNode<Node>("ShotContainer");

        private bool _CanResize = true;


        private Vector2 _ResizingVector;

        #region Detectors

        private Detector _DetectorUp => GetNode<Detector>(Utils.Constants.DETECTOR_UP_PATH);
        private Detector _DetectorDown => GetNode<Detector>(Utils.Constants.DETECTOR_DOWN_PATH);
        private Detector _DetectorLeft => GetNode<Detector>(Utils.Constants.DETECTOR_LEFT_PATH);
        private Detector _DetectorRight => GetNode<Detector>(Utils.Constants.DETECTOR_RIGHT_PATH);

        #endregion



        private float _ExpandLeft = 0;
        private float _ExpandRight = 0;
        private float _ExpandUp = 0;
        private float _ExpandDown = 0;


        private float _ExpandBackLeft = 0;
        private float _ExpandBackRight = 0;
        private float _ExpandBackUp = 0;
        private float _ExpandBackDown = 0;


        private Vector2 _WindowVelocity = Vector2.Zero;


        private Rect2 _WinRect;

        public override void _Ready()
        {
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(Main2) + " Instance already exist, destroying the last added.");
                return;
            }
            instance = this;
            #endregion
            DetectorSetup();

            _Player = _PackedPlayer.Instantiate<Player>();
            AddChild(_Player);


            _WinRect = new Rect2I(MainWindow.Position, MainWindow.Size);
            MainCamera.AnchorMode = Camera2D.AnchorModeEnum.FixedTopLeft;

            _Player.GlobalPosition = (_WinRect.Position) + _WinRect.Size / 2;

            ExternWindow lWin = _PackedWin.Instantiate<ExternWindow>();
            AddChild(lWin);




        }

        public override void _Process(double pDelta)
        {
            base._Process(pDelta);
            if (doAction != null) doAction();
            processDelta = (float)pDelta;


            MainWindow.Position = (Vector2I)_WinRect.Position;
            MainWindow.Size = (Vector2I)_WinRect.Size;
            MainCamera.Position = MainWindow.Position;
            
            WindowUpdate();
            DetectorsSettings();



        }

        #region STATE MACHINE

        private void SetModeVoid()
        {
            doAction = DoActionVoid;
        }

        private void DoActionVoid()
        {
            _WinRect.Size = _WinRect.Size.Lerp(new Vector2(500, 500), 0.5f);

        }

        private void SetModeResize()
        {
            doAction = DoActionResize;
        }

        private void DoActionResize()
        {
            // Effectuer l'interpolation linéaire
            _WinRect.Position = _WinRect.Position.Lerp(_WinRect.Position + _ResizingVector, 0.5f);

            // Vérifier si la position actuelle est proche de la position cible
            if (_WinRect.Position != _WinRect.Position + _ResizingVector)
            {
                // Réinitialiser _ResizingVector
                _ResizingVector = Vector2.Zero;

                // Effectuer d'autres actions nécessaires après le lerp
                SetModeVoid();

                // Désactiver le redimensionnement
            }


        }


        #endregion


        private void DetectorsSettings()
        {

            _DetectorLeft.Position = (Vector2I)_WinRect.Position + new Vector2(0,_WinRect.Size.Y/2);
            _DetectorLeft.Shape.Size = new Vector2(1, _WinRect.Size.Y);

            _DetectorRight.Position = _WinRect.Position + new Vector2(_WinRect.Size.X, _WinRect.Size.Y / 2);
            _DetectorRight.Shape.Size = new Vector2(1, _WinRect.Size.Y);

            _DetectorUp.GlobalPosition = _WinRect.Position + new Vector2(_WinRect.Size.X / 2, 0);
            _DetectorUp.Shape.Size = new Vector2(_WinRect.Size.X, 1);

            _DetectorDown.Position = _WinRect.Position + new Vector2(_WinRect.Size.X / 2, _WinRect.Size.Y);
            _DetectorDown.Shape.Size = new Vector2(_WinRect.Size.X, 1);

        }

        private void WindowUpdate()
        {
                


            if (Mathf.Abs(_ExpandLeft) < 0.01f)
            {
                _ExpandLeft = 0.0f;
            }
            else
                _ExpandLeft = Mathf.Lerp(_ExpandLeft, 0, 10 * processDelta);

            if (Mathf.Abs(_ExpandRight) < 0.01f)
            {
                _ExpandRight = 0;

            }

            else
                _ExpandRight = Mathf.Lerp(_ExpandRight, 0, 10 * processDelta);
 

            if (Mathf.Abs(_ExpandUp) < 0.01f)
            {
                _ExpandUp = 0.0f;

            }
            else
                _ExpandUp = Mathf.Lerp(_ExpandUp, 0, 10 * processDelta);
            

            if (Mathf.Abs(_ExpandDown) < 0.01f)
            {
                _ExpandDown = 0.0f;
            }

            else
                _ExpandDown = Mathf.Lerp(_ExpandDown, 0, 10 * processDelta);


            _WinRect = _WinRect.GrowIndividual(_ExpandLeft, _ExpandUp, _ExpandRight, _ExpandDown);


        }


        private void DetectorSetup()
        {
            _DetectorUp.Connect("area_entered", new Callable(this, nameof(UpHit)));
            _DetectorDown.Connect("area_entered", new Callable(this, nameof(DownHit)));
            _DetectorLeft.Connect("area_entered", new Callable(this, nameof(LeftHit)));
            _DetectorRight.Connect("area_entered", new Callable(this, nameof(RightHit)));
        }

        #region Detectors
        private void UpHit(Area2D pEntered)
        {
            if (pEntered is PlayerBullet)
            {
                _ExpandUp = 10;
                _ExpandBackUp = -10;
                _ResizingVector.Y -= 30;
                _CanResize = false;
                _WindowVelocity += Vector2.Up * _ExpandUp;
                SetModeResize();

            }
        }
        private void DownHit(Area2D pEntered)
        {
            if (pEntered is PlayerBullet)
            {
                _ExpandDown = 10;
                _ExpandBackDown = -10;
                _ResizingVector.Y += 30;

                _CanResize = false;

                _WindowVelocity = Vector2.Down * _ExpandDown;

                SetModeResize();
            }
        }
        private void LeftHit(Area2D pEntered)
        {
            if (pEntered is PlayerBullet)
            {
                _ExpandLeft = 10;
                _ExpandBackLeft = -10;
                _ResizingVector.X -= 30;
                _CanResize = false;

                _WindowVelocity += Vector2.Left * _ExpandLeft;
                SetModeResize();

            }
        }
        private void RightHit(Area2D pEntered)
        {
            if (pEntered is PlayerBullet)
            {
                _ExpandRight = 10;
                _ExpandBackRight = -10;
                _ResizingVector.X += 30;
                _CanResize = false;

                _WindowVelocity += Vector2.Right * _ExpandRight;
                SetModeResize();

            }
        }

        #endregion

        public override void _Draw()
        {

            DrawRect(_WinRect, Colors.AliceBlue, true);
            base._Draw();
        }

        public override void _PhysicsProcess(double delta)
        {
            MainCamera.Position = MainWindow.Position;
            base._PhysicsProcess(delta);
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }



        private void DebugPrinter()
        {
            if (_WindowPosition)
                GD.Print("Window Position : " + MainWindow.Position);
            if (_PlayerPositionPrint)
                GD.Print("Player Position : " + _Player.Position);
        }
    }
}
