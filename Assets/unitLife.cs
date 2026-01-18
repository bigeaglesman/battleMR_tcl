using UnityEngine;

public class unitLife : MonoBehaviour
{
    public AllyUnitManager unitManager;

    void OnDestroy()
    {
        unitManager.NotifyUnitDeath(gameObject);
    }
}
