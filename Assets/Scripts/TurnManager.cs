using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] TileGenerator TileGenerator;
    [SerializeField] GameObject playerPointer;
    [SerializeField] GameObject[] players;
    [SerializeField] string placePointerAbove = "scull";

    Tile[] tiles;
    int playerTurn;
    int playersDealt;
    int tilesPerPlayer = 13;
    bool gameStarted = false;
    Vector3 newPos;
    Vector3 verticalOffset;

    void Start()
    {
        // Get all tiles
        tiles = TileGenerator.getTiles();

        playerTurn = 0;
        playersDealt = 0;
        verticalOffset = new Vector3(0, 2, 0); // how high the turn icon should appear over player's head

        // Start game
        gameStarted = true;
    }

    void Update()
    {
        // Debug.Log("Player Turn: " + playerTurn);
        // placePlayerPointer();
    }

    public Tile[] getTiles()
    {
        return tiles;
    }

    public Tile[] getHand()
    {
        // FUNCTION: Get 13 tiles for a player. Return the tiles as an array

        Tile[] slicedArray = new Tile[tilesPerPlayer];

        if (!gameStarted)
        {
            Debug.LogError("Error: Wait for game to start before dealing.");
            return slicedArray;
        }
        
        if (playersDealt > players.Length)
        {
            Debug.LogError("Error: Too many hands dealt. Please call getHand() only ONCE per player.");
            return slicedArray;
        }

        int startIndex = tilesPerPlayer * playersDealt;
        System.Array.Copy(tiles, startIndex, slicedArray, 0, tilesPerPlayer);
        playersDealt++;

        return slicedArray;
    }

    void placePlayerPointer()
    {
        // FUNCTION: Move the pointer to point at the player whose turn it is

        newPos = players[playerTurn].transform.FindChildRecursive(placePointerAbove).position;
        newPos += verticalOffset;
        playerPointer.transform.position = newPos;
    }

    public bool isPlayerTurn(int player)
    {
        return player == playerTurn;
    }

    public void nextTurn()
    {
        if (playerTurn < 3)
        {
            playerTurn++;
        }
        else
        {
            playerTurn = 0;
        }
    }
}
