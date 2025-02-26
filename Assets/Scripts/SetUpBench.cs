using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpBench : MonoBehaviour
{
    [SerializeField] Slots Slots;
    [SerializeField] GameObject pieces;
    [SerializeField] int[] startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Get slots
        Vector3[] slots = Slots.getSlots();

        // Place the tiles in starting position
        int i = 0;
        foreach (Transform child in pieces.transform)
        {
            Vector3 pos = slots[startingPosition[i]];
            child.gameObject.transform.localPosition = pos;
            i++;
        }
    }
}
