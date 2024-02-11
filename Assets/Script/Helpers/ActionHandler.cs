using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper class that deals with the extra effects that hitting a target may have, calling the appropriate function.
/// </summary>
public class ActionHandler
{
    LifeCounter lifeCounter;
    ShooterPhy shooterPhy;
    ScoreKeeper scoreKeeper;

    TextMeshProUGUI lifeText;
    TextMeshProUGUI scoreText;

    /// <summary>
    /// Initializes the class with other helper classes, since consequences more than likely are handled by them.
    /// </summary>
    /// <param name="lifeCounter"></param>
    /// <param name="shooterPhy"></param>
    /// <param name="scoreKeeper"></param>
    /// <param name="lifeText"></param>
    /// <param name="scoreText"></param>
    public ActionHandler(LifeCounter lifeCounter, ShooterPhy shooterPhy, ScoreKeeper scoreKeeper, TextMeshProUGUI lifeText, TextMeshProUGUI scoreText)
    {
        this.lifeCounter = lifeCounter;
        this.shooterPhy = shooterPhy;
        this.scoreKeeper = scoreKeeper;
        this.scoreText = scoreText;
        this.lifeText = lifeText;
    }

    /// <summary>
    /// Checks the type of target block that was hit and performs action based on that.
    /// </summary>
    /// <param name="t"></param>
    public void PerformAction(Target t)
    {
        switch (t.targetAction)
        {
            case TargetActionEnum.None:
                scoreText.text = scoreKeeper.UpdateScore(t.points).ToString();
                break;

            case TargetActionEnum.HealthBonus:
                lifeText.text = lifeCounter.UpdateBallCounter(t.points).ToString(); 
                break;

            case TargetActionEnum.BonusForce:
                shooterPhy.HitTarget(t.impact);
                break;
        }
    }
}

/// <summary>
/// Enum to identify the consequences.
/// </summary>
public enum TargetActionEnum
{
    None,
    HealthBonus,
    BonusForce
}