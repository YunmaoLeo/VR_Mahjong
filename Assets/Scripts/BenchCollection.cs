using System;
using System.Collections.Generic;
using UnityEngine;

public class BenchCollection : MonoBehaviour
{
    public List<SingleBench> BenchList;
    
    public static BenchCollection Instance;
    private void Start()
    {
        Instance = this;
        foreach (var singleBench in BenchList)
        {
            singleBench.enabled = false;
        }
    }

    public void EnableSpecificBench(int playerId)
    {
        var bench = BenchList[playerId - 1];
        bench.gameObject.SetActive(true);
        bench.enabled = true;
        bench.EnableBench();
    }

    public SingleBench GetSpecificBench(int playerId)
    {
        return BenchList[playerId - 1];
    }
}
