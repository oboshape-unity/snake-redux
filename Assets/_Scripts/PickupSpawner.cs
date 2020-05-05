using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour {
	[Header("Layers in game")] // layers that we wil be using in the game.
	public LayerMask collectablesLayer;
	public LayerMask gamePlayLayer;

	[Header("Drag the four walls from scene in here to set boundaries")] // used to work out game grid space
	public Transform leftWall;
	public Transform rightWall;
	public Transform topWall;
	public Transform bottomWall;

	[Header("Prefab and initial collectable count")]
	public GameObject collectable;
	public int initialCollectableCount;

	private GameObject collectableContainer;



	void OnEnable()
	{
		CreateInitialPickups(initialCollectableCount);
	}

	void OnDisable()
	{
		// clear the grid of all apples
		Destroy(collectableContainer);
	}

		

	//------------------------------------------------------------------------------------------
	// move the collectable to a clear space on the board
	//------------------------------------------------------------------------------------------
	public void Pickup_SetRandomPosition(Transform obj)
	{
		// disable the pickup that we just hit
		obj.gameObject.SetActive(false);

		Vector2 nextPickupPosition = GetRandomCoord();

		// as a failsafe as we dont what the while loop to be constantly checking for a spare space for longer than 2 seconds.
		float timeOutFailsafe = Time.time + 2f;
		while(Physics2D.OverlapCircle(nextPickupPosition, 0.4f, collectablesLayer) || Physics2D.OverlapCircle(nextPickupPosition, 0.4f, gamePlayLayer))
		{
			nextPickupPosition = GetRandomCoord();
			if (Time.time > timeOutFailsafe)
				break;
		}

		obj.position = nextPickupPosition;
		// and then reenable it
		obj.gameObject.SetActive(true);
	}


	//------------------------------------------------------------------------------------------
	// returns a random vector2 position within the game grid.
	//------------------------------------------------------------------------------------------
	Vector2 GetRandomCoord()
	{
		// due to Random.Range using ints, min is +1 so it doesnt spawn inside wall, and max is exclusive
		int randomX = Random.Range((int)leftWall.position.x+1, (int)rightWall.position.x);
		int randomY = Random.Range((int)bottomWall.position.y+1, (int)topWall.position.y);
		return new Vector2(randomX, randomY);
	}


	//------------------------------------------------------------------------------------------
	// generate the initial amount of pickups in the gameboard
	//------------------------------------------------------------------------------------------
	void CreateInitialPickups(int ammount)
	{
		// create a new Gameobject in the heirarchy to store all the collectables so its tidy
		collectableContainer = new GameObject("Collectables") as GameObject;
		for(int i = 0 ; i < initialCollectableCount ; i++)
		{
			GameObject temp = Instantiate(collectable, Vector2.zero, Quaternion.identity) as GameObject;
			// now child this new collectable to that of the created collectables container
			temp.transform.SetParent(collectableContainer.transform);
			Pickup_SetRandomPosition(temp.transform);
		}
	}
}
