using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class Game : Node2D
{
	[Export] public PackedScene[] Levels;
	// int currentLevel = 0;
	[Export] public PackedScene PlayerScene;
	[Export] public PackedScene PickupScene;
	public struct Message{
		public Player.Team SenderTeam;
		public Player.Team? ReceiverTeam;
		public string Text;
		public Message(Player.Team senderTeam, string message, Player.Team? receiverTeam = null){
			SenderTeam = senderTeam; ReceiverTeam = receiverTeam; Text = message;
		}
	}
	Queue<Message> messages;
	Queue<PackedScene> levelQueue;
	List<Player> players;
	List<PlayerSummary> summaries;
	Camera2D camera;
	Node2D levelHolder;
	Timer timer;
	Label results;
	AudioStreamPlayer music;
	[Signal]
	public delegate void GameOverEventHandler(PlayerSummary[] summaries);
	public override void _Ready()
	{
		camera = GetNode<Camera2D>("Camera2D");
		levelHolder = GetNode<Node2D>("LevelHolder");
		timer = GetNode<Timer>("Timer");
		results = GetNode<Label>("Results");
		music = GetNode<AudioStreamPlayer>("Music");
		music.Play();
		summaries = new();
		players = new();
		messages = new();
		levelQueue = new();
		// AdvanceLevel();
		// Only runs if scene is run independently
		// SetTournament()
		if(GetParent() is Window){
			PlayerSummary[] temp = {
				new PlayerSummary("kb", Player.Team.Blue),
				new PlayerSummary("0", Player.Team.Green),
			};
			SetTournament(temp, 1);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Handle messages
		while(messages.Count > 0){
			var message = messages.Dequeue();
			var sender = summaries.Find(p => p.PlayerTeam == message.SenderTeam);
			var receiver = summaries.Find(p => p.PlayerTeam == message.ReceiverTeam);
			if(message.Text == "kill"){
				sender.Kills++;
			}
			else if(message.Text == "die"){
				sender.Deaths++;
			}
			// GD.Print(sender.ToString());
		}
		// Remove dead players
		players = players.Where(p => IsInstanceValid(p)).ToList();
		// GD.Print(players.Count);
		if(players.Count == 1 && timer.IsStopped()){
			players[0].Win();
			var summary = summaries.Find(s => s.PlayerTeam == players[0].PlayerTeam);
			summary.MatchWins++;
			timer.Start();
			music.Playing = false;
		}
	}

	public void SetTournament(PlayerSummary[] summaries, int numLevels){
		this.summaries = new(summaries);
		levelQueue.Clear();
		var scenes = new List<PackedScene>();
		for (int idx = 0; idx < numLevels; idx++)
		{
			if(scenes.Count == 0){
				foreach (var level in Levels)
				{
					scenes.Add(level);
				}
			}
			int index = Mathf.FloorToInt(GD.Randf() * scenes.Count);
			levelQueue.Enqueue(scenes[index]);
			scenes.RemoveAt(index);
		}
		AdvanceLevel();
	}
	void AdvanceLevel(){
		if(levelQueue.Count == 0) return;
		SetLevel(levelQueue.Dequeue());
		music.Playing = true;
	}

	void ClearLevel(){
		foreach (var child in levelHolder.GetChildren())
		{
			levelHolder.RemoveChild(child);
			child.QueueFree();
		}
		players.Clear();
	}

	void SetLevel(PackedScene levelScene){
		ClearLevel();
		var level = levelScene.Instantiate();
		levelHolder.AddChild(level);
		var spawners = GetTree().GetNodesInGroup("Spawner");
		if(spawners.Count < summaries.Count()){
			GD.PrintErr("Not enough spawners in level!");
			return;
		}
		// Get a random spawner for each player
		var openSpawners = new List<int>();
		foreach (var spawner in spawners)
		{
			openSpawners.Add(openSpawners.Count);
		}
		foreach (var summary in summaries)
		{
			var idx = Mathf.FloorToInt(GD.Randf() * openSpawners.Count);
			var spawner = spawners[openSpawners[idx]] as Node2D;
			openSpawners.RemoveAt(idx);
			var player = PlayerScene.Instantiate<Player>();
			player.Position = spawner.Position;
			player.InputDevice = summary.InputDevice;
			player.PlayerTeam = summary.PlayerTeam;
			players.Add(player);
			level.AddChild(player);
		}
	}
	public void AddMessage(Player sender, string message){
		messages.Enqueue(new Message(sender.PlayerTeam, message, null));
	}
	public void AddMessage(Player sender, string message, Player receiver){
		messages.Enqueue(new Message(sender.PlayerTeam, message, receiver.PlayerTeam));
	}
	private void _on_timer_timeout()
	{
		if(levelQueue.Count > 0) AdvanceLevel();
		else{
			// Show Results Screen
			ClearLevel();
			EmitSignal(SignalName.GameOver, summaries.ToArray());
		}
	}
	private void _on_music_finished()
	{
		music.Play();
	}
	private void _on_pickup_timer_timeout()
	{
		// Find an unoccupied spawner
		var nodes = GetTree().GetNodesInGroup("Spawner").Where(n => n.GetChildCount() == 0).ToArray();
		if(nodes.Count() == 0) return;
		var idx = Mathf.FloorToInt(GD.Randf() * nodes.Count());
		var pickup = PickupScene.Instantiate<Node2D>();
		nodes[idx].AddChild(pickup);
		pickup.Position = Vector2.Zero;
	}
}

