using Godot;
using System;

public partial class Launcher : Area2D
{
	private void _on_body_entered(Node2D body)
	{
		if(body is Player player){
			player.SetVelocity(Vector2.Up * 600.0f, 0.3f, true);
		}
	}
}
