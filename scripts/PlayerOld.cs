using Godot;
using System;

// enum PlayerState{
// 	Grounded,
// 	Falling,
// 	Ballistic,
// 	Dashing,
// }
public partial class PlayerOld : CharacterBody2D
{
	[Export] public string InputDevice = "kb";
	[ExportGroup("Movement")]
	[Export] public float Speed = 300.0f;
	[Export] public float JumpVelocity = -200.0f;
	[Export] public float BonusJumpVelocity = -400.0f;
	[Export] public float GravityCoefficient = 0.65f;
	[Export] public float CoyoteTime = 0.1f;
	[Export] public float JumpBufferTime = 0.2f;
	[Export] public float WallJumpSpeed = 300.0f;
	[Export] public float WallJumpLockout = 0.5f;
	[ExportGroup("Pickups")]
	[Export] public int BonusJumps = 0;
	[Export] public int Dashes = 0;
	[Export] public int Items = 0;
	// PlayerState state = PlayerState.Falling;
	// misc clocks
	float coyoteClock = 0.0f;
	float jumpBufferClock = 0.0f;
	float wallJumpBufferClock = 0.0f;
	float moveLockoutClock = 0.0f;
	Vector2 lastWallNormal = Vector2.Zero;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		// Decrement clocks
		if(coyoteClock > 0.0f) coyoteClock -= dt;
		if(jumpBufferClock > 0.0f) jumpBufferClock -= dt;
		if(wallJumpBufferClock > 0.0f) wallJumpBufferClock -= dt;
		if(moveLockoutClock > 0.0f) moveLockoutClock -= dt;

		if(IsOnFloor()){
			coyoteClock = CoyoteTime;
			BonusJumps = 1;
		}
		else{
			var left = TestMove(Transform, Vector2.Left * 4);
			var right = TestMove(Transform, Vector2.Right * 4);
			if(left) lastWallNormal = Vector2.Right;
			if(right) lastWallNormal = Vector2.Left;
			if(left || right) wallJumpBufferClock = JumpBufferTime;
		}

		// Setup movement vars
		bool isGrounded = IsOnFloor() || coyoteClock > 0.0f;
		// bool canJump = isGrounded || coyoteClock > 0.0f;
		bool canBonusJump = BonusJumps > 0;
		bool canWallJump = IsOnWallOnly() || wallJumpBufferClock > 0.0f;
		bool jumpJustPressed = Input.IsActionJustPressed(InputDevice + "_jump");
		bool jumpPressed = Input.IsActionPressed(InputDevice + "_jump");
		bool shouldJump = jumpJustPressed || jumpBufferClock > 0.0f;
		float hMove = Input.GetAxis(InputDevice + "_left", InputDevice + "_right");

		// Handle movement
		
		Vector2 velocity = Velocity;

		// Add the gravity.
		if(!isGrounded){
			var gravityPower = gravity;
			if(jumpPressed) gravityPower *= GravityCoefficient;
			velocity.Y += gravityPower * (float)delta;
		}

		// Handle regular jump.
		if(isGrounded && shouldJump){
			velocity.Y = JumpVelocity;
			jumpBufferClock = 0.0f;
		}
		// Wall jump
		else if(canWallJump && shouldJump){
			var jumpDir = lastWallNormal;
			jumpDir.Y = -1.0f;
			jumpDir = jumpDir.Normalized();
			velocity = jumpDir * WallJumpSpeed;
			moveLockoutClock = WallJumpLockout;
		}
		// Bonus jump
		else if(jumpJustPressed && canBonusJump){
			velocity.Y = BonusJumpVelocity;
			BonusJumps--;
		}

		// Handle horizontal movement
		if(moveLockoutClock <= 0.0f){
			float hSpeed = Speed * hMove;
			velocity.X = hSpeed;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	private void _on_head_area_entered(Area2D area)
	{
		// Replace with function body.
	}
}
