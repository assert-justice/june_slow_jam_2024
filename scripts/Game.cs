using Godot;
using System;

public partial class Game : Node2D
{
	// [Export] public PackedScene[] Levels;
	Camera2D camera;
	public override void _Ready()
	{
		camera = GetNode<Camera2D>("Camera2D");
	}

	public override void _Process(double delta)
	{
	}
}
