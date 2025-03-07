using System.Collections.Generic;
using UnityEngine;

public class MahjongPrefabManager : MonoBehaviour
{
    public List<Transform> TriTileList;
    public List<Transform> DotTileList;
    public List<Transform> BambooTileList;

    public static MahjongPrefabManager Instance { get; private set; }

    public Transform GetAccordingMahjongModel(TilesGenerator.TileType type, int point)
    {
        switch (type)
        {
            case TilesGenerator.TileType.Bamboo:
                return BambooTileList[point - 1];
            case TilesGenerator.TileType.Dot:
                return DotTileList[point - 1];
            case TilesGenerator.TileType.Tri:
                return TriTileList[point - 1];
        }

        return null;
    }


    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
