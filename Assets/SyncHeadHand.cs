using UnityEngine;
using Fusion;

public class SyncHeadHand : NetworkBehaviour
{
    [Networked] public Vector3 HeadPosition { get; set; }
    [Networked] public Quaternion HeadRotation { get; set; }

    [Networked] public Vector3 LHandPosition { get; set; }
    [Networked] public Quaternion LHandRotation { get; set; }

    [Networked] public Vector3 RHandPosition { get; set; }
    [Networked] public Quaternion RHandRotation { get; set; }

    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public Transform fieldTransform;

    public Transform headTarget;
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            gameObject.tag = "SyncLocal";
            headTransform = GameObject.FindWithTag("HeadTransform").GetComponent<Transform>();
            leftHandTransform = GameObject.FindWithTag("LHandTransform").GetComponent<Transform>();
            rightHandTransform = GameObject.FindWithTag("RHandTransform").GetComponent<Transform>();
        }
        else 
        {
            gameObject.tag = "SyncEnemy";
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            HeadPosition =  headTransform.position - fieldTransform.position;
            HeadRotation = Quaternion.Inverse(fieldTransform.rotation) * headTransform.rotation;

            LHandPosition = leftHandTransform.position - fieldTransform.position;
            LHandRotation = Quaternion.Inverse(fieldTransform.rotation) * leftHandTransform.rotation;

            RHandPosition = rightHandTransform.position - fieldTransform.position;
            RHandRotation = Quaternion.Inverse(fieldTransform.rotation) * rightHandTransform.rotation;
        }
        else
        {
            headTarget.position = fieldTransform.rotation * Vector3.Reflect(HeadPosition, fieldTransform.forward) + fieldTransform.position;

            leftHandTarget.position = fieldTransform.rotation * Vector3.Reflect(LHandPosition, fieldTransform.forward) + fieldTransform.position;

            rightHandTarget.position = fieldTransform.rotation * Vector3.Reflect(RHandPosition, fieldTransform.forward) + fieldTransform.position;
        }


    }

    public void SetField()
    {
        fieldTransform = GameObject.FindWithTag("Field").GetComponent<Transform>();
    }
}
