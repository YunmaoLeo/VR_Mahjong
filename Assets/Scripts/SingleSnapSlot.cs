using System;
using UnityEngine;

public class SingleSnapSlot : MonoBehaviour
{
    [SerializeField] private SingleBench singleBench;

    private bool _hasOneTile = false;
    private MahjongTile _currentTile;

    public bool SnapToSlot(MahjongTile tile)
    {
        if (_currentTile != null)
        {
            return false;
        }
        _currentTile = tile;
        tile.transform.position = transform.position;
        tile.transform.rotation = transform.rotation;
        singleBench.AssignNewMahjong(tile);

        return true;
    }

    public bool RemoveTile(MahjongTile tile)
    {
        if (tile == _currentTile)
        {
            _hasOneTile = false;
            singleBench.RemoveMahjong(tile);
            _currentTile = null;
            return true;
        }

        return false;
    }
}
