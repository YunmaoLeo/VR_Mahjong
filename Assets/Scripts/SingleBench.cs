using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using TileInfo = TilesGenerator.TileInfo;

public class SingleBench : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private int scoreForQuadrupleCombo = 10;
    [SerializeField] private int scoreForTripleCombo = 6;
    [SerializeField] private int scoreForDoubleCombo = 2;

    [SerializeField] private float heightMultiplier = 3.0f;
    [SerializeField] private float comboAnimationDuration = 2.0f;
    [SerializeField] private float comboAnimationInterval = 0.5f;
    
    [SerializeField] private float comboScaleMultiplier = 1.25f;
    
    [SerializeField] private float comboTileDistanceMultiplier = 0.1f;
    
    [SerializeField] private bool debug_needTestComboCalculate = false;

    [SerializeField] private List<Transform> snapSlotList;
    
    private int _currentScore = 0;
    [FormerlySerializedAs("_tiles")] public List<MahjongTile> _tileList = new List<MahjongTile>();

    public void EnableBench()
    {
        snapSlotList.ForEach(slot => slot.gameObject.SetActive(true));
    }
    
    private void Start()
    {
        pointText.gameObject.SetActive(false);
        pointText.transform.localScale = Vector3.zero;
        pointText.text = _currentScore.ToString();
    }

    private void ShowText()
    {
        pointText.transform.localScale = Vector3.zero;
        pointText.gameObject.SetActive(true);
        pointText.transform.DOScale(Vector3.one, 0.25f);
    }

    private void UpdateTextAppearance()
    {
        pointText.text = _currentScore.ToString();
    }

    public void AddScore(int score)
    {
        _currentScore += score;
        NetworkController.Instance.BroadcastCurrentPlayerScoreInfo(_currentScore);
        UpdateTextAppearance();
    }

    public void AssignNewMahjong(MahjongTile tile)
    {
        _tileList.Add(tile);
    }

    public void RemoveMahjong(MahjongTile tile)
    {
        _tileList.Remove(tile);
    }

    public void ComboAnimation(List<MahjongTile> tiles)
    {
        Vector3 referenceRight = tiles[0].transform.right;
        var sortedTile =  tiles.OrderBy(
            t => Vector3.Dot(t.transform.position, referenceRight)).ToList();
        tiles = sortedTile;
        
        float targetHeight = tiles[0].transform.position.y + 1.0f;
        Vector3 finalPosition = tiles[0].transform.position;
        finalPosition.y = targetHeight;
        
        
        
        for (int i = 0; i < tiles.Count; i++)
        {
            Sequence sequence = DOTween.Sequence();
            var tile = tiles[i];
            sequence.Append(tile.transform.DOMoveY(
                targetHeight, comboAnimationDuration / 2f).SetEase(Ease.OutQuad));

            var baseDir = tiles[0].transform.right;
            var targetPosition = finalPosition + baseDir * (i * comboTileDistanceMultiplier);

            var tempI = i;
            sequence.Append(tiles[i].transform.DOMove(targetPosition, comboAnimationDuration / 2.0f)
            .SetEase(Ease.InOutQuad)
            .OnKill(() =>
            {
                var originScale = tiles[tempI].transform.localScale;
                tiles[tempI].transform.DOScale(originScale * comboScaleMultiplier, 0.1f).OnKill(() =>
                {
                    tiles[tempI].transform.DOScale(originScale, 0.05f).OnKill(() =>
                    {
                        tiles[tempI].DestroyTile();
                    });
                });
            })
            );
            
            sequence.Play();
        }


    }
    
    public void StartCalculateScore()
    {
        ShowText();
        //create a set and iterate all tiles
        foreach (var tile in _tileList)
        {
            tile.TileInfo = new TileInfo(tile.Type, tile.Point);
        }
        
        Dictionary<TileInfo, int> tileCountsDict = new Dictionary<TileInfo, int>();
        
        Sequence mainSequence = DOTween.Sequence();

        foreach (var tile in _tileList)
        {
            if (tileCountsDict.ContainsKey(tile.TileInfo))
            {
                tileCountsDict[tile.TileInfo]++;
            }
            else
            {
                tileCountsDict.Add(tile.TileInfo, 1);
            }
        }
        foreach (var kv in tileCountsDict)
        {
            
            var tileInfo = kv.Key;
            
            if (kv.Value > 1)
            {
                //get all same ones
                List<MahjongTile> sameTiles = new List<MahjongTile>();
                foreach (var tile in _tileList)
                {
                    if (Equals(tile.TileInfo, tileInfo))
                    {
                        Debug.Log("find something to integrate: " + kv.Key.Type);
                        tile.OnComboState();
                        sameTiles.Add(tile);
                    }
                }
                
                //do combo animation
                mainSequence.AppendCallback(() =>
                {
                    ComboAnimation(sameTiles);
                }).AppendInterval(comboAnimationDuration + comboAnimationInterval).AppendCallback(() =>
                {
                    AddScore(GetCorrectScore(kv.Value));
                });

            }
        }

        mainSequence.Play();
    }

    private int GetCorrectScore(int value)
    {
        switch (value)
        {
            case 2:
                return scoreForDoubleCombo;
                break;
            case 3:
                return scoreForTripleCombo;
                break;
            case 4:
                return scoreForQuadrupleCombo;
                break;
        }

        return 0;
    }

    private void Update()
    {
        if (debug_needTestComboCalculate)
        {
            debug_needTestComboCalculate = false;
            StartCalculateScore();
        }
    }
}
