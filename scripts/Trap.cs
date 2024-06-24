using Godot;
using System;

public partial class Trap : Node2D
{
	[Export] public PackedScene BulletScene;
	[Export] public float BulletSpeed = 200.0f;
	[Export] public float Cooldown = 3;
	RayCast2D ray;
	AnimatedSprite2D sprite;
	float clock = 0.0f;
	int lastFrame = 0;
	// bool canFire = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ray = GetNode<RayCast2D>("RayCast2D");
		sprite = GetNode<AnimatedSprite2D>("Sprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// if(!canFire) return;
		if(sprite.Frame == 10 && lastFrame != 10) Fire();
		lastFrame = sprite.Frame;
		if(clock > 0) {
			clock -= (float)delta;
			return;
		}
		var collider = ray.GetCollider();
		// GD.Print(collider);
		if(collider is CharacterBody2D){
			// Shoot bullet
			sprite.Play();
			// bullet.Velocity = Vector2.Up * BulletSpeed;
			// canFire = false;
			clock = Cooldown;
			// GD.Print("fire!");
		}
	}
	void Fire(){
		var bullet = BulletScene.Instantiate<Bullet>();
		GetParent().AddChild(bullet);
		bullet.Position = Position;
		bullet.Rotation = Rotation;
		bullet.Velocity = Transform.BasisXform(Vector2.Up * BulletSpeed);
	}
}
