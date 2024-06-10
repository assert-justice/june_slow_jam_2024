using Godot;


public partial class Player : CharacterBody2D{
	enum PlayerState{
		Grounded,
		Falling,
		Ballistic,
		Dashing,
	}
	
}
