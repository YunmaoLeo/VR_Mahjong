using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SingleBench : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointText;
    
    private int _currentScore = 0;

    private void Start()
    {
        pointText.gameObject.SetActive(false);
        pointText.transform.localScale = Vector3.zero;
        pointText.text = _currentScore.ToString();
    }

    private void UpdateTextAppearance()
    {
        pointText.text = _currentScore.ToString();
    }

    public void AddPoint(int point)
    {
        _currentScore += point;
        UpdateTextAppearance();
        if (!pointText.gameObject.activeInHierarchy)
        {
            pointText.gameObject.SetActive(true);
            pointText.transform.DOScale(Vector3.one, 0.2f);
        }
        
        
    }
    
    
}
