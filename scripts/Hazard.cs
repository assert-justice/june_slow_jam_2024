using Godot;
using System;

public partial class Hazard : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<AnimatedSprite2D>("Sprite").Play();
	}
	private void _on_body_entered(Node2D body)
	{
		if(body is Player player){
			player.Die();
		}
	}
}

