using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{
    [SerializeField] TileGenerator TileGenerator;
    [SerializeField] BenchManager BenchManager; // only for debugging purposes
    [SerializeField] GameObject pieces; // A parent object containing all pieces as children
    [SerializeField] GameObject bench;
    [SerializeField] float padding = .02f;
    [SerializeField] GameObject testObj;
    [SerializeField] Vector3 testOffset;

    private Vector3[] slots;
    private float offset;
    private Quaternion rotationWhenAligned;
    private Vector3 centerPiecePlacement;
    private float detectionDistance = 0.07f;
    private Vector3 previousPosition; // for debugging

    private void OnDrawGizmos()
    {
        // FUNCTION: Draws gizmos of slot positions for debugging
        
        Gizmos.color = Color.green;
        foreach (Vector3 slot in slots)
        {
            Gizmos.DrawWireSphere(slot, .1f);  // last arg is radius
        }
    }

    void Awake()
    {
        // Determine position of the center piece relative to bench
        centerPiecePlacement = bench.transform.position;
        //centerPiecePlacement += testOffset;
        centerPiecePlacement += new Vector3(0f, 0.08f, -0.16f);
        

        // Determine rotation of the bench for placing an entire line of tiles
        Vector3 rot = bench.transform.rotation * Vector3.forward;
        rot = Quaternion.Euler(0, 90, 0) * rot;

        // Get the local rotation of each tile
        rotationWhenAligned = Quaternion.Euler(65.16f, 19.01f, 2.45f);
        Tile.setRot(rotationWhenAligned);

        // Get the size for placement offset
        offset = TileGenerator.getOffset();

        // Fill the slots
        // Center
        Vector3 pos6 = centerPiecePlacement;
        // Right side
        Vector3 pos7 = centerPiecePlacement + rot * (offset);
        Vector3 pos8 = centerPiecePlacement + rot * (offset * 2);
        Vector3 pos9 = centerPiecePlacement + rot * (offset * 3);
        Vector3 pos10 = centerPiecePlacement + rot * (offset * 4);
        Vector3 pos11 = centerPiecePlacement + rot * (offset * 5);
        Vector3 pos12 = centerPiecePlacement + rot * (offset * 6);
        Vector3 pos13 = centerPiecePlacement + rot * (offset * 7);
        // Left side
        Vector3 pos5 = centerPiecePlacement - rot * (offset);
        Vector3 pos4 = centerPiecePlacement - rot * (offset * 2);
        Vector3 pos3 = centerPiecePlacement - rot * (offset * 3);
        Vector3 pos2 = centerPiecePlacement - rot * (offset * 4);
        Vector3 pos1 = centerPiecePlacement - rot * (offset * 5);
        Vector3 pos0 = centerPiecePlacement - rot * (offset * 6);

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

    public bool isSlotOccupied(Vector3 slot)
    {
        // Return whether there are nearby objects
        Collider[] nearbyColliders = Physics.OverlapSphere(slot, detectionDistance);
        return (nearbyColliders.Length > 0);
    }

    private void Start()
    {
        previousPosition = testObj.transform.position;
    }

    private void Update()
    {
        // Debugging:
        // Since it's VR, regular keyboard input doesn't work.
        // So instead of pressing a key to trigger a function,
        // just move the test object.
        
        if (testObj.transform.position != previousPosition)
        {
            previousPosition = testObj.transform.position;

            // Function(s) to test:
            printSlotInfo();
            BenchManager.printBench();
        }
    }

    void printSlotInfo()
    {
        // Useful for debugging
        for (int i = 0; i < 14; i++)
        {
            Vector3 slot = slots[i];
            bool tileFound = isSlotOccupied(slot);
            Debug.Log("Is slot occupied? " + tileFound);
            if (tileFound)
            {
                Tile tile = getTileInSlot(slot);
                Debug.Log("Tile found. Information about tile " + i + ": ");
                tile.printTile();
            }
        }
    }

    public Tile getTileInSlot(Vector3 slot)
    {
        // Get nearby colliders
        Collider[] nearbyColliders = Physics.OverlapSphere(slot, detectionDistance);

        // Get the GameObjects associated with the colliders
        List<Tile> nearbyTiles = new List<Tile>();
        foreach (Collider collider in nearbyColliders)
        {
            Tile possibleTile = collider.gameObject.GetComponent<Tile>();
            if (possibleTile != null)
            {
                // This GameObject is a tile!
                nearbyTiles.Add(possibleTile);
            }
        }

        if (nearbyTiles.Count > 1)
        {
            Debug.LogWarning("Warning: Unable to determine which tile is in slot. Returning random tile out of " + 
                nearbyTiles.Count + " options.");
        }

        return null;
    }
}
