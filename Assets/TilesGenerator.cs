using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;
using Random = System.Random;

public class TilesGenerator : MonoBehaviour
{
    [SerializeField] private NetworkObject tilePrefab;
    [SerializeField] private Transform refinedTilePrefab;
    [SerializeField] private Transform tileSpawnPoint;
    public static TilesGenerator Instance;

    [SerializeField] private bool debug_shouldGenerate;

    [SerializeField] private float tileSize = 0.3f;    
    [SerializeField] private float tileHeight = 0.1f;

    
    [SerializeField] private float wallLength = 13 * 0.3f;
    [SerializeField] private float wallOffset = 0.8f;

    public int HasBeenDrawed = 0;
    
    public List<RefinedMahjongTile> RefinedTilesList;

    [SerializeField] private Transform MahjongTilesPool;

    public enum TileType
    {
        Bamboo,
        Dot,
        Tri,
    }

    public class TileInfo : System.Object
    {
        public TileInfo(TileType type, int v)
        {
            Type = type;
            Value = v;
        }

        public TileType Type;
        public int Value;

        public string GetSpriteName()
        {
            return Type.ToString().ToLower() + Value.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            
            TileInfo other = (TileInfo)obj;
            return Type == other.Type && Value == other.Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + Value.GetHashCode();
                return hash;
            }
        }
    }

    public List<TileInfo> AllTiles = new List<TileInfo>();


    void Start()
    {
        Instance = this;
        RefinedTilesList = new List<RefinedMahjongTile>();
        
        GenerateTilesAndShuffle(42);
    }

    public RefinedMahjongTile GetFirstMahjong()
    {
        var result = RefinedTilesList.First();
        RefinedTilesList.RemoveAt(0);
        return result;
    }

    public void GenerateTilesAndShuffle(int seed)
    {

        Vector3[] wallPositions =
        {
            new Vector3(wallOffset, 0, 0), // Ease
            new Vector3(0, 0, -wallOffset), // South
            new Vector3(-wallOffset, 0, 0), // West
            new Vector3(0, 0, wallOffset) // North
        };

        Quaternion[] wallRotations =
        {
            Quaternion.Euler(0, 90, 0), // Ease
            Quaternion.Euler(0, 180, 0), // South
            Quaternion.Euler(0, -90, 0), // West
            Quaternion.Euler(0, 0, 0), // North
        };
        
                
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 9; j++)
            {
                AllTiles.Add(new TileInfo(TileType.Bamboo, j));
                AllTiles.Add(new TileInfo(TileType.Dot, j));
                AllTiles.Add(new TileInfo(TileType.Tri, j));
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

        int tileIndex = 0;
        
        for (int wall = 0; wall < 4; wall++)
        {
            Vector3 wallStartPos = wallPositions[wall]; // 当前墙的起点
            Quaternion wallRotation = wallRotations[wall]; // 当前墙的朝向

            for (int col = 0; col < 13; col++) // 13 列
            {
                for (int row = 1; row >=0; row--) // 2 层
                {
                    Vector3 offset = new Vector3(col * tileSize - (wallLength / 2), row * tileHeight, 0);
                    Vector3 tilePosition = wallStartPos + wallRotation * offset;
                    var tileObject = Instantiate(refinedTilePrefab, tilePosition + tileSpawnPoint.position,
                        wallRotation, MahjongTilesPool);
                    var refinedMahjongTile = tileObject.GetComponent<RefinedMahjongTile>();
                    refinedMahjongTile.TileInfo = AllTiles[tileIndex];
                    refinedMahjongTile.Point = AllTiles[tileIndex].Value;
                    refinedMahjongTile.Type = AllTiles[tileIndex].Type;
                    refinedMahjongTile.InitializeMahjongModel();
                    RefinedTilesList.Add(refinedMahjongTile);
                    tileIndex++;
                }
            }
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (debug_shouldGenerate)
        {
            GenerateTilesAndShuffle(42);
            debug_shouldGenerate = false;
        }
    }
}