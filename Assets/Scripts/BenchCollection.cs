using System;
using System.Collections.Generic;
using UnityEngine;

public class BenchCollection : MonoBehaviour
{
    public List<SingleBench> BenchList;

    private void Start()
    {
        foreach (var singleBench in BenchList)
        {
            singleBench.gameObject.SetActive(false);
        }
    }

    public void EnableLocalBench(int playerId)
    {
        BenchList[playerId].gameObject.SetActive(true);
    }
}
