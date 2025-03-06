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
        Instance = this;
    }
    public void UpdatePlayerInfoText(int playerId, int score)
    {
        var text = playerScoreTextList[playerId - 1];
        text.gameObject.SetActive(true);
        text.text = score.ToString();
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

    public void ShowGameHelpInfo(string info)
    {
        ShowHelpInfo();
        gameInfoText.text = info;
    }

    private void ShowHelpInfo()
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
