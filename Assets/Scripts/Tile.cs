using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private string suit; // characters, bamboos, circles | dragon, wind | flowers, season
    private int number; // 1-9 | 1-4
    bool special; // dragon, wind
    bool bonus; // flowers, season
 
    public bool sameTile(Tile other)
    {
        return (this.suit == other.suit && this.number == other.number && this.special == other.special && this.bonus == other.bonus);
    }

    public void setTileSpecial(string suit, int number)
    {
        setTile(suit, number);
        setSpecialOrBonus(true, false);
    }

    public void setTileBonus()
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
}
