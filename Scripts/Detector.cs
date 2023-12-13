using Godot;
using System;

// Author : Dany
namespace Com.IsartDigital.WindowWar.Gameplay {
	
	public partial class Detector : Area2D
	{
		public CollisionShape2D Collision => GetChild<CollisionShape2D>(0);
		public RectangleShape2D Shape = new RectangleShape2D();

		public override void _Ready()
		{
			Collision.Shape = Shape;
		}

		public override void _Process(double pDelta)
		{

		}
	}
}
