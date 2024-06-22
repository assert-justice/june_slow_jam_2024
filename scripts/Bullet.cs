using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export] public Vector2 Velocity;
	// Called when the node enters the scene tree for the first time.

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// Translate(Velocity * (float)delta);
		GlobalPosition += Velocity * (float)delta;
	}
	private void _on_body_entered(Node2D body)
	{
		if(body is Player player && !body.IsQueuedForDeletion()){
			player.Die();
		}
		QueueFree();
	}
}
