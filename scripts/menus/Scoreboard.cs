using Godot;
using System;
using System.Linq;

public partial class Scoreboard : Control
{
    public PackedScene scoreBarScene;
    VBoxContainer scoreBarContainer;
    PlayerSummary[] summaries;
    ScoreBar[] scorebars;
    bool active;
    [Signal]
    public delegate void ExitScoreboardEventHandler();

    public override void _Ready()
    {
        active = false;
        scoreBarScene = GD.Load<PackedScene>("res://scripts/menus/score_bar.tscn");
        scoreBarContainer = GetNode<VBoxContainer>("VictoriesContainer");
        summaries = new PlayerSummary[] {
            new PlayerSummary("0", Player.Team.Blue),
            new PlayerSummary("1", Player.Team.Red),
            new PlayerSummary("2", Player.Team.Yellow),
            new PlayerSummary("3", Player.Team.Green),
        };

        scorebars = new ScoreBar[summaries.Length];
        for (int i = 0; i < summaries.Length; i++)
        {
            var scoreBar = scoreBarScene.Instantiate<ScoreBar>();
            scoreBar.Initialize(summaries[i].PlayerTeam, summaries.Length - 1 - i, summaries.Length - 1, i);
            scoreBar.Name = $"ScoreBar {i}";
            scoreBarContainer.AddChild(scoreBar);
            scorebars[i] = scoreBar;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if(!active) return; 

        if (@event.IsActionPressed("ui_accept"))
        {
            EmitSignal(SignalName.ExitScoreboard);
            GetViewport().SetInputAsHandled();
        }
    }

    public void SetSummaries(PlayerSummary[] summaries)
    {
        this.summaries = summaries.OrderByDescending(s => s.MatchWins).ToArray();
        // clean up old scorebars
        foreach (var child in scoreBarContainer.GetChildren())
        {
            if (child is ScoreBar scoreBar)
            {
                scoreBar.QueueFree();
            }
        }
        scorebars = new ScoreBar[this.summaries.Length];
        for (int i = 0; i < this.summaries.Length; i++)
        {
            var scoreBar = scoreBarScene.Instantiate<ScoreBar>();
            // todo: make this a setup function or something
            scoreBar.Initialize(
                this.summaries[i].PlayerTeam,
                this.summaries[i].MatchWins,
                3,
                i
            );
            scoreBar.Name = $"ScoreBar {i}";
            scoreBarContainer.AddChild(scoreBar);
            scorebars[i] = scoreBar;
        }
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }
}
