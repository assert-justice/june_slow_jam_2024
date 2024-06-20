using Godot;
using System;

public partial class Corpse : Node2D
{
	public void Init(Player player){
		player.GetParent().AddChild(this);
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		var color = Player.GetTeamName(player.PlayerTeam);
		var isWinged = player.Dashes > 0;
		var wingStatus = isWinged ? "winged" : "wingless";
		var anim = $"{color}_{wingStatus}";
		Position = player.Position;
		sprite.Animation = anim;
		sprite.Play();
		GetNode<AudioStreamPlayer2D>("Sound").Play();
	}
	private void _on_sprite_animation_finished()
	{
		QueueFree();
	}
}


