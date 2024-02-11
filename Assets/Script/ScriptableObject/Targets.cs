using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Scriptable object that holds tile info.
/// </summary>
[CreateAssetMenu(menuName = "Assets/CreateTarget")]
public class Targets : ScriptableObject
{
    public string[] nameArray;
    public Target[] targetArray;

    public Dictionary<string, Target> Dict;

    public void Init()
    {
        Dict = new Dictionary<string, Target>();
        for (int i = 0; i < nameArray.Length; i++)
        {
            string name = nameArray[i];
            Dict.Add(nameArray[i], targetArray[i]);
        }
    }
}

/// <summary>
/// Class defined for the tiles info.
/// </summary>
[Serializable]
public class Target
{
    public int points;
    public ParticleSystem particleEffect;
    public float impact;
    public GameObject prefab;
    public float probability;
    public TargetActionEnum targetAction;
}