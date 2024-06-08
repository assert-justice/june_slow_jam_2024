using Godot;
using System;
using System.Collections.Generic;

public partial class main : Control
{
	[Export]
	PackedScene GameScene;
	CanvasLayer gameHolder;
	Control menuHolder;
	Stack<string> menuStack;
	public override void _Ready()
	{
		gameHolder = GetNode<CanvasLayer>("GameHolder");
		gameHolder.Layer = -1;
		menuHolder = GetNode<Control>("MenuHolder");
		menuStack = new Stack<string>();
		SetMenu("Main");
	}
	public override void _Process(double delta){
		if(Input.IsActionJustPressed("ui_cancel")){
			if(!IsPaused()) SetPaused(true);
			else if(menuStack.Peek() == "Pause") SetPaused(false);
			else if(menuStack.Count > 1) PopMenu();
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
				if(child.Name == name){
					SetFocus(node);
					found = true;
					node.Visible = true;
				}
			}
		}
		if(!found) GD.PrintErr("Invalid menu name");
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
	void Launch(){
		foreach (var child in gameHolder.GetChildren())
		{
			child.QueueFree();
		}
		var game = GameScene.Instantiate();
		gameHolder.AddChild(game);
		menuHolder.Visible = false;
		// gameHolder.ProcessMode = ProcessModeEnum.Always;
		SetPaused(false);
	}
	void SetBusVolume(string busName, double value){
		var busId = AudioServer.GetBusIndex(busName);
		var db = Math.Log10(value / 100) * 10;
		AudioServer.SetBusVolumeDb(busId, (float)db);
	}
	private void _on_play_button_down()
	{
		Launch();
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
}
