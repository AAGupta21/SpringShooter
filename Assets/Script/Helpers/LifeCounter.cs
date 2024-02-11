using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Helper class that keeps track of how many more throws are left for user.
/// </summary>
public class LifeCounter
{
    public int BallCounter;
    public System.Action gameOverEvent;

    /// <summary>
    /// Initiates the initial throw count and the event that is to be called when user is out of throws.
    /// </summary>
    /// <param name="bCounter"></param>
    /// <param name="eve"></param>
    public LifeCounter(int bCounter, System.Action eve)
    {
        BallCounter = bCounter;
        gameOverEvent = eve;
    }

    /// <summary>
    /// Updates the counter with value that is entered, Checks if game over condition is true, and invokes the action if it is.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public int UpdateBallCounter(int val)
    {
        BallCounter += val;

        if(BallCounter < 1)
        {
            gameOverEvent?.Invoke();
        }

        return BallCounter;
    }
}