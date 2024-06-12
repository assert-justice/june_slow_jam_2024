using Godot;
using System;

public partial class Background : Node2D
{
    Sprite2D sprite;
    Viewport viewport;

    [Export] public PackedScene Scene3D { get; set; }

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite");
        viewport = GetNode<SubViewportContainer>("SubViewportContainer").GetNode<SubViewport>("SubViewport");
        var scene = Scene3D.Instantiate();
        viewport.AddChild(scene);
    }

    public override void _Process(double delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
        Texture texture = viewport.GetTexture();
        if(texture != null){
            sprite.Texture = (Texture2D)texture;
        }
    }
}
