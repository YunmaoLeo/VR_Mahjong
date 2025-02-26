using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{
    [SerializeField] GameObject centerPiecePlacement;
    [SerializeField] GameObject pieces; // A parent object containing all pieces as children
    [SerializeField] GameObject bench;
    [SerializeField] float padding = .02f;

    private Vector3[] slots;
    private float offsetBeforePadding;
    private float offset;
    private Quaternion rotationWhenAligned;
    private float x0, y0, z0;
    

    void Awake()
    {
        // Get the rotation when a piece is aligned with the bench
        rotationWhenAligned = centerPiecePlacement.transform.rotation;
        
        // Get the size for placement offset
        BoxCollider boxCollider = centerPiecePlacement.GetComponent<BoxCollider>();
        offsetBeforePadding = boxCollider.size.x * centerPiecePlacement.transform.localScale.x;
        Debug.Log("Offset before padding: " + offsetBeforePadding);
        offset = offsetBeforePadding + padding;
        
        x0 = centerPiecePlacement.transform.localPosition.x;
        y0 = centerPiecePlacement.transform.localPosition.y;
        z0 = centerPiecePlacement.transform.localPosition.z;

        // Fill the slots
        // Center
        Vector3 pos6 = new Vector3(x0, y0, z0);
        // Right side
        Vector3 pos7 = new Vector3(x0 + offset, y0, z0);
        Vector3 pos8 = new Vector3(x0 + offset * 2, y0, z0);
        Vector3 pos9 = new Vector3(x0 + offset * 3, y0, z0);
        Vector3 pos10 = new Vector3(x0 + offset * 4, y0, z0);
        Vector3 pos11 = new Vector3(x0 + offset * 5, y0, z0);
        Vector3 pos12 = new Vector3(x0 + offset * 6, y0, z0);
        Vector3 pos13 = new Vector3(x0 + offset * 7, y0, z0);
        // Left side
        Vector3 pos5 = new Vector3(x0 - offset, y0, z0);
        Vector3 pos4 = new Vector3(x0 - offset * 2, y0, z0);
        Vector3 pos3 = new Vector3(x0 - offset * 3, y0, z0);
        Vector3 pos2 = new Vector3(x0 - offset * 4, y0, z0);
        Vector3 pos1 = new Vector3(x0 - offset * 5, y0, z0);
        Vector3 pos0 = new Vector3(x0 - offset * 6, y0, z0);

        slots = new Vector3[14] { pos0, pos1, pos2, pos3, pos4, pos5, pos6, pos7, pos8, pos9, pos10, pos11, pos12, pos13 };
    }

    public Vector3[] getSlots()
    {
        return slots;
    }

    public Transform getTransform()
    {
        // Transform of the Tiles parent, containing all tiles/pieces as children
        return pieces.transform; 
    }

    public Vector3 getWorldPos(Vector3 slot)
    {
        // World position of a slot in slots array
        Transform slotsTransform = pieces.transform;
        return slotsTransform.position + slotsTransform.rotation * slot;
    }

    public Quaternion getRotation()
    {
        // Rotation of the pieces themselves. They are tilted back to lie on the bench
        return rotationWhenAligned; 
    }
}
