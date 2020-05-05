using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {


	[Header("Gameplay timing settings")]
	public float 		timeStep = 0.5f;
	public float 		timeStepIncrement = 0.025f;
	private float 	minimumTimeStep = 0.1f;

	// event delagate to get the step timer to work with any subscribed object.
	public delegate void 	MoveTimer();
	public static event 		MoveTimer moveTimer;

	// need this to get the timing of the audio clip and change the timestep of the movement
	public StartOptions startOptionsScript;
	[Header("need this for resetting game")]
	public GameObject mainUIMenu;

	[Header("Game GUI Components")]
	public Text scoreText;
	public Text gameInfoText;
	public Text gameOverText;

	// Gameplay Tracker Variables
	private int 		score = 0;
	private bool 	gameOptionToRestart = false;
	private bool 	gameOver = false;

	private bool mainAudioLoopPlaying = false;

	void Start()
	{
		scoreText.enabled = true;
		scoreText.text = "";
		UpdateScoreDisplay();
		StartCoroutine(GameLoop());
	}

	public void FlagMainAudioAsPlaying(bool value)
	{
		mainAudioLoopPlaying = value;
	}

	// update, this is called every frame and this is just to get the player input, for the gameover scene
	void Update()
	{
		// if the game is over, then check for the restart keypress
		if(gameOptionToRestart)
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				Destroy(mainUIMenu);
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
	}


	// coroutine that will yield a wait depending on game timestep.
	IEnumerator GameLoop()
	{
		// Wait until this CoRoutine finishes before continuing.
		// defeats the purpose of a coRoutine, but it does the job nested nicely :)
		yield return StartCoroutine(StartDelayCoRoutine());

		// need some way to wait here until the mid game loop audio starts.
		while(!mainAudioLoopPlaying)
			yield return null;

		// Core Game Iterations
		while(true)
		{
			
			
			yield return new WaitForSeconds(timeStep);

			if (gameOver)
			{
				gameInfoText.enabled = true;
				yield return new WaitForSeconds(2);
				gameOptionToRestart = true;
				gameInfoText.text = "Press 'R' to restart...";


			}
			else
			{
				// watch for non-subscriber errors for our Event before calling it
				if(moveTimer != null)
					moveTimer();
				else
					Debug.LogError("No subscribers to moveTimer event!!");
			}
		} 
	}

	// Game starting delay coRoutine
	IEnumerator StartDelayCoRoutine()
	{
		gameInfoText.enabled = true;
		gameInfoText.text = "Get Ready..";
		yield return new WaitForSeconds(0.5f);
		gameInfoText.text = "Set ..";
		while(mainAudioLoopPlaying == false)
			yield return null;
		gameInfoText.text = "GO!";
		yield return new WaitForSeconds(0.5f);
		gameInfoText.text = "";
		gameInfoText.enabled = false;
	}







	// ==========================================
	//					                COMMON FUNCTIONS
	// ==========================================


	public void SpeedUpGame()
	{
		// make sure that timestep never gets lower than the minimum when decrementing
		timeStep = (timeStep<=minimumTimeStep) ? minimumTimeStep : (timeStep -= timeStepIncrement);

	}


	public void IncreaseScore()
	{
		score++;
		UpdateScoreDisplay();
	}



	public void UpdateScoreDisplay()
	{
		scoreText.text = "Score : " + score.ToString();
	}



	public void EndGame()
	{
		gameOver = true;
		gameOverText.enabled = true;
		gameOverText.text = "Game Over";
	}

}
