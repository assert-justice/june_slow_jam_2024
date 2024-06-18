using Godot;
using System;

public partial class Launcher : Area2D
{
	AnimatedSprite2D sprite;
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite");
	}
	private void _on_body_entered(Node2D body)
	{
		if(body is Player player){
			player.SetVelocity(Vector2.Up * 600.0f, player.Velocity.X);
			sprite.Play();
		}
	}
}
