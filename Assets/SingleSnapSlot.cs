using System;
using UnityEngine;

public class SingleSnapSlot : MonoBehaviour
{
    [SerializeField] private SingleBench singleBench;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TestTile"))
        {
            if (!other.GetComponent<BoxCollider>().enabled)
            {
                return;
            }
            
            Debug.Log("Single Bench Collider Happen");
            var mahjongTile = other.gameObject.GetComponent<MahjongTile>();
            mahjongTile.OnTileSnapped();
            other.transform.position = transform.position;
            other.transform.rotation = transform.rotation;
            
            //update score on networkController and bench text
            if (NetworkController.Instance != null)
            {
                NetworkController.Instance.AddLocalPlayerPoints(mahjongTile.Point);
            }
            singleBench.AddPoint(mahjongTile.Point);
            Debug.Log($"Single Bench Added {mahjongTile.Point}");
            other.GetComponent<BoxCollider>().enabled = false;
            other.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
