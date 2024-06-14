
public class PlayerSummary{
    public string InputDevice = "kb";
    public Player.Team PlayerTeam = Player.Team.Blue;
    public int Kills = 0;
    public int Deaths = 0;
    public int MatchWins = 0;
    public PlayerSummary(){}
    public PlayerSummary(string inputDevice, Player.Team playerTeam){
        InputDevice = inputDevice; PlayerTeam = playerTeam;
    }
    public override string ToString(){
        return $"{InputDevice} Kills: {Kills}, Deaths: {Deaths}, Wins: {MatchWins}";
    }
}