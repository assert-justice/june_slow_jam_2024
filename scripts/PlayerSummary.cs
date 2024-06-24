
using Godot;

public partial class PlayerSummary : GodotObject {
    public string InputDevice = "kb";
    public Player.Team PlayerTeam = Player.Team.Blue;
    public int Kills = 0;
    public int Deaths = 0;
    public int MatchWins = 0;
    public int CrossTournamentMatchWins = 0;
    public PlayerSummary(){}
    public PlayerSummary(string inputDevice, Player.Team playerTeam){
        InputDevice = inputDevice; PlayerTeam = playerTeam;
    }
    public PlayerSummary(string inputDevice, Player.Team playerTeam, int matchWins, int crossTournamentMatchWins){
        InputDevice = inputDevice; PlayerTeam = playerTeam;
        MatchWins = matchWins; CrossTournamentMatchWins = crossTournamentMatchWins;
    }
    public override string ToString(){
        return $"Name: {InputDevice} Kills: {Kills}, Deaths: {Deaths}, Wins: {MatchWins}";
    }
}