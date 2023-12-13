using Godot;
using System;

// Author : Dany
namespace Com.IsartDigital.WindowWar.Gameplay {
	
	public partial class ExternWindow : Window
	{

		private Main2 _Main = Main2.GetInstance();


        private Camera2D _Camera => GetNode<Camera2D>("Camera2D");


        public override void _Ready()
		{
            Title = "ExternWindow";
            Transient = true;
            World2D = _Main.MainWindow.World2D;
			Size = new Vector2I(500, 500);
            _Camera.AnchorMode = Camera2D.AnchorModeEnum.FixedTopLeft;
            Position = new Vector2I(500,500);
		}

		public override void _Process(double pDelta)
		{
            _Camera.Position = Position;
        }

    }
}
