using System.Numerics;
using Meta.XR.MRUtilityKit.SceneDecorator;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerAvatar : MonoBehaviour
{
    public Transform head;

    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    
    public Transform HeadLookAtPos;

    [SerializeField] private float lookAtThreshold = 0.1f;

    void Start()
    {
        
        Debug.Log("Head Up: " + head.up);
        Debug.Log("Forward: " + head.forward);
        Debug.Log("right: " +head.right);
    }

    // Update is called once per frame
    [SerializeField] private Vector3 finger0OffsetEuler;
    void Update()
    {
        var targetDirection = (HeadLookAtPos.position - head.position).normalized;
        if (Vector3.Dot(targetDirection, transform.forward) > lookAtThreshold)
        {
            head.rotation = Quaternion.LookRotation(targetDirection,
                                transform.TransformVector(new Vector3(1,0,0)))
                            * Quaternion.Euler(90,0,0);
        }
    }
}
