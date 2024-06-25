using Godot;
using System;

public partial class ScoreBar : Control
{
    [Export] public StyleBoxTexture player1StyleBox;
    [Export] public StyleBoxTexture player2StyleBox;
    [Export] public StyleBoxTexture player3StyleBox;
    [Export] public StyleBoxTexture player4StyleBox;
    private int rank; // 0 = first, 1 = second, 2 = third, 3 = fourth
    private Player.Team team;
    private int victories;
    private int victoryCap;
    private AnimatedSprite2D sprite;
    private ProgressBar progressBar;

    public override void _Ready()
    {
        Render();
    }

    public void Initialize(Player.Team team, int victories, int victoryCap, int rank)
    {
        this.team = team;
        this.victories = victories;
        this.victoryCap = victoryCap;
        this.rank = rank;

        Render();
    }

    private void Render() 
    {
        var panelContainer = GetNode<PanelContainer>("PanelContainer");
        progressBar = panelContainer.GetNode<ProgressBar>("ProgressBar");
        progressBar.MaxValue = victoryCap;
        progressBar.Value = victories;
        progressBar.AddThemeStyleboxOverride("fill", GetFillStylebox());

        sprite = panelContainer.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string animation = GetAnimationName();
        GD.Print(animation);
        sprite.Play(animation);
    }

    private string GetAnimationName()
    {
        var teamParticle = this.team switch {
            Player.Team.Blue => "blue",
            Player.Team.Red => "red",
            Player.Team.Yellow => "yellow",
            Player.Team.Green => "green",
            _ => throw new NotImplementedException()
        };

        var wingStateParticle = this.rank switch {
            0 => "winged",
            _ => "wingless"
        };

        var behaviorParticle = this.victories switch {
            0 => "idle",
            _ => "running"
        };

        return $"{teamParticle}_{wingStateParticle}_{behaviorParticle}";
    }

    private StyleBoxTexture GetFillStylebox()
    {
        GD.Print(this.team);
        return this.team switch
        {
            Player.Team.Blue => player1StyleBox,
            Player.Team.Red => player2StyleBox,
            Player.Team.Yellow => player3StyleBox,
            Player.Team.Green => player4StyleBox,
            _ => throw new NotImplementedException()
        };
    }
}
