using Godot;
using System;

public partial class Registration : Control
{
    PlayerSummary playerSummary;
    Panel panel;
    Label label;

    const string JOIN_TEXT = "Press A to join...";

    StyleBoxTexture player1StyleBox;
    StyleBoxTexture player2StyleBox;
    StyleBoxTexture player3StyleBox;
    StyleBoxTexture player4StyleBox;

    public override void _Ready()
    {
        playerSummary = null;
        panel = GetNode<Panel>("Panel");
        label = GetNode<Label>("Label");

        player1StyleBox = ResourceLoader.Load("res://player-1-active-panel-texture.tres") as StyleBoxTexture;
        player2StyleBox = ResourceLoader.Load("res://player-2-active-panel-texture.tres") as StyleBoxTexture;
        player3StyleBox = ResourceLoader.Load("res://player-3-active-panel-texture.tres") as StyleBoxTexture;
        player4StyleBox = ResourceLoader.Load("res://player-4-active-panel-texture.tres") as StyleBoxTexture;
    }

    public void Register(PlayerSummary playerSummary, int slot)
    {
        this.playerSummary = playerSummary;
        
        string labelText = $"Player {slot + 1}";// 0-indexed
        if (playerSummary.MatchWins > 0)
        {
            labelText += $"\nWins: {playerSummary.MatchWins}";
        }
        label.Text = labelText;

        switch(slot)
        {
            case 0:
                panel.AddThemeStyleboxOverride("panel", player1StyleBox);
                break;
            case 1:
                panel.AddThemeStyleboxOverride("panel", player2StyleBox);
                break;
            case 2:
                panel.AddThemeStyleboxOverride("panel", player3StyleBox);
                break;
            case 3:
                panel.AddThemeStyleboxOverride("panel", player4StyleBox);
                break;
        }
    }

    public bool IsRegistered()
    {
        return playerSummary != null;
    }

    public PlayerSummary GetPlayerSummary() {
        return playerSummary;
    }

    public void Unregister()
    {
        this.playerSummary = null;
        label.Text = JOIN_TEXT;
        panel.RemoveThemeStyleboxOverride("panel");
    }
}
