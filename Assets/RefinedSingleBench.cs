using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RefinedSingleBench : MonoBehaviour
{
    [SerializeField] private List<RefinedSnapSlot> slots;

    private List<RefinedMahjongTile> _tilesInHold;

    [SerializeField] private bool debug_needDraw = false;
    [SerializeField] private bool debug_needThrow = false;

    [SerializeField] private Transform ThrowArea;
    
    // draw tile, throw tile, 
    void Start()
    {
        _tilesInHold = new List<RefinedMahjongTile>();

    }

    public void DrawTile()
    {
        //get first one
        var firstTile = TilesGenerator.Instance.GetFirstMahjong();
        //move to first available slot
        var targetSlot = GetFirstAvailableSlot();
        
        targetSlot.AssignNewTile(firstTile);
    }

    public void ThrowTile(RefinedSnapSlot slot)
    {
        if (slot.IsEmpty()) return;

        var tile = slot.CurrentTile;
        tile.DisablePhysicsComponents();
        slot.RemoveCurrentTile();
        
        TileThrowArea.Instance.ThrowTile(tile);
    }

    private RefinedSnapSlot GetFirstAvailableSlot()
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                return slot;
            }
        }

        return slots.Last();
    }

    // Update is called once per frame
    void Update()
    {
        if (debug_needDraw)
        {
            DrawTile();
            debug_needDraw = false;
        }

        if (debug_needThrow)
        {
            debug_needThrow = false;
            RefinedSnapSlot first = null;
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty()) first = slot;
            }

            ThrowTile(first);
        }
    }

    public void ResetTilePosition(RefinedSnapSlot oldSlot, RefinedMahjongTile tile)
    {
        float minDistance =float.MaxValue;
        RefinedSnapSlot targetSlot = null;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty()) continue;
            var distance = Vector3.Distance(slot.transform.position, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                targetSlot = slot;
            }
        }
        
        //move from tile position to target slot;
        targetSlot.AssignNewTile(tile);
        oldSlot.RemoveCurrentTile();   
    }

    public void EnableThrowArea()
    {
        ThrowArea.gameObject.SetActive(true);
    }
    
    public void DisEnableThrowArea()
    {
        ThrowArea.gameObject.SetActive(false);
    }
}
