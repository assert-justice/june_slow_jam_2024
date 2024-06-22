using Godot;
using System;

public partial class Hazard : Area2D
{
	[Export] public float JiggleTime = 0.1f;
	[Export] public float JiggleDistance = 2.0f;
	Vector2 origin;
	float jiggleClock = 0.0f;
	AnimatedSprite2D sprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		origin = Position;
	}
	public override void _PhysicsProcess(double delta)
	{
		if(jiggleClock > 0){
			jiggleClock -= (float)delta;
		}
		else{
			var offset = Vector2.Zero;
			offset.X = GD.Randf() * 2 - 0.5f;
			offset.Y = GD.Randf() * 2 - 0.5f;
			Position = origin + offset * JiggleDistance;
			jiggleClock = JiggleTime;
		}
	}
	private void _on_body_entered(Node2D body)
	{
		if(body is Player player){
			player.Die();
		}
	}
}

