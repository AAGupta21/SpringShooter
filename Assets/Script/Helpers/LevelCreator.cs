using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Randomizes the level based on probability factor entered in properties. Manages the pool to limit creations of extra target tiles/blocks.
/// </summary>
public class LevelCreator
{
    private Vector2 lowLimit;
    private Vector2 HighLimit;

    private Dictionary<Vector2, string> tMap;
    private string[] nameArray;
    private List<float> probChart;
    private Transform targetParent;

    private Targets pTarget;

    private Dictionary<string, Stack<GameObject>> poolDict;

    /// <summary>
    /// Initializes the variables, sets up an array that holds the probability factor for all tiles. 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetP"></param>
    /// <param name="lLimit"></param>
    /// <param name="hLimit"></param>
    public LevelCreator(Targets target, Transform targetP, Vector2 lLimit, Vector2 hLimit)
    {
        lowLimit = lLimit;
        HighLimit = hLimit;
        nameArray = target.nameArray;
        pTarget = target;
        targetParent = targetP;
        float sum = 0;
        float[] probs = new float[target.nameArray.Length];
        probChart = new List<float>();

        for (int i = 0; i < target.nameArray.Length; i++)
        {
            probs[i] = target.targetArray[i].probability;
        }

        for(int i = 0; i < probs.Length; i++)
        {
            sum += probs[i];
            probChart.Add(sum);
        }

        poolDict = new Dictionary<string, Stack<GameObject>>();
    }
    
    /// <summary>
    /// Randomizes the tiles based on probability and readies the dictionary that holds the information of the grid.
    /// </summary>
    public void CreateLevel()
    {
        if(tMap==null)
            tMap = new Dictionary<Vector2, string>();
        else
            tMap.Clear();
        
        var total = probChart[probChart.Count - 1];

        for(float i = lowLimit.x; i < HighLimit.x; i++)
        {
            for(float j = lowLimit.y; j < HighLimit.y; j++)
            {
                var random = Random.Range(0, total);
                int index = 0;
                foreach(var item in probChart)
                {
                    if(random <  item)
                    {
                        break;
                    }
                    index++;
                }
                tMap.Add(new Vector2(i, j), nameArray[index]);
            }
        }

        LevelGenerator();
    }

    /// <summary>
    /// Instantiates/pops from pool the tile and places it at the right spot.
    /// </summary>
    private void LevelGenerator()
    {
        foreach (var item in tMap)
        {
            if (poolDict.ContainsKey(item.Value))
            {
                if (poolDict[item.Value].Count > 0)
                {
                    var tempObj = poolDict[item.Value].Pop();
                    tempObj.transform.localPosition = new Vector3(item.Key.x, item.Key.y, 0f);
                    tempObj.SetActive(true);
                    continue;
                }
            }

            Object.Instantiate(pTarget.Dict[item.Value].prefab, new Vector3(item.Key.x, item.Key.y, 0f), Quaternion.Euler(0f, 0f, 180f), targetParent);
        }
    }

    /// <summary>
    /// Pushes all created tiles into the pool dict.
    /// </summary>
    public void ResetLevel()
    {
        for (int i = targetParent.childCount - 1; i >= 0; i--)
        {
            var tempT = targetParent.GetChild(i);

            if (!poolDict.ContainsKey(tempT.tag))
            {
                poolDict.Add(tempT.tag, new Stack<GameObject>());
            }

            poolDict[tempT.tag].Push(tempT.gameObject);
            tempT.gameObject.SetActive(false);
        }
    }
}
