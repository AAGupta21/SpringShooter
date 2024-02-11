using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class that takes care of the score.
/// </summary>
public class ScoreKeeper
{
    private int score;
    private int highScore;

    public ScoreKeeper(int s)
    {
        score = s;
        highScore = PlayerPrefs.GetInt("SpringHighScore", 0);
    }

    public int UpdateScore(int point)
    {
        score += point;
        return score;
    }

    public bool CheckHighScore()
    {
        if(score > highScore) 
        {
            PlayerPrefs.SetInt("SpringHighScore", score);
            return true;
        }

        return false;
    }
}
