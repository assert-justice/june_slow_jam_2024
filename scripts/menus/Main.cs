using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Main : Control
{
	[Export]
	PackedScene GameScene;
	CanvasLayer gameHolder;
	Control menuHolder;
	Control menuBackground;
	Lobby lobby;
	Scoreboard scoreboard;
	AudioStreamPlayer musicPlayer;
	Stack<string> menuStack;
	public override void _Ready()
	{
		gameHolder = GetNode<CanvasLayer>("GameHolder");
		gameHolder.Layer = -1;
		menuHolder = GetNode<Control>("MenuHolder");
		menuBackground = GetNode<Control>("MenuBackground");
		menuStack = new Stack<string>();
		lobby = GetNode<Lobby>("MenuHolder/Lobby");
		lobby.StartGame += _on_lobby_start_game;
		lobby.ExitLobby += _on_lobby_exit_lobby;
		scoreboard = GetNode<Scoreboard>("MenuHolder/Scoreboard");
		scoreboard.ExitScoreboard += _on_scoreboard_exit;
		musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
		
		SetMenu("Title");
	}
	public override void _PhysicsProcess(double delta)
	{
		bool isInGame = gameHolder.GetChildCount() > 0;
		if(!isInGame && !musicPlayer.Playing) musicPlayer.Play();
		menuBackground.Visible = !isInGame;
	}
	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("pause") && !IsPaused()){
			SetPaused(true);
			GetViewport().SetInputAsHandled();
		}
		else if (@event.IsActionPressed("ui_cancel") && IsPaused()){
			if(menuStack.Peek() == "Pause") SetPaused(false);
			else if(menuStack.Count > 1) {
				PopMenu();
			}
			GetViewport().SetInputAsHandled();
		}
		else if(@event.IsActionPressed("ui_accept") && menuStack.Count() > 0 && menuStack.Peek() == "Title"){
			PushMenu("Main");
			GetViewport().SetInputAsHandled();
		}
	}
	bool IsPaused(){
		return gameHolder.ProcessMode == ProcessModeEnum.Disabled;
	}
	void SetPaused(bool paused){
		if(paused == IsPaused()) return;
		if(paused){
			SetMenu("Pause");
			gameHolder.ProcessMode = ProcessModeEnum.Disabled;
		}
		else{
			foreach(var child in menuHolder.GetChildren()){
				if(child is CanvasItem node){node.Visible = false;}
			}
			gameHolder.ProcessMode = ProcessModeEnum.Always;
		}
	}
	void SelectMenu(){
		bool found = false;
		var name = menuStack.Peek();
		menuHolder.Visible = true;
		// gameHolder.ProcessMode = ProcessModeEnum.Disabled;
		foreach(var child in menuHolder.GetChildren()){
			if(child is CanvasItem node){
				node.Visible = false;
				if(node.Name == name){
					SetFocus(node);
					found = true;
					node.Visible = true;
				}
			}
		}
		if(!found) GD.PrintErr("Invalid menu name: " + name);
	}
	bool SetFocus(Node node){
		if(node is Control ctrl && ctrl.FocusMode == FocusModeEnum.All){
			ctrl.GrabFocus();
			return true;
		}
		foreach(var child in node.GetChildren()){
			if(SetFocus(child)) return true;
		}
		return false;
	}
	void PushMenu(string name){
		menuStack.Push(name);
		SelectMenu();
	}
	void PopMenu(){
		if(menuStack.Count <= 1){
			GD.PrintErr("Attempt to pop from near empty menu stack");
			return;
		}
		menuStack.Pop();
		SelectMenu();
	}
	void SetMenu(string name){
		menuStack.Clear();
		menuStack.Push(name);
		SelectMenu();
	}
	void Launch(PlayerSummary[] playerSummaries){
		foreach (var child in gameHolder.GetChildren())
		{
			child.QueueFree();
		}
		var game = GameScene.Instantiate() as Game;
		gameHolder.AddChild(game);
		game.SetTournament(playerSummaries, 3);
		game.GameOver += _on_game_game_over;
		menuHolder.Visible = false;
		menuStack.Clear();
		// gameHolder.ProcessMode = ProcessModeEnum.Always;
		musicPlayer.Stop();
		SetPaused(false);
	}
	void SetBusVolume(string busName, double value){
		var busId = AudioServer.GetBusIndex(busName);
		var db = Math.Log10(value / 100) * 10;
		AudioServer.SetBusVolumeDb(busId, (float)db);
	}
	private void _on_play_button_down()
	{
		PushMenu("Lobby");
		lobby.SetActive(true);
	}
	private void _on_options_button_down()
	{
		PushMenu("Options");
	}
	private void _on_exit_button_down()
	{
		GetTree().Quit();
	}
	private void _on_back_button_down()
	{
		PopMenu();
	}
	private void _on_resume_button_down()
	{
		SetPaused(false);
	}
	private void _on_main_menu_button_down()
	{
		SetPaused(true);
		SetMenu("Main");
		gameHolder.GetChild(0).QueueFree();
	}
	private void _on_fullscreen_toggle_toggled(bool toggled_on)
	{
		// var mode = DisplayServer.WindowGetMode();
		var mode = toggled_on ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed;
		DisplayServer.WindowSetMode(mode);
	}
	private void _on_main_volume_slider_value_changed(double value)
	{
		SetBusVolume("Master", value);
	}
	private void _on_music_volume_slider_value_changed(double value)
	{
		SetBusVolume("Music", value);
	}
	private void _on_sfx_volume_slider_value_changed(double value)
	{
		SetBusVolume("Sfx", value);
	}
	private void _on_voice_volume_slider_value_changed(double value)
	{
		SetBusVolume("Voice", value);
	}
	private void _on_lobby_start_game(Registration[] registrations)
	{
		var summaries = registrations.Select(r => r.GetPlayerSummary()).Where(s => s != null).ToArray();
		// move match wins to cross tournament wins
		summaries = summaries.Select(s => new PlayerSummary(s.InputDevice, s.PlayerTeam, s.MatchWins, s.CrossTournamentMatchWins)).ToArray();
		Launch(summaries);
	}
	private void _on_lobby_exit_lobby()
	{
		lobby.SetActive(false);
		PopMenu();
	}

	private void _on_scoreboard_exit() 
	{
		SetMenu("Title");
		PushMenu("Main");
		PushMenu("Lobby");

		lobby.SetActive(true);
		scoreboard.SetActive(false);
	}

	private void _on_game_game_over(PlayerSummary[] playerSummaries)
	{
		SetMenu("Scoreboard");
		scoreboard.SetActive(true);
		scoreboard.SetSummaries(playerSummaries);
	}
}
