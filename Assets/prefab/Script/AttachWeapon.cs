using UnityEngine;

public class AttachWeapon : MonoBehaviour
{
    public Transform target;
    public Transform holdingPoint;
    public Transform IKTarget;

    void Start()
    {
        
        //transform.position = target.position;
        //transform.rotation = target.rotation;
        transform.SetParent(target);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        
    }

    private void LateUpdate()
    {
        Attach();
    }

    private void Attach()
    {
        
        IKTarget.position = holdingPoint.position;        
        IKTarget.rotation = holdingPoint.rotation;
        
        
    }
}
