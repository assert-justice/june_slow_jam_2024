using Godot;
using System;

public partial class Launcher : Area2D
{
	[Export] public float Power = 600.0f;
	[Export] public float LockoutTime = 0.2f;
	AnimatedSprite2D sprite;
	AudioStreamPlayer2D sound;
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		sound = GetNode<AudioStreamPlayer2D>("Sound");
	}
	private void _on_body_entered(Node2D body)
	{
		if(body is Player player){
			// convert player v into local coordinates
			var vel = player.Velocity;
			vel.Y = 0;
			vel = Transform.BasisXformInv(vel);
			vel.Y = -Power;
			vel = Transform.BasisXform(vel);
			player.SetVelocity(vel, LockoutTime);
			sprite.Play();
			sound.Play();
		}
	}
}
