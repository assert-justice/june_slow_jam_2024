using Godot;
using System;

public partial class Registration : Control
{
    PlayerSummary playerSummary;
    PanelContainer labelContainer;
    Label label;
    TextureRect portraitRect;

    [Export]
    public Texture2D Portrait;

    const string JOIN_TEXT = "Press A to join...";

    StyleBoxTexture player1StyleBox;
    Texture2D player1Portrait;
    StyleBoxTexture player2StyleBox;
    Texture2D player2Portrait;
    StyleBoxTexture player3StyleBox;
    Texture2D player3Portrait;
    StyleBoxTexture player4StyleBox;
    Texture2D player4Portrait;

    public override void _Ready()
    {
        playerSummary = null;
        labelContainer = GetNode<PanelContainer>("Label Container");
        label = labelContainer.GetNode<Label>("Label");
        portraitRect = GetNode<Panel>("Portrait Panel").GetNode<TextureRect>("Portrait");
        portraitRect.Texture = Portrait;

        player1StyleBox = ResourceLoader.Load("res://player-1-active-panel-texture.tres") as StyleBoxTexture;
        player1Portrait = ResourceLoader.Load("res://sprites/Cyan Mothman Portrait.png") as Texture2D;
        player2StyleBox = ResourceLoader.Load("res://player-2-active-panel-texture.tres") as StyleBoxTexture;
        player2Portrait = ResourceLoader.Load("res://sprites/Magenta Mothman Portrait.png") as Texture2D;
        player3StyleBox = ResourceLoader.Load("res://player-3-active-panel-texture.tres") as StyleBoxTexture;
        player3Portrait = ResourceLoader.Load("res://sprites/Yellow Mothman Portrait.png") as Texture2D;
        player4StyleBox = ResourceLoader.Load("res://player-4-active-panel-texture.tres") as StyleBoxTexture;
        player4Portrait = ResourceLoader.Load("res://sprites/Green Mothman Portrait.png") as Texture2D;
    }

    public void Register(PlayerSummary playerSummary, int slot)
    {
        this.playerSummary = playerSummary;

        labelContainer.Visible = false;

        switch(slot)
        {
            case 0:
                // labelContainer.AddThemeStyleboxOverride("panel", player1StyleBox);
                portraitRect.Modulate = new Color(1,1,1,1);
                // portraitRect.Texture = player1Portrait;
                break;
            case 1:
                // labelContainer.AddThemeStyleboxOverride("panel", player2StyleBox);
                portraitRect.Modulate = new Color(1,1,1,1);
                // portraitRect.Texture = player2Portrait;
                break;
            case 2:
                // labelContainer.AddThemeStyleboxOverride("panel", player3StyleBox);
                portraitRect.Modulate = new Color(1,1,1,1);
                // portraitRect.Texture = player3Portrait;
                break;
            case 3:
                // labelContainer.AddThemeStyleboxOverride("panel", player4StyleBox);
                portraitRect.Modulate = new Color(1,1,1,1);
                // portraitRect.Texture = player4Portrait;
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
        labelContainer.Visible = true;
        // labelContainer.RemoveThemeStyleboxOverride("panel");
        portraitRect.Modulate = new Color(1, 1, 1, 10f/255);
    }
}
