using UnityEngine;
using Fusion;
using static Oculus.Interaction.Context;

public class RoundManager : NetworkBehaviour
{
    public static RoundManager instance;
    public NetworkRunner runner;

    public UIManager2 uiManager2;

    public override void Spawned()
    {
        instance = this;
        runner = FindFirstObjectByType<NetworkRunner>();
        uiManager2 = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager2>();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (instance == this) instance = null;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_GameOver(int result)
    {
        // 1 - master win, 0 - guest win
        if (result == 1)
        {
            if (runner.IsSharedModeMasterClient)
            {
                Debug.Log("Master Win!");
                uiManager2.YouWin();
            }
            else
            {
                Debug.Log("Guest Lose!");
                uiManager2.YouLose();
            }
        }
        else if (result == 0)
        {
            if (runner.IsSharedModeMasterClient)
            {
                Debug.Log("Master Lose!");
                uiManager2.YouLose();
            }
            else
            {
                Debug.Log("Quest Win!");
                uiManager2.YouWin();
            }
        }
    }
}
