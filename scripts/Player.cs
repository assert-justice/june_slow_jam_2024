using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public enum Team{
		Blue,
		Red,
		Yellow,
		Green,
	}
	[Export] public string InputDevice = "kb";
	[Export] public Team PlayerTeam = Team.Blue;
	[ExportGroup("Movement")]
	[Export] public float Speed = 300.0f;
	[Export] public float JumpVelocity = -200.0f;
	[Export] public float BonusJumpVelocity = -300.0f;
	[Export] public float GravityCoefficient = 0.65f;
	[Export] public float CoyoteTime = 0.1f;
	[Export] public float JumpBufferTime = 0.2f;
	[Export] public float WallJumpSpeed = 300.0f;
	[Export] public float WallJumpDuration = 0.5f;
	[Export] public float DashSpeed = 800.0f;
	[Export] public float DashDuration = 0.2f;
	[Export] public float BounceSpeed = 500.0f;
	[Export] public float BounceTime = 0.3f;
	[Export] public float WallSlideSpeed = 100.0f;
	[ExportGroup("Pickups")]
	[Export] public int BonusJumps = 0;
	[Export] public int Dashes = 0;
	[Export] public int Items = 0;
	// Misc clocks
	float coyoteClock = 0.0f;
	float jumpBufferClock = 0.0f;
	float wallJumpBufferClock = 0.0f;
	float wallJumpClock = 0.0f;
	float dashClock = 0.0f;
	Vector2 lastWallNormal = Vector2.Zero;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	// Children
	CollisionShape2D dashCollider;
	CollisionShape2D hitBox;
	AnimatedSprite2D sprite;
	// Node references
	Game game;
	string animState = "default";
	// Builtin methods
	public override void _Ready()
	{
		dashCollider = GetNode<CollisionShape2D>("DashBox/DashCollider");
		hitBox = GetNode<CollisionShape2D>("HitBox");
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		sprite.Play();
		// SetAnimation("default");
		game = GetTree().GetNodesInGroup("Game")[0] as Game;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		// Decrement clocks
		if(coyoteClock > 0.0f) coyoteClock -= dt;
		if(jumpBufferClock > 0.0f) jumpBufferClock -= dt;
		if(wallJumpBufferClock > 0.0f) wallJumpBufferClock -= dt;
		if(wallJumpClock > 0.0f) wallJumpClock -= dt;
		if(dashClock > 0.0f) dashClock -= dt;
		if (dashCollider.Disabled == dashClock > 0.0f){
			dashCollider.Disabled = !(dashClock > 0.0f);
		}

		bool onLeftWall = false;
		bool onRightWall = false;
		if(IsOnFloor()){
			coyoteClock = CoyoteTime;
			BonusJumps = 1;
			// Dashes = 1;
		}
		else{
			onLeftWall = TestMove(Transform, Vector2.Left * 4);
			onRightWall = TestMove(Transform, Vector2.Right * 4);
			if(onLeftWall) lastWallNormal = Vector2.Right;
			if(onRightWall) lastWallNormal = Vector2.Left;
			if(onLeftWall || onRightWall) wallJumpBufferClock = JumpBufferTime;
		}

		// Setup movement vars
		bool isGrounded = IsOnFloor() || coyoteClock > 0.0f;
		// bool canJump = isGrounded || coyoteClock > 0.0f;
		bool canBonusJump = BonusJumps > 0;
		bool canWallJump = IsOnWallOnly() || wallJumpBufferClock > 0.0f;
		bool jumpJustPressed = Input.IsActionJustPressed(InputDevice + "_jump");
		bool jumpPressed = Input.IsActionPressed(InputDevice + "_jump");
		bool dashJustPressed = Input.IsActionJustPressed(InputDevice + "_dash");
		bool shouldJump = jumpJustPressed || jumpBufferClock > 0.0f;
		Vector2 move = Input.GetVector(InputDevice + "_left", InputDevice + "_right", InputDevice + "_up", InputDevice + "_down");
		float hMove = move.X;
		if(onLeftWall) sprite.FlipH = false;
		else if(onRightWall) sprite.FlipH = true;
		else if(hMove < 0.0f){
			sprite.FlipH = true;
		}
		else if(hMove > 0.0f){
			sprite.FlipH = false;
		}

		// Handle movement
		
		Vector2 velocity = Velocity;

		// Add the gravity.
		if(!isGrounded && dashClock <= 0.0f){
			var gravityPower = gravity;
			if(jumpPressed) gravityPower *= GravityCoefficient;
			velocity.Y += gravityPower * (float)delta;
			// Cap vertical speed if on wall
			if(canWallJump && velocity.Y > WallSlideSpeed) velocity.Y = WallSlideSpeed;
		}

		// Handle dash
		if(dashJustPressed && Dashes > 0 && move.Length() > 0.0f){
			velocity = move.Normalized() * DashSpeed;
			dashClock = DashDuration;
			Dashes--;
		}
		// Handle regular jump.
		else if(isGrounded && shouldJump){
			velocity.Y = Dashes > 0 ? BonusJumpVelocity : JumpVelocity;
			jumpBufferClock = 0.0f;
			// Can cancel out of a dash by jumping
			dashClock = 0.0f;
		}
		// Wall jump
		else if(canWallJump && shouldJump){
			var jumpDir = lastWallNormal;
			jumpDir.Y = -1.0f;
			jumpDir = jumpDir.Normalized();
			velocity = jumpDir * WallJumpSpeed;
			wallJumpClock = WallJumpDuration;
			dashClock = 0.0f;
		}
		// Bonus jump
		else if(jumpJustPressed && canBonusJump){
			// velocity.Y = BonusJumpVelocity;
			velocity.Y = Dashes > 0 ? BonusJumpVelocity : JumpVelocity;
			BonusJumps--;
		}

		// Handle horizontal movement
		if(wallJumpClock <= 0.0f && dashClock <= 0.0f){
			float hSpeed = Speed * hMove;
			velocity.X = hSpeed;
		}

		// Update animation
		if(velocity.Y < 0.0f){
			//rising jump
			animState = "jumping";
			if(Velocity.Y >= 0.0f){
				sprite.Play();
			}
			// sprite.iso
		}
		else if(isGrounded){
			if(hMove != 0.0f) animState = "running";
			else animState = "default";
		}
		else if(velocity.Y > 0.0f){
			//falling
			animState = "falling";
		}
		else{
			animState = "default";
		}
		if(animState!="jumping" && !sprite.IsPlaying()) sprite.Play();
		var anim = GenAnimation(animState);
		if(sprite.Animation != anim) sprite.Animation = anim;

		Velocity = velocity;
		MoveAndSlide();
	}

	// Utility methods
	string GenAnimation(string name){
		var winged = Dashes > 0 ? "winged" : "wingless";
		return $"{Enum.GetName(PlayerTeam.GetType(), PlayerTeam).ToLower()}_{winged}_{name}";
	}
	// Public methods
	public void SetHLockoutTime(float time){
		if(time > wallJumpClock) wallJumpClock = time;
	}
	public void SetVelocity(Vector2 velocity, float lockoutTime = 0){
		SetHLockoutTime(lockoutTime);
		Velocity = velocity;
	}
	public void Bounce(Vector2 position){
		var dir = (Position - position).Normalized();
		SetVelocity(dir * BounceSpeed, BounceTime);
		// Velocity = dir * BounceSpeed;
		// wallJumpClock = BounceTime;
	}
	public bool Die(){
		// Returns false if the player cannot currently die i.e. dash immunity.
		if(dashClock > 0.0f) return false;
		game.AddMessage(this, "die");
		GetParent().RemoveChild(this);
		QueueFree();
		return true;
	}
	public bool AddDash(){
		// If already at max dashes, return false to indicate the powerup should not be consumed?
		Dashes++;
		// SetAnimation("winged");
		return true;
	}
	// Signal methods
	private void _on_dash_box_body_entered(Node2D body)
	{
		if(body is Player player){
			if(player == this) return;
			player.Die();
			game.AddMessage(this, "kill", player);
			Bounce(player.Position);
		}
	}
	private void _on_feet_area_entered(Area2D area)
	{
		if(area.IsInGroup("Head")){
			if(area.GetParent() is Player player){
				player.Die();
				game.AddMessage(this, "kill", player);
				Bounce(player.Position);
			}
		}
	}
}

