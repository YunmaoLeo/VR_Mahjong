using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TurnManager TurnManager;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timePerTurn;
    private float remainingTime;

    private void Start()
    {
        remainingTime = timePerTurn;
    }

    void Update()
    {
        if(remainingTime > 0)
        {
            timerText.color = Color.white;
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            timerText.color = Color.red;
            TurnManager.nextTurn();
            remainingTime = timePerTurn;
        }
        
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{00:00}:{1:00}", minutes, seconds);
    }
}
