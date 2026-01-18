using Fusion;
using UnityEngine;

public class PlayerJoined : MonoBehaviour
{
    [SerializeField] private GameObject syncAvatar;

    public void OnplayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            NetworkObject localPlayer = runner.Spawn(syncAvatar, transform.position, Quaternion.identity, player);
            Debug.Log("Player Joined");
        }
    }
}
