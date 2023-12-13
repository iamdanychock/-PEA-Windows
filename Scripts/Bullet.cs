using Godot;
using System;

// Author : Dany
namespace Com.IsartDigital.WindowWar.Gameplay
{
	
	public partial class Bullet : Area2D
	{
        [Export] protected float _ShotSpeed = 1000f;
        protected Vector2 _Velocity;

        public override void _Ready()
        {
            Connect("area_entered",new Callable(this,nameof(Entered)));
        }

        public void Shot(Vector2 pDirection)
        {
            _Velocity = pDirection.Normalized() * _ShotSpeed;

        }

        public override void _Process(double pDelta)
        {
            GlobalPosition += _Velocity * (float)pDelta;
        }

        protected void Entered(Area2D pEntered)
        {
            if (pEntered is Detector)
            {
                Destroy();
            }
        }

        public void Destroy()
        {
            QueueFree();
        }
    }
}
