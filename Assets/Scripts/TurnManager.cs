using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    int playerTurn;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTurn = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isPlayerTurn(int player)
    {
        return player == playerTurn;
    }

    public void nextTurn()
    {
        if (playerTurn < 4)
        {
            playerTurn++;
        }
        else
        {
            playerTurn = 1;
        }
    }
}
