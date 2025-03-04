using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] TileGenerator TileGenerator;
    //[SerializeField] GameObject playerPointer;
    [SerializeField] GameObject playerAvatar;
    [SerializeField] BenchManager playerInfo;
    [SerializeField] TextMeshProUGUI scoreText;
    //[SerializeField] GameObject[] players;
    [SerializeField] string placePointerAbove = "scull";

    Tile[] tiles;
    Vector3 newPos;
    Vector3 verticalOffset;

    void Start()
    {
        // Variables
        tiles = TileGenerator.getTiles();
        verticalOffset = new Vector3(0, 2, 0); // how high the turn icon should appear over player's head
        
        // scoreText.color = Color.white;

        // Setup
        // placePlayerPointer();
    }

    void Update()
    {
        // displayScore();
    }

    public void evaluate()
    {
        // Compare score of all players
    }

    public Tile[] getTiles()
    {
        return tiles;
    }

    void displayScore()
    {
        scoreText.text = playerInfo.getScore().ToString();
    }

    void placePlayerPointer()
    {
        newPos = playerAvatar.transform.FindChildRecursive(placePointerAbove).position;
        newPos += verticalOffset;
        scoreText.transform.position = newPos;
        scoreText.transform.rotation = Quaternion.LookRotation(-1 * playerAvatar.transform.forward);
    }
}
