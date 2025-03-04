using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BenchManager : MonoBehaviour
{
    [SerializeField] Slots Slots;
    [SerializeField] int player;
    [SerializeField] GameObject pieces;

    private Vector3[] slots;
    private Tile[] tiles;

    void Start()
    {
        slots = Slots.getSlots();
    }

    private void Update()
    {
        tiles = checkBench();
    }

    public int getScore()
    {
        int sum = 0;
        
        foreach (Tile tile in tiles)
        {
            sum += tile.getScore();
        }

        return sum;
    }

    public void printBench()
    {
        Debug.Log("Bench: ");
        int i = 1;
        foreach (Tile tile in tiles)
        {
            Debug.Log("Tile #" + i + " on bench: " + tile.printTile());
            i++;
        }
    }

    private Tile[] checkBench()
    {
        List<Tile> nearbyTiles = new List<Tile>();

        foreach (Vector3 slot in slots)
        {
            if (Slots.isSlotOccupied(slot))
            {
                var tileInSlot = Slots.getTileInSlot(slot);
                if (tileInSlot != null)
                {
                    nearbyTiles.Add(tileInSlot);
                }
            }
        }
        return nearbyTiles.ToArray();
    }

    /**************************************** Old Functions *************************************/
    /*
    // [SerializeField] TurnManager TurnManager;
    // [SerializeField] Canvas PlayerUI;
    // IMPORTANT
    private string pokeName1 = "[BuildingBlock] Poke Interaction Draw";
    private string pokeName2 = "[BuildingBlock] Poke Interaction Play";

    private int numTiles = 13;
    private int numScoringSets;
    
    private Tile[] tilesFromLastTurn;
    private Tile[] tilesRemovedOnNewTurn;
    private bool dealt = false;
    */
    // Dictionary to represent scoring sets
    // private Dictionary<string, List<Tile[]>> scoringSets;
    /*
     * { "pung" : < [tile1, tile2, tile3], [...] > ,
     *   "chow" : < [tile10, tile11, tile12], [...] > ,
     *   "winds": < > }
     */
    /*
    void Start()
    {
        // Get slots
        slots = Slots.getSlots();

        // Create an array for scoring sets. It will grow every time a player makes a valid set
        scoringSets = new Dictionary<string, List<Tile[]>>();
        numScoringSets = 0;
    }

    void Update()
    {
        // Get tiles - only do this once
        if (!dealt)
        {
            tiles = TurnManager.getHand();
            // Add tiles as children to the "pieces" GameObject
            foreach (Tile tile in tiles)
            {
                tile.transform.SetParent(pieces.transform);
            }
            // Set up tiles
            setUp();
            dealt = true;
        }

        // Regular turn
        tilesFromLastTurn = getCurrentHand();
        tilesRemovedOnNewTurn = getDifference(tilesFromLastTurn);

        if (isPlayerTurn())
        {
            displayUI(true);
            checkCombos(tilesRemovedOnNewTurn);
        }
        else
        {
            displayUI(false);

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

    void displayUI(bool display)
    {
        PlayerUI.enabled = display;
        PlayerUI.transform.Find(pokeName1).gameObject.SetActive(display);
        PlayerUI.transform.Find(pokeName2).gameObject.SetActive(display);
    }

    public int getFirstEmptySlot()
    {
        // FUNCTION: Returns the first empty slot on the bench

        int i = 0;
        foreach (Vector3 slot in slots)
        {
            if (Slots.isSlotOccupied(slot))
            {
                Debug.Log("Slot " + i + " is occupied.");
            }
            else
            {
                Debug.Log("Slot " + i + " is free.");
                return i;
            }
            i++;
        }
        return -1;
    }

    public Vector3 getSlotAtIndex(int i)
    {
        //return Slots.getWorldPos(slots[i]);
        return slots[i];
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
        // FUNCTION: Checks whether the player discarded the right tiles to make a "pung" or "chow" during their turn

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

    public void setUp()
    {
        // FUNCTION: Makes 13 tiles fly to their spots on a player's bench

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].flyToPos(slots[i]);
        }
    }
    */
}
