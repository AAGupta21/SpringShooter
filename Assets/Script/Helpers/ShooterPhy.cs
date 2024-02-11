using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class that takes care of all calculation.
/// </summary>
public class ShooterPhy
{
    private float maxPullAmt;
    private float minPullAmt;
    private Transform oriPos;
    private Rigidbody playerRB;
    private MeshRenderer playerMR;
    private Transform playerTR;

    /// <summary>
    /// Initializes the class object with values. Also gets reference to player's rigidbody.
    /// </summary>
    /// <param name="minPull"></param>
    /// <param name="maxPull"></param>
    /// <param name="playerO"></param>
    /// <param name="oPos"></param>
    public ShooterPhy(float minPull, float maxPull, GameObject playerO, Transform oPos)
    {
        maxPullAmt = maxPull;
        minPullAmt = minPull;
        playerRB = playerO.GetComponent<Rigidbody>();
        playerMR = playerO.GetComponent<MeshRenderer>();
        playerTR = playerO.transform;
        oriPos = oPos;
    }

    /// <summary>
    /// Calculates and adds force to the ball based on Hooke's law.
    /// </summary>
    /// <param name="firstTouch"></param>
    /// <param name="secondTouch"></param>
    /// <param name="k"></param>
    public void Shoot(Vector3 firstTouch, Vector3 secondTouch, float k)
    {
        playerMR.enabled = true;
        playerTR.position = oriPos.position;

        Vector3 fr = new Vector3();

        var direc = (firstTouch - secondTouch).normalized;

        var amt = (firstTouch - secondTouch).magnitude;

        var adjustedAmt = Mathf.Clamp(amt, minPullAmt, maxPullAmt);

        fr = direc * (k * adjustedAmt);

        playerRB.AddForce(fr, ForceMode.Impulse);
    }

    /// <summary>
    /// When a tile is hit, this calculates and adds a reverse force on the ball to reduce speed.
    /// </summary>
    /// <param name="amt"></param>
    public void HitTarget(float amt)
    {
        var direc = playerRB.velocity.normalized;
        playerRB.AddForce(-direc * amt, ForceMode.Impulse);
    }

    /// <summary>
    /// Sets the ball in initial position with zero velocity.
    /// </summary>
    public void ResetPlayer()
    {
        playerRB.velocity = Vector3.zero;
        playerTR.position = oriPos.position;
        playerMR.enabled = false;
    }
}