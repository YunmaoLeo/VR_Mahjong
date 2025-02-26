using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snap : MonoBehaviour
{
    [SerializeField] Slots Slots;
    [SerializeField] GameObject pieces;
    [SerializeField] float snapDistance = 0.05f;

    private Vector3[] slots;

    // Start is called before the first frame update
    void Start()
    {
        // Get slots
        slots = Slots.getSlots();
    }

    // Update is called once per frame
    void Update()
    {
        snapToPosition();
    }

    private void snapToPosition()
    {
        Vector3 piecePos, slotPos, pieceToSlot;

        // For each Mahjong piece
        foreach (Transform child in pieces.transform)
        {
            piecePos = child.position; // world position

            // For each slot, i.e. position on the bench
            foreach (Vector3 slot in slots)
            {
                slotPos = Slots.getWorldPos(slot); // world position

                // If the distance between their positions is less than snapDistance...
                pieceToSlot = slotPos - piecePos;
                if (pieceToSlot.magnitude < snapDistance)
                {
                    // Snap!
                    // Debug.Log("Within snapDistance!");
                    child.position = slotPos;
                    child.rotation = Slots.getRotation();
                    continue;
                }
            }
        }
    }
}
