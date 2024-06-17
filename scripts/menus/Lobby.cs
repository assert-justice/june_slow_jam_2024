using Godot;
using System;

public partial class Lobby : Control
{
    Registration player1Registration;
    Registration player2Registration;
    Registration player3Registration;
    Registration player4Registration;
    Control readyMoon;

    [Signal]
    public delegate void StartGameEventHandler(Registration[] registrations);
    [Signal]
    public delegate void ExitLobbyEventHandler();

    bool active = false;

    public override void _Ready()
    {
        active = false;

        var container = GetNode<HFlowContainer>("HFlowContainer");
        player1Registration = container.GetNode<Registration>("Player 1 Registration");
        player2Registration = container.GetNode<Registration>("Player 2 Registration");
        player3Registration = container.GetNode<Registration>("Player 3 Registration");
        player4Registration = container.GetNode<Registration>("Player 4 Registration");

        readyMoon = GetNode<Control>("Ready Moon");
        readyMoon.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (active && CanStartGame()) {
            readyMoon.Visible = true;
        } else {
            readyMoon.Visible = false;
        }
    }


    public void SetActive(bool active) {
        this.active = active;
    }

    public override void _Input(InputEvent @event)
    {
        if (!active) return; // @todo: convert to using SetProcessInput
        HandleCancelInput();
        HandleUnjoinInput();
        HandleJoinInput();

        if (Input.IsActionJustPressed("start") && CanStartGame())
        {
            GD.Print("Starting game...");
            // emit signal to start game
            active = false;
            Registration[] registrations = new Registration[] { player1Registration, player2Registration, player3Registration, player4Registration };
            EmitSignal(SignalName.StartGame, registrations);
        }
    }


    private void HandleJoinInput()
    {
        if (Input.IsActionJustPressed("kb_join") && !player1Registration.IsRegistered())
        {
            GD.Print("Player 1 joined");
            player1Registration.Register(new PlayerSummary("kb", Player.Team.Blue));
        }
        if (Input.IsActionJustPressed("0_join") && !player1Registration.IsRegistered())
        {
            GD.Print("Player 1 joined");
            player1Registration.Register(new PlayerSummary("0", Player.Team.Blue));
        }
        if (Input.IsActionJustPressed("1_join") && !player2Registration.IsRegistered())
        {
            GD.Print("Player 2 joined");
            player2Registration.Register(new PlayerSummary("1", Player.Team.Red));
        }
        if (Input.IsActionJustPressed("2_join") && !player3Registration.IsRegistered())
        {
            GD.Print("Player 3 joined");
            player3Registration.Register(new PlayerSummary("2", Player.Team.Yellow));
        }
        if (Input.IsActionJustPressed("3_join") && !player4Registration.IsRegistered())
        {
            GD.Print("Player 4 joined");
            player4Registration.Register(new PlayerSummary("3", Player.Team.Green));
        }
    }

    private void HandleUnjoinInput()
    {
        if (Input.IsActionJustPressed("kb_unjoin") && player1Registration.IsRegistered())
        {
            GD.Print("Player 1 left");
            player1Registration.Unregister();
        }
        if (Input.IsActionJustPressed("0_unjoin") && player1Registration.IsRegistered())
        {
            GD.Print("Player 1 left");
            player1Registration.Unregister();
        }
        if (Input.IsActionJustPressed("1_unjoin") && player2Registration.IsRegistered())
        {
            GD.Print("Player 2 left");
            player2Registration.Unregister();
        }
        if (Input.IsActionJustPressed("2_unjoin") && player3Registration.IsRegistered())
        {
            GD.Print("Player 3 left");
            player3Registration.Unregister();
        }
        if (Input.IsActionJustPressed("3_unjoin") && player4Registration.IsRegistered())
        {
            GD.Print("Player 4 left");
            player4Registration.Unregister();
        }
    }

    private void HandleCancelInput() {
        if (Input.IsActionJustPressed("kb_unjoin") && !player1Registration.IsRegistered()
        || Input.IsActionJustPressed("0_unjoin") && !player1Registration.IsRegistered()
        || Input.IsActionJustPressed("1_unjoin") && !player2Registration.IsRegistered()
        || Input.IsActionJustPressed("2_unjoin") && !player3Registration.IsRegistered()
        || Input.IsActionJustPressed("3_unjoin") && !player4Registration.IsRegistered())
        {
            GD.Print("Exiting lobby");
            player1Registration.Unregister();
            player2Registration.Unregister();
            player3Registration.Unregister();
            player4Registration.Unregister();
            EmitSignal(SignalName.ExitLobby);
        }
    }

    public bool CanStartGame() {
        Registration[] registrations = new Registration[] { player1Registration, player2Registration, player3Registration, player4Registration };
        int playerCount = 0;
        foreach (Registration registration in registrations) {
            if (registration.IsRegistered()) {
                playerCount++;
            }
        }   
        return playerCount >= 2;
    }

    public bool IsKeyboardPlayerJoined() {
        return player1Registration.IsRegistered() && player1Registration.GetPlayerSummary().InputDevice == "kb";
    }
}
