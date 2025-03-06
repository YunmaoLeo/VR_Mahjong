using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameInfoTextManager : MonoBehaviour
{
    public static GameInfoTextManager Instance;

    [SerializeField] private TextMeshProUGUI gameInfoText;
    
    [SerializeField] private List<TextMeshProUGUI> playerScoreTextList;
    void Start()
    {
        
    }
    public void UpdatePlayerInfoText(int playerId)
    {
        var text = playerScoreTextList[playerId - 1];
        text.gameObject.SetActive(true);
        text.text = playerId.ToString();
    }

    IEnumerator HideInfoTextWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        HideHelpInfo();
    }

    public void ShowGameHelpInfoWithDelay(string info, float duration)
    {
        ShowHelpInfo();
        gameInfoText.text = info;
        StartCoroutine(HideInfoTextWithDelay(duration));
    }

    public void ShowHelpInfo()
    {
        gameInfoText.gameObject.SetActive(true);
    }

    public void HideHelpInfo()
    {
        gameInfoText.gameObject.SetActive(false);
    }

    
    void Update()
    {
        
    }
}
