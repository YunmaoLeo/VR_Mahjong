using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] BenchManager BenchManager;
    [SerializeField] GameObject allPieces; // empty parent for all tiles
    [SerializeField] Transform origin; // the center of the tile setup
    [SerializeField] int sidelength = 13; // number of tiles that make up a side of the square

    private Tile[] tiles;
    private Material[] materials;
    private int num_tiles = 144; // total tiles
    private float offset; // block width
    private float height; // block height
    private float padding = .02f; // space between tiles
    private int base_num; // Calculated from side length. Helpful for building the formation

    void Awake()
    {
        // Calculate base number from side length
        if (sidelength % 2 == 0)
        {
            Debug.Log("Side length must be an odd number. Increasing sidelength by 1.");
            sidelength += 1;
        }
        base_num = sidelength / 2;

        // Get the approximate size of a tile used for placement offset
        BoxCollider boxCollider = tilePrefab.GetComponent<BoxCollider>();
        float offsetBeforePadding = boxCollider.size.x * tilePrefab.transform.localScale.x;
        float heightBeforePadding = boxCollider.size.z * tilePrefab.transform.localScale.z;
        offset = offsetBeforePadding + padding;
        height = heightBeforePadding + padding;

        // Materials
        loadMaterials();

        // Generate
        tiles = generateTiles();

        // Shuffle
        shuffle(tiles);

        // Position
        positionTiles();

        // printAllTiles();
    }

    public float getOffset()
    {
        return offset;
    }

    public Tile[] getTiles()
    {
        return tiles;
    }

    void loadMaterials()
    {
        materials = Resources.LoadAll<Material>("Mahjong Tiles"); // load materials from Resources/'Mahjong Tiles' folder
    }

    Material getMaterial(int materialNum)
    {
        if (materialNum >= materials.Length)
        {
            return materials[0];
        }
        return materials[materialNum];
    }

    void printAllTiles()
    {
        int i = 1;
        foreach (Tile tile in tiles)
        {
            Debug.Log("Tile " + i + ": " + tile.printTile());
            i++;
        }
    }

    Tile generateTile(int tileNum)
    {
        // FUNCTION: Generate one tile

        GameObject newTile = Instantiate(tilePrefab, Vector3.zero, origin.rotation);
        Tile tile = newTile.GetComponent<Tile>();
        tile.BenchManager = BenchManager;

        // Set texture of tile
        Renderer renderer = newTile.GetComponent<Renderer>();
        renderer.material = getMaterial(tileNum);

        // Parent it
        tile.transform.SetParent(allPieces.transform);

        return tile;
    }
    
    Tile[] generateTilestest()
    {
        // FUNCTION: Generates tiles (testing stage)

        Tile[] tiles = new Tile[num_tiles];
        
        for (int i = 0; i < num_tiles; i++)
        {
            tiles[i] = generateTile(i);
        }

        return tiles;
    }

    Tile[] generateTiles()
    {
        // FUNCTION: Generates tiles
        // Order: dots, bamboo, triangle, flowers (4), plants (4), letters (7)

        tiles = new Tile[num_tiles];
        string[] suits = { "dots", "bamboo", "triangle" };
        string[] specialSuits = { "flowers", "plants" };

        int tilesAdded = 0;

        // Suits
        foreach (string suit in suits)
        { 
            for (int j = 1; j <= 9; j++)
            {
                // 4 duplicates of each
                for (int k = 0; k < 4; k++)
                {
                    Tile newTile = generateTile(tilesAdded);
                    newTile.setTile(suit, j); // set suit and number
                    tiles[tilesAdded] = newTile;
                    tilesAdded++;
                }
            }
        }

        // Flowers and Plants
        foreach (string suit in specialSuits)
        {
            for (int j = 1; j <= 4; j++)
            {
                 Tile newTile = generateTile(tilesAdded);
                 newTile.setTileSpecial(suit, j); // set suit and number
                 tiles[tilesAdded] = newTile;
                 tilesAdded++;
            }
        }

        // Letters
        for (int j = 1; j <= 7; j++)
        {
            for (int k = 0; k < 4; k++)
            {
                Tile newTile = generateTile(tilesAdded);
                newTile.setTileBonus("letters", j);
                tiles[tilesAdded] = newTile;
                tilesAdded++;
            }
        }

        Debug.Log("Total tiles generated (should be " + num_tiles + "): " + tilesAdded); // check 144 tiles were generated
        return tiles;
    }

    public void positionTiles()
    {
        // FUNCTION: Positions the tiles in two layers

        int tile_num = num_tiles - 1;
        tile_num = placeLayer(tile_num, 0);
        tile_num = placeLayer(tile_num, height);
    }

    int placeTile(int tile_num, Vector3 pos, Quaternion rot)
    {
        // FUNCTION: Positions a single tile based on position and rotation

        tiles[tile_num].transform.position = pos;
        tiles[tile_num].transform.rotation = rot;
        return tile_num - 1;
    }

    int placeLayer(int tile_num, float height)
    {
        // FUNCTION: Main algorithm for positioning a single layer of tiles

        // Repeats in 4 directions - keep track of which direction you are in
        Vector3 forward, left, right, starting_point;
        Vector3 upward = new Vector3(0, height, 0);
        Quaternion rot;
        
        for(int i = 0; i < 4; i++)
        { 
            // Directions
            forward = origin.rotation * Vector3.forward;
            forward = Quaternion.Euler(0, 90*i, 0) * forward; // different for each loop
            left = Quaternion.Euler(0, 90, 0) * forward;
            right = Quaternion.Euler(0, -90, 0) * forward;

            // Walk to starting position
            float distance = offset * (base_num + 1.5f);
            starting_point = origin.position + forward * distance + upward; // walk forward
            starting_point = starting_point + right * (offset * base_num); // walk right

            // Place tiles
            rot = Quaternion.LookRotation(forward);
            for (int j = 0; j < num_tiles / 8; j++)
            {
                tile_num = placeTile(tile_num, starting_point + left * offset * j, rot);
            }
        }
        return tile_num;
    }

    private Tile[] shuffle(Tile[] arr)
    {
        // FUNCTION: Shuffles an array
        // Credit ChatGPT

        System.Random rng = new System.Random();
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            var temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
        return arr;
    }

    // Test: Generate deck

    // Make four copies of a tile
    // characters 1-9 x4
    // bamboo 1-9 x4
    // dots 1-9 x4
    // letters 1-7 x4
    // flowers 1-4
    // plants 1-4

    // My script needs to instantiate a specified number of copies of them and position them in the middle
    // They should all be building blocks
    // Test: See if you can make one copy of a tile that has all the properties
    // Test: See if you can make two copies (3 of the same tile, total)
    // Test: See if you can add them to an array
    // Test: see if you can print each tile in the array
}
