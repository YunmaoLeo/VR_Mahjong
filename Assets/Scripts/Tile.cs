using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public BenchManager BenchManager;
    
    // Attributes
    [SerializeField] private string suit; // characters, bamboos, circles | dragon, wind | flowers, season
    [SerializeField] private int number; // 1-9 | 1-4
    [SerializeField] private bool special; // flowers, plants
    [SerializeField] private bool bonus; // letters

    // Animation
    private bool fly = false;
    private Vector3 pos;
    private static Quaternion rot;
    private Rigidbody rb;

    // Variables for creating an arc animation when flying to position
    Vector3 startPosition;
    float journeyLength;
    float distanceTravelled;
    float height = 1f;


    /*************************** Utility Functions ***************************/
    public bool sameTile(Tile other)
    {
        // FUNCTION: Returns true if two tiles are identical

        return (this.suit == other.suit && this.number == other.number && this.special == other.special && this.bonus == other.bonus);
    }
    public string printTile()
    {
        string printString = "Suit: " + suit + ", Number: " + number + ", Special tile: " + special + ", Bonus tile: " + bonus;
        // Debug.Log(printString);
        
        return printString;
    }

    /***************************** Set Functions ******************************/
    public void setTileSpecial(string suit, int number)
    {
        setTile(suit, number);
        setSpecialOrBonus(true, false);
    }

    public void setTileBonus(string suit, int number)
    {
        setTile(suit, number);
        setSpecialOrBonus(false, true);
    }

    public void setTile(string suit, int number)
    {
        this.suit = suit;
        this.number = number;
        setSpecialOrBonus(false, false);
    }

    public void setSpecialOrBonus(bool isSpecial, bool isBonus)
    {
        if (!(isSpecial && isBonus))
        {
            special = isSpecial;
            bonus = isBonus;
        } 
        else
        {
            Debug.Log("ERROR: Tile cannot be both a special tile and a bonus tile.");
        }
    }

    public static void setRot(Quaternion newRot)
    {
        rot = newRot;
    }

    /***************************** Get Functions ******************************/

    public int getScore()
    {
        int base_score = number;
        int multiplier = 0;
        
        if (suit == "dots" || suit == "bamboo" || suit == "triangle")
        {
            multiplier = 1;
        } else if (bonus)
        {
            multiplier = 2;
        } else if (special)
        {
            multiplier = 3;
        }
        else
        {
            Debug.LogError("Error: Unable to identify tile.");
        }

        return base_score * multiplier;
    }

    public string getSuit()
    {
        return suit;
    }

    public int getNumber()
    {
        return number;
    }

    public bool isSpecial()
    {
        return special;
    }

    public bool isBonus()
    {
        return bonus;
    }

    /***************************** Old Functions ******************************/
    /*
    private void Update()
    {
        if (fly)
        {
            flyToEmptySlotUpdate();
        }
    }
    */

    /*
    public void flyToPos(Vector3 position)
    {
        // FUNCTION: Animation that moves a tile from its current position to the given position

        fly = true;
        pos = position;
        transform.rotation = rot;
        startPosition = transform.position;
        journeyLength = Vector3.Distance(startPosition, pos);
        distanceTravelled = 0;
    }

    public void flyToEmptySlot()
    {
        // FUNCTION: Wrapper function for flyToPos. The position is the first empty slot on the player's bench

        int i = BenchManager.getFirstEmptySlot();
        Debug.Log("empty spot found: " + i);
        if (i >= 0 && i < 13)
        {
            flyToPos(BenchManager.getSlotAtIndex(i));
        }
        else { fly = false; }
    }

    public void flyToEmptySlotUpdate()
    {
        // FUNCTION: Make object fly to position with an arc path
        // Credit ChatGPT

        float speed = 2f;
        distanceTravelled += speed * Time.deltaTime;
        float t = Mathf.Clamp01(distanceTravelled / journeyLength);  // t is a value between 0 and 1
        Vector3 horizontalDirection = pos - startPosition;
        horizontalDirection.y = 0;
        Vector3 targetHorizontalPosition = startPosition + horizontalDirection * t;
        float arcHeight = Mathf.Sin(t * Mathf.PI) * height;
        transform.position = new Vector3(targetHorizontalPosition.x, startPosition.y + arcHeight, targetHorizontalPosition.z);
        if (distanceTravelled >= journeyLength)
        {
            transform.position = pos;
            fly = false;
        }
    }
    */
}
