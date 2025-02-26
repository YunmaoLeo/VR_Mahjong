using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BenchManager : MonoBehaviour
{
    [SerializeField] GameObject pieces;
    [SerializeField] TurnManager TurnManager;
    [SerializeField] int player;

    private int numTiles, numScoringSets;
    private Tile[] tiles;
    private Tile[] tilesFromLastTurn;
    private Tile[] tilesRemovedOnNewTurn;

    // Dictionary to represent scoring sets
    /*
     * { "pung" : < [tile1, tile2, tile3], [...] > ,
     *   "chow" : < [tile10, tile11, tile12], [...] > ,
     *   "winds": < > }
     */
    private Dictionary<string, List<Tile[]>> scoringSets;

    void Start()
    {
        // Get tiles
        numTiles = transform.childCount;
        tiles = new Tile[numTiles];
        for (int i = 0; i < numTiles; i++)
        {
            tiles[i] = transform.GetChild(i).GetComponent<Tile>();
        }

        // Create an array for scoring sets. It will grow every time a player makes a valid set
        scoringSets = new Dictionary<string, List<Tile[]>>();
        numScoringSets = 0;
    }

    // Update is called once per frame
    void Update()
    {
        tilesFromLastTurn = getCurrentHand();
        tilesRemovedOnNewTurn = getDifference(tilesFromLastTurn);

        if (isPlayerTurn())
        {
            checkCombos(tilesRemovedOnNewTurn);
        }
        else
        {
            // Player can still make a pung
            checkPung(tilesRemovedOnNewTurn);

            // Player can still make Mahjong
            if (numScoringSets == 4)
            {
                checkPair(tilesRemovedOnNewTurn);
            }
        }
    }

    bool isPlayerTurn()
    {
        return TurnManager.isPlayerTurn(player);
    }

    Tile[] getCurrentHand()
    {
        if (tiles.Length == numTiles)
        {
            return tiles;
        }
        if (tilesFromLastTurn == null)
        {
            Debug.Log("No valid old hand from previous turn exists!");
        }
        return tilesFromLastTurn;
    }

    Tile[] getDifference(Tile[] old)
    {
        List<Tile> diff = new List<Tile>();
        bool foundMatch;

        foreach (Tile tileOld in old)
        {
            foundMatch = false;
            foreach (Tile tile in tiles)
            {
                if (tileOld == tile)
                {
                    foundMatch = true;
                    break;
                }
            }
            if (foundMatch == false)
            {
                diff.Add(tileOld);
            }
        }
        return diff.ToArray();
    }

    bool checkCombos(Tile[] tileSet)
    {
        if (checkPung(tileSet))
        {
            List<Tile[]> existingTileSets = scoringSets["pung"];
            existingTileSets.Add(tileSet);
            scoringSets["pung"] = existingTileSets;
            numScoringSets++;
            return true;
        }
        if (checkChow(tileSet))
        {
            List<Tile[]> existingTileSets = scoringSets["chow"];
            existingTileSets.Add(tileSet);
            scoringSets["chow"] = existingTileSets;
            numScoringSets++;
            return true;
        }
        return false;
    }

    bool checkPung(Tile[] tileSet)
    {
        // Pung: 3 identical
        if (tileSet.Length != 3)
        {
            return false;
        }
        return tileSet[0].sameTile(tileSet[1]) && tileSet[0].sameTile(tileSet[2]);
    }

    bool checkPair(Tile[] tileSet)
    {
        // Pair: 2 identical
        if (tileSet.Length != 2)
        {
            return false;
        }
        return tileSet[0].sameTile(tileSet[1]);
    }

    bool checkChow(Tile[] tileSet)
    {
        // Chow: 3 sequential of same suit
        if (tileSet.Length != 3)
        {
            return false;
        }
        
        Array.Sort(tileSet);
        for (int i = 0; i < 2; i++)
        {
            // Check sequential
            if (tileSet[i].getNumber() != (tileSet[i + 1].getNumber() - 1))
            {
                return false;
            }
            // Check same suit
            if (tileSet[i].getSuit() != tileSet[i + 1].getSuit())
            {
                return false;
            }
        }
        return true;
    }
}
