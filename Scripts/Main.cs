using Godot;
using Com.IsartDigital.WindowWar.Utils;
using System;

//Author : Dany
namespace Com.IsartDigital.WindowWar.Gameplay
{

    public partial class Main : Node2D
    {

        #region Singleton
        static private Main instance;

        static public Main GetInstance()
        {
            if (instance == null) instance = new Main();
            return instance;
        }

        private Main() : base() { }
        #endregion

        #region Exports

        [Export] private NodePath _MainCameraPath;
        [Export] private NodePath _ShotContainerPath;
        [Export] private PackedScene _PackedPlayer;
        [Export] private PackedScene _PackedExternWindow;


        [Export] private Vector2I _InitialSize;


        #endregion

        #region Variable Declaration

        public Camera2D MainCamera => GetNode<Camera2D>(_MainCameraPath);
        public Window MainWindow => GetWindow();

        public Node ShotContainer => GetNode<Node>(_ShotContainerPath);

        public Vector2I ScreenSize => DisplayServer.ScreenGetSize();

        private Detector _DetectorUp => GetNode<Detector>(Utils.Constants.DETECTOR_UP_PATH);
        private Detector _DetectorDown => GetNode<Detector>(Utils.Constants.DETECTOR_DOWN_PATH);
        private Detector _DetectorLeft => GetNode<Detector>(Utils.Constants.DETECTOR_LEFT_PATH);
        private Detector _DetectorRight => GetNode<Detector>(Utils.Constants.DETECTOR_RIGHT_PATH);


        private float _ExpandLeft = 0;
        private float _ExpandRight = 0;
        private float _ExpandUp = 0;
        private float _ExpandDown = 0;

        private Rect2 _WindowRect;

        private Vector2 _WindowOrigin;

        private Player _Player;

        #endregion


        public override void _Ready()
        {
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(Main) + " Instance already exist, destroying the last added.");
                return;
            }
            instance = this;
            #endregion

            ProjectSettings.SetSetting("display/window/size/viewport_width", ScreenSize.X);
            ProjectSettings.SetSetting("display/window/size/viewport_height", ScreenSize.Y);

            //MainWindow.Size = _InitialSize;

            WindowSetup();
            CameraSetup();
            DetectorSetup();
            PlayerSetup();

            #region Debug
            Panel pPanel = new Panel();
            AddChild(pPanel);
            pPanel.Size = _InitialSize;
            pPanel.Position = MainWindow.Position - MainWindow.Size/2;
            #endregion


        }

        public override void _Process(double pDelta)
        {
            MainWindow.Position = (Vector2I)_WindowRect.Position;
            MainWindow.Size = (Vector2I)_WindowRect.Size;

            WindowUpdate();

            DetectorsSettings();

            if(Input.IsActionJustPressed("ui_accept"))
            {
                Window lWind = new Window();
                AddChild(lWind);
            }

            #region Debug
            //GD.Print("DetectorUp : " + _DetectorUp.Position);
            GD.Print("_WinRectPos : " + _WindowRect.Position);
            //GD.Print("RectSize: " + _WindowRect.Size);
            GD.Print("WindowPos: " + MainWindow.Position);
            GD.Print("SreenSize : " + ScreenSize);
            GD.Print("PlayerPos : " + _Player.GlobalPosition);
            #endregion
        }






        #region Setup

        private void CameraSetup()
        {
            MainCamera.AnchorMode = Camera2D.AnchorModeEnum.FixedTopLeft;
        }

        private void WindowSetup()
        {
   
            //MainWindow.Position = ScreenSize / 2 - _InitialSize/2;
            _WindowRect = new Rect2(MainWindow.Position, MainWindow.Size);
            MainWindow.Unresizable = true;


        }

        private void PlayerSetup()
        {
            _Player = _PackedPlayer.Instantiate<Player>();
            AddChild(_Player);

            _Player.Position = MainWindow.Position + _WindowRect.Size / 2;
        }

        #endregion

        #region Process
        private void DetectorsSettings()
        {

            //_DetectorLeft.Position = _WindowRect.Position;
            //_DetectorLeft.Shape.Size = new Vector2(60, _WindowRect.Size.Y);
            //_DetectorLeft.Collision.Position = new Vector2(-60 / 2, _DetectorLeft.Collision.Position.Y);

            //_DetectorRight.Position = _WindowRect.Position + new Vector2(_WindowRect.Size.X, _WindowRect.Size.Y / 2);
            //_DetectorRight.Shape.Size = new Vector2(60, _WindowRect.Size.Y);
            //_DetectorRight.Collision.Position = new Vector2(60 / 2, _DetectorRight.Collision.Position.Y);

            _DetectorUp.GlobalPosition = _WindowRect.Position + new Vector2(_WindowRect.Size.X, 0); 
            _DetectorUp.Shape.Size = new Vector2(_WindowRect.Size.X, 1);

            //_DetectorDown.Position = _WindowRect.Position + new Vector2(_WindowRect.Size.X / 2, _WindowRect.Size.Y);
            //_DetectorDown.Shape.Size = new Vector2(_WindowRect.Size.X, 60);
            //_DetectorDown.Collision.Position = new Vector2(_DetectorRight.Collision.Position.X, 60 / 2);

        }

        private void WindowUpdate()
        {

            _ExpandLeft = (float)Mathf.Lerp(_ExpandLeft, 0.0, 10.0 * GetProcessDeltaTime());
            _ExpandRight = (float)Mathf.Lerp(_ExpandRight, 0.0, 10.0 * GetProcessDeltaTime());
            _ExpandUp = (float)Mathf.Lerp(_ExpandUp, 0.0, 10.0 * GetProcessDeltaTime());
            _ExpandDown = (float)Mathf.Lerp(_ExpandDown, 0.0, 10.0 * GetProcessDeltaTime());

            //_WindowRect = _WindowRect.GrowIndividual(_ExpandLeft, _ExpandUp, _ExpandRight, _ExpandDown);

        }


        private void DetectorSetup()
        {
            _DetectorUp.Connect("area_entered", new Callable(this, nameof(UpHit)));
            _DetectorDown.Connect("area_entered", new Callable(this, nameof(DownHit)));
            _DetectorLeft.Connect("area_entered", new Callable(this, nameof(LeftHit)));
            _DetectorRight.Connect("area_entered", new Callable(this, nameof(RightHit)));
        }
        #endregion

        #region Detectors
        private void UpHit(Area2D pEntered)
        {
            if (pEntered is PlayerBullet)
            {
                _ExpandUp = 10;
            }
        }
        private void DownHit(Area2D pEntered)
        {
            if (pEntered is PlayerBullet)
            {
                _ExpandDown = 10;
            }
        }
        private void LeftHit(Area2D pEntered)
        {
            if (pEntered is PlayerBullet)
            {
                _ExpandLeft = 10;
            }
        }
        private void RightHit(Area2D pEntered)
        {
            if (pEntered is PlayerBullet)
            {
                _ExpandRight = 10;
            }
        }

        #endregion

        #region Windows Creator




        #endregion


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
    }
}

