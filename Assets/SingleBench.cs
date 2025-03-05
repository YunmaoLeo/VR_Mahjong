using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SingleBench : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointText;
    
    private int _currentScore = 0;
    private List<MahjongTile> _tiles = new List<MahjongTile>();

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

    public void AssignNewMahjong(MahjongTile tile)
    {
        _tiles.Add(tile);
    }

    public void RemoveMahjong(MahjongTile tile)
    {
        _tiles.Remove(tile);
    }


}
