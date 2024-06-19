using Godot;
using System;

public partial class Pickup : RigidBody2D
{
	public override void _Ready()
	{
		GetNode<AnimatedSprite2D>("Sprite").Play();
	}
	private void _on_pickup_area_body_entered(Node2D body)
	{
		if(body is Player player){
			if(player.AddDash()) {
				QueueFree();
			}
		}
	}
}
