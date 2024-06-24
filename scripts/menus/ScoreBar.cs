using Godot;
using System;

public partial class ScoreBar : Control
{
    public Player.Team Team;
    public int Victories;

    public int VictoryCap;

    private AnimatedSprite2D sprite;
    private ProgressBar progressBar;

    public override void _Ready()
    {
        var panelContainer = GetNode<PanelContainer>("PanelContainer");
        progressBar = panelContainer.GetNode<ProgressBar>("ProgressBar");
        progressBar.MaxValue = VictoryCap;
        progressBar.Value = Victories;

        sprite = panelContainer.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string animation = GetAnimationFromTeam(Team);
        GD.Print(animation);
        sprite.Play(animation);
    }

    private string GetAnimationFromTeam(Player.Team team)
    {
        return team switch
        {
            Player.Team.Blue => "blue_wingless_running",
            Player.Team.Red => "red_wingless_running",
            Player.Team.Yellow => "yellow_wingless_running",
            Player.Team.Green => "green_wingless_running",
            _ => throw new NotImplementedException()
        };
    }
}
