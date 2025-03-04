using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TilesGenerator : MonoBehaviour
{
    
    [SerializeField] private List<Sprite> tileSprites;
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform tileSpawnPoint;
    
    public static TilesGenerator Instance;
    public enum TileType
    {
        Bamboo,
        Dot,
        Tri,
    }

    public class TileInfo
    {
        public TileInfo(TileType type, int v)
        {
            Type = type;
            Value = v;
        }

        public TileType Type;
        public int Value;
    }

    public List<TileInfo> AllTiles = new List<TileInfo>();


    void Start()
    {
        Instance = this;
    }


    public void GenerateTilesAndShuffle(int seed)
    {
        var newTile = Instantiate(tilePrefab);
        
        return;
        for (int i = 0; i < 4; i++)
        {
            foreach (var ts in tileSprites)
            {
                var value = int.Parse(ts.name.Substring(ts.name.Length - 1));
                var title = ts.name.Substring(0, ts.name.Length - 1);
                switch (title)
                {
                    case "bamboo":
                        AllTiles.Add(new TileInfo(TileType.Bamboo, value));
                        break;
                    case "dot":
                        AllTiles.Add(new TileInfo(TileType.Dot, value));
                        break;
                    case "tri":
                        AllTiles.Add(new TileInfo(TileType.Tri, value));
                        break;
                }
            }
        }

        // shuffle
        Random rng = new Random(seed);
        int n = AllTiles.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (AllTiles[k], AllTiles[n]) = (AllTiles[n], AllTiles[k]);
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}