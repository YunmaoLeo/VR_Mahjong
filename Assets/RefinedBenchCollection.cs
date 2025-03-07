using System.Collections.Generic;
using UnityEngine;

public class RefinedBenchCollection : MonoBehaviour
{
    //also as mahjong rule manager
    public List<RefinedSingleBench> refinedBenches;

    public static RefinedBenchCollection Instance;
    void Start()
    {
        Instance = this;
    }

    public RefinedSingleBench GetSpecificBench(int playerId)
    {
        return refinedBenches[playerId - 1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
