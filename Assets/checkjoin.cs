using Fusion;
using UnityEngine;

public class checkjoin : NetworkBehaviour, IPlayerJoined
{
	void IPlayerJoined.PlayerJoined(PlayerRef player)
	{
		Debug.LogFormat("player joined");
	}

}
