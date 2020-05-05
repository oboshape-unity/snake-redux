

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	[Header("Body prefab goes here")]
	public GameObject bodyPartPrefab;

	[Header("Sound FX Clips")]
	public AudioClip pickupSFX;
	public AudioClip slowDownSFX;

	private int bodyCount;
	private GameManager gameManager;
	private PickupSpawner pickupSpawner;
	private Vector3 moveDirection;
	private Vector3 desiredMoveDirection;
	private LayerMask gamePlayLayer;
	private Transform bodyPartOrganiser;
	private AudioSource myAudioSource;

	// declare a Queue to hold the bodypart sections of the player
	private Queue<GameObject> q_playerBodyParts;

	void Start()
	{
		q_playerBodyParts = new Queue<GameObject>();   // initialise the body parts Queue

		moveDirection = Vector2.left;  // initialise to start going left
		desiredMoveDirection = Vector2.left; // initialise to start going left

		gameManager = GameObject.FindObjectOfType<GameManager>();
		if(gameManager == null)
			Debug.LogWarning("Cannot find GameController!");

		pickupSpawner = GameObject.FindObjectOfType<PickupSpawner>();
		if(pickupSpawner == null)
			Debug.Log("Cannot find PickupSpawner!");

		// set the player layermask for the movement to only check for collisions on default layer
		gamePlayLayer = pickupSpawner.gamePlayLayer;

		myAudioSource = GetComponent<AudioSource>();

		bodyPartOrganiser = new GameObject("Body Parts").transform;
	}




	void OnEnable()
	{
		// subscribe to the moveTimer event
		GameManager.moveTimer += Move;	
	}



	void OnDisable()
	{
		// unsubscribe to moveTimer event
		GameManager.moveTimer -= Move;
	}




	void Update()
	{
		
		// Get the players input, doing this here instead of checking what key is down in the Move function
		// this way the player can change their mind each frame before commiting to a direction to move.
		if(Input.GetKey(KeyCode.LeftArrow))
			desiredMoveDirection = Vector2.left;
		else if (Input.GetKey(KeyCode.RightArrow))
			desiredMoveDirection = Vector2.right;
		else if(Input.GetKey(KeyCode.UpArrow))
			desiredMoveDirection = Vector2.up;
		else if(Input.GetKey(KeyCode.DownArrow))
			desiredMoveDirection = Vector2.down;

		//  ok here we have to check that we cannot double back directly on ourselves
		if (desiredMoveDirection == moveDirection * -1)
			desiredMoveDirection = moveDirection;

	}
		
	void Move()
	{
		// now see if moving would hit anything using the desired movenent direction
		Vector2 nextPosition = transform.position + desiredMoveDirection;

		 //check to see if the next move we are going to make hits anything on our gameplay layer
		if(Physics2D.OverlapCircle(nextPosition, 0.4f,gamePlayLayer))
		{
			gameManager.EndGame();
		}
		else
		{
			moveDirection =  desiredMoveDirection;
			// create a bodypart at current location before valid movement
			BodyManager(transform.position);
			// once all the checks are done move the player
			transform.position += moveDirection ;
		}
	}




	void BodyManager(Vector2 position)
	{
		// were going to add a new body part on each time the players head moved to new location
		GameObject bodypart = Instantiate(bodyPartPrefab, position, Quaternion.identity) as GameObject;
		bodypart.transform.SetParent(bodyPartOrganiser);
		q_playerBodyParts.Enqueue(bodypart);

		//  if theres more body parts than we should have in the Q then remove the last one
		if(q_playerBodyParts.Count > bodyCount)
		{
			Destroy(q_playerBodyParts.Dequeue());
		}
	}




	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("Collectable_Apple"))
			{
				// play the pickup sound
			myAudioSource.clip = pickupSFX;
			myAudioSource.Play();
				// move the pickup to a new location
				pickupSpawner.Pickup_SetRandomPosition(other.transform);

				// now make the total of the body count 1 longer
				bodyCount++;
				gameManager.IncreaseScore();
				gameManager.SpeedUpGame();
			}
	}



}
