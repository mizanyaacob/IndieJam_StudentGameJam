using System.Collections.Generic;

[System.Serializable]
    public static class GameData
    {
        public static List<PlayerScoreData> PlayerScores = new List<PlayerScoreData>();
    }
public class PlayerScoreData
{
    public string playerName;
    public int score;

    public PlayerScoreData(string name, int score)
    {
        this.playerName = name;
        this.score = score;
    }


}
