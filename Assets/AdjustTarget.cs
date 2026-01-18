//using System.Numerics;
using UnityEngine;

public class AdjustTarget : MonoBehaviour
{
    [SerializeField] Transform LHandAnchor;
    [SerializeField] Transform RHandAnchor;
    [SerializeField] Transform HeadAnchor;
    [SerializeField] Transform LHandTarget;
    [SerializeField] Transform RHandTarget;
    [SerializeField] Transform HeadTarget;
    [SerializeField] Vector3 LHandAdj;
    [SerializeField] Vector3 RHandAdj;
    [SerializeField] Vector3 HeadAdj;

    void Update()
    {
        LHandTarget.position = LHandAnchor.position;
        RHandTarget.position = RHandAnchor.position;
        HeadTarget.position = HeadAnchor.position;
        LHandTarget.rotation = LHandAnchor.rotation * Quaternion.Euler(LHandAdj) ;
        RHandTarget.rotation = RHandAnchor.rotation * Quaternion.Euler(RHandAdj);
        HeadTarget.rotation = HeadAnchor.rotation * Quaternion.Euler(HeadAdj);
    }
}
