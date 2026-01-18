using Oculus.Interaction.Surfaces;
using UnityEngine;
using Unity.AI.Navigation;

public class characterSpawner : MonoBehaviour
{
	public GameObject plane;
	public GameObject character1;
	public GameObject character2;
	public TMPro.TextMeshPro characterCounterText;
	int characterCounter = 0;
	private float deltaTime = 0.0f;
	public Transform spawnLocation;
	public GameObject field;

private Unity.AI.Navigation.NavMeshSurface navMeshSurface;

void Start()
{
    navMeshSurface = field.GetComponent<Unity.AI.Navigation.NavMeshSurface>();
    navMeshSurface.BuildNavMesh();
}


	void Update()
    {
		// if (OVRInput.Get(OVRInput.Button.One))
		// {
		// 	GameObject c1 = Instantiate(character1, spawnLocation.position, Quaternion.identity);
		// 	c1.GetComponent<randomRun>().movePlane = plane;
		// 	characterCounter++;
		// }
		// if (OVRInput.Get(OVRInput.Button.Two))
		// {
		// 	GameObject c2 = Instantiate(character2, spawnLocation.position, Quaternion.identity);
		// 	c2.GetComponent<randomRun>().movePlane = plane;
		// 	characterCounter++;
		// }
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

		characterCounterText.text = $"Charcter Count: {characterCounter}\nFPS: {fps:F1}";
    }
	public void SpanwCharacter1()
	{
		for (int i = 0; i < 30; i++)
		{
			GameObject c1 = Instantiate(character1, spawnLocation.position, Quaternion.identity);
			c1.GetComponent<randomRun>().movePlane = plane;
			characterCounter++;
		}
	}

	public void SpawnCharacter2()
	{
		for (int i = 0; i < 30; i++)
		{
			GameObject c2 = Instantiate(character2, spawnLocation.position, Quaternion.identity);
			c2.GetComponent<randomRun>().movePlane = plane;
			characterCounter++;
		}
	}

	public void TransparentField()
	{
		MeshRenderer fieldRenderer = plane.GetComponent<MeshRenderer>() ; 
		fieldRenderer.enabled = !fieldRenderer.enabled;
	}
}
