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
    [SerializeField] private Transform FunctionArea;
    
    
    
    // draw tile, throw tile, 
    void Start()
    {
        DisEnableThrowArea();
        _tilesInHold = new List<RefinedMahjongTile>();

    }

    public void StartTurn()
    {
        //call network controller: xxx draw
        // detect Hu
        bool canHu = CanHu(GetTileInfos(), null);
        if (canHu)
        {
            //call network controller: xxx hu;
            Debug.Log("CanHu");
            return;
        }
        EnableThrowArea();
        //enable throw
    }

    public void EndTurn()
    {
        NetworkController.Instance.Broadcast_EndTurn();
        // call network controller: throw xxx
        // call network controller: end turn
    }

    public RefinedMahjongTile DrawTile()
    {
        //get first one
        var firstTile = TilesGenerator.Instance.GetFirstMahjong();
        //move to first available slot
        var targetSlot = GetFirstAvailableSlot();
        
        targetSlot.AssignNewTile(firstTile);

        return firstTile;
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

    public List<TilesGenerator.TileInfo> GetTileInfos()
    {        
        List<TilesGenerator.TileInfo> tileInfos = new List<TilesGenerator.TileInfo>();
        
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                continue;
            }
            tileInfos.Add(slot.CurrentTile.TileInfo);
        }

        tileInfos.OrderBy(t => t.Type).ThenBy(t => t.Value).ToList();
        return tileInfos;
    }

    public void DetectConditions(RefinedMahjongTile newTile)
    {
        var newInfo = newTile.TileInfo;
        var tileInfos = GetTileInfos();

        if (CanHu(tileInfos, newInfo))
        {
            return;
        }

        if (CanGang(tileInfos, newInfo))
        {
            return;
        }
        
        if (CanPeng(tileInfos, newInfo))
        {
            return;
        }
    }

    public bool CanHu(List<TilesGenerator.TileInfo> hand, TilesGenerator.TileInfo newTile)
    {
        if (hand.Count % 3 != 2) return false;
        return CheckHu(hand);
    }

    //back tracking
    private bool CheckHu(List<TilesGenerator.TileInfo> hand)
    {
        if (hand.Count == 0) return true;
        if (hand.Count == 2) return hand[0].Equals(hand[1]);
        
        for (int i = 0; i < hand.Count - 2; i++)
        {
            if (hand[i].Value == hand[i + 1].Value && hand[i].Value == hand[i + 2].Value)
            {
                var newHand = new List<TilesGenerator.TileInfo>(hand);
                newHand.RemoveRange(i, 3);
                if (CheckHu(newHand)) return true;
            }
        }

        for (int i = 0; i < hand.Count - 2; i++)
        {
            if (hand[i].Type == hand[i + 1].Type && hand[i].Type == hand[i + 2].Type &&
                hand[i].Value + 1 == hand[i + 1].Value && hand[i].Value + 2 == hand[i + 2].Value)
            {
                var newHand = new List<TilesGenerator.TileInfo>(hand);
                newHand.RemoveAt(i + 2);
                newHand.RemoveAt(i + 1);
                newHand.RemoveAt(i);
                if (CheckHu(newHand)) return true;
            }
        }

        return false;
    }

    public bool CanPeng(List<TilesGenerator.TileInfo> hand, TilesGenerator.TileInfo newTile)
    {
        return hand.Count(t => t.Equals(newTile)) >= 2;
    }

    public bool CanGang(List<TilesGenerator.TileInfo> hand, TilesGenerator.TileInfo newTile)
    {
        return hand.Count(t => t.Equals(newTile)) >= 3;
    }

    public bool CanChi(List<TilesGenerator.TileInfo> hand, TilesGenerator.TileInfo newTile)
    {
        if (!hand.Any(t => t.Type == newTile.Type)) return false;

        var values = hand.Where(t => t.Type == newTile.Type).Select(t => t.Value).Distinct().ToList();
       

        return values.Contains(newTile.Value - 1) && values.Contains(newTile.Value + 1) ||
               values.Contains(newTile.Value - 2) && values.Contains(newTile.Value - 1) ||
               values.Contains(newTile.Value + 1) && values.Contains(newTile.Value + 2);
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

    public void OnThrowTileEvent(TilesGenerator.TileInfo tileInfo)
    {
        NetworkController.Instance.BroadcastThrowTile(tileInfo);
        DisEnableThrowArea();
        EndTurn();
    }

    public void ThrowTileAccordingTypeInfo(TilesGenerator.TileType type, int value)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty()) continue;
            var info = slot.CurrentTile.TileInfo;
            if (info.Type == type && info.Value == value)
            {
                slot.ThrowTile();
            }
        }
    }
}
