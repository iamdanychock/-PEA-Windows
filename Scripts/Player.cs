using Godot;
using System;

//Author : Dany
namespace Com.IsartDigital.WindowWar.Gameplay
{

    public partial class Player : Area2D
    {

        #region Singleton
        static private Player instance;

        static public Player GetInstance()
        {
            if (instance == null) instance = new Player();
            return instance;
        }

        private Player() : base() { }
        #endregion

        #region Exports

        [Export] private PackedScene _PackedBullet;
        [Export] private int _Speed;
        [Export] private float _FireRateTime;


        

        #endregion


        #region Variable Declaration

        private Main2 _Main => Main2.GetInstance();
        private Node _ShotContainer => _Main.ShotContainer;

        private Vector2 _Velocity;

        private bool _CanMove = true;

        private float _TimeSinceLastShot = 0f;
        private Vector2 _PreviousPosition;

        #endregion

        public override void _Ready()
        {
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(Player) + " Instance already exist, destroying the last added.");
                return;
            }
            instance = this;
            #endregion

            Connect("area_entered", new Callable(this,nameof(Collider)));
            
        }

        public override void _Process(double pDelta)
        {
            _TimeSinceLastShot += (float)pDelta;
            if (_CanMove)
            {
                Move();
            }
            if (Input.IsActionPressed("fire") && _TimeSinceLastShot >= _FireRateTime)
            {
                CreateBullet();
            }
        }


        private void CreateBullet()
        {
            _TimeSinceLastShot = 0;
            PlayerBullet lShot = _PackedBullet.Instantiate<PlayerBullet>();
            lShot.GlobalPosition = GlobalPosition;
            Vector2 target = GetGlobalMousePosition() - GlobalPosition;
            lShot.Shot(target);
            _ShotContainer.AddChild(lShot);
        }


        private void Move()
        {
            _Velocity = Vector2.Zero;
            Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            _Velocity = inputDir * _Speed;

            _PreviousPosition = GlobalPosition;

            GlobalPosition += _Velocity;

        }


        private void Collider(Area2D pEntered)
        {
            if (pEntered is Detector)
            {
                GlobalPosition = _PreviousPosition;
                GD.Print("in");
            }

        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}
