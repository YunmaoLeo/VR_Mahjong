using System;
using UnityEngine;

public class SingleSnapSlot : MonoBehaviour
{
    [SerializeField] private SingleBench singleBench;

    private bool _hasOneTile = false;

    public void SnapToSlot(MahjongTile tile)
    {
        tile.transform.position = transform.position;
        tile.transform.rotation = transform.rotation;
        singleBench.AssignNewMahjong(tile);
    }

    public void RemoveTile(MahjongTile tile)
    {
        singleBench.RemoveMahjong(tile);
    }
}
