using Godot;
using System;

public partial class Trap : Node2D
{
	[Export] public PackedScene BulletScene;
	[Export] public float BulletSpeed = 600.0f;
	RayCast2D ray;
	bool canFire = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ray = GetNode<RayCast2D>("RayCast2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if(!canFire) return;
		var collider = ray.GetCollider();
		// GD.Print(collider);
		if(collider is CharacterBody2D){
			// Shoot bullet
			var bullet = BulletScene.Instantiate<Bullet>();
			GetParent().AddChild(bullet);
			bullet.Position = Position;
			bullet.Rotation = Rotation;
			bullet.Velocity = Transform.BasisXform(Vector2.Up * BulletSpeed);
			// bullet.Velocity = Vector2.Up * BulletSpeed;
			canFire = false;
			// GD.Print("fire!");
		}
	}
}
