using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Lobby : Control
{
    Registration player1Registration;
    Registration player2Registration;
    Registration player3Registration;
    Registration player4Registration;
    Control readyMoon;
    Dictionary<int, int> deviceToSlot;

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

        deviceToSlot = new Dictionary<int, int>();
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

        HandleCancelInput(@event);
        HandleUnjoinInput(@event);
        HandleJoinInput(@event);

        if (@event.IsActionPressed("start") && CanStartGame())
        {
            GD.Print("Starting game...");
            // emit signal to start game
            active = false;
            Registration[] registrations = new Registration[] { player1Registration, player2Registration, player3Registration, player4Registration };
            EmitSignal(SignalName.StartGame, registrations);
			GetViewport().SetInputAsHandled();
        }
    }

    private (int, Player.Team, Registration) GetFirstAvailableRegistration() {
        if (!player1Registration.IsRegistered()) {
            return (0, Player.Team.Blue, player1Registration);
        }
        if (!player2Registration.IsRegistered()) {
            return (1, Player.Team.Red, player2Registration);
        }
        if (!player3Registration.IsRegistered()) {
            return (2, Player.Team.Yellow, player3Registration);
        }
        if (!player4Registration.IsRegistered()) {
            return (3, Player.Team.Green, player4Registration);
        }
        return (-1, Player.Team.Yellow, null);
    }


    private void HandleJoinInput(InputEvent @event)
    {
        if(@event.IsActionPressed("join")) {
            if(deviceToSlot.ContainsKey(@event.Device)) return; // already joined

            var (slot, team, registration) = GetFirstAvailableRegistration();
            if (slot == -1) {
                return; // no slots available
            }

            string deviceName = @event.Device == 0 && @event is InputEventKey ? "kb" : @event.Device.ToString();
            GD.Print($"Player {slot + 1} joined! ({@event.Device}, {deviceName})");
            registration.Register(new PlayerSummary(deviceName, team), slot);
            deviceToSlot.Add(@event.Device, slot);
			GetViewport().SetInputAsHandled();
        }
    }

    private void HandleUnjoinInput(InputEvent @event)
    {
        if(!@event.IsActionPressed("unjoin")) return;
        if (!deviceToSlot.ContainsKey(@event.Device)) return;

        switch(deviceToSlot[@event.Device]) {
            case 0:
                if (player1Registration.IsRegistered()) {
                    GD.Print("Player 1 left");
                    player1Registration.Unregister();
                    deviceToSlot.Remove(@event.Device);
                    GetViewport().SetInputAsHandled();
                }
                break;
            case 1:
                if (player2Registration.IsRegistered()) {
                    GD.Print("Player 2 left");
                    player2Registration.Unregister();
                    deviceToSlot.Remove(@event.Device);
                    GetViewport().SetInputAsHandled();
                }
                break;
            case 2:
                if (player3Registration.IsRegistered()) {
                    GD.Print("Player 3 left");
                    player3Registration.Unregister();
                    deviceToSlot.Remove(@event.Device);
                    GetViewport().SetInputAsHandled();
                }
                break;
            case 3:
                if (player4Registration.IsRegistered()) {
                    GD.Print("Player 4 left");
                    player4Registration.Unregister();
                    deviceToSlot.Remove(@event.Device);        
                    GetViewport().SetInputAsHandled();
                }
                break;
        } 
    }

    private void HandleCancelInput(InputEvent @event) {
        if(!@event.IsActionPressed("unjoin")) return;

        if (!deviceToSlot.ContainsKey(@event.Device))
        {
            GD.Print("Exiting lobby");
            player1Registration.Unregister();
            player2Registration.Unregister();
            player3Registration.Unregister();
            player4Registration.Unregister();
            deviceToSlot.Clear();
            EmitSignal(SignalName.ExitLobby);
            GetViewport().SetInputAsHandled();
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
