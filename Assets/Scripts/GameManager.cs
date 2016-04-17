using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, ITickable {
	public static int FPS = 60;

	public static bool DEBUG = true;

	public static GameManager instance;

	public PlayerController player;
	public WolfController wolf;
	public Text fpsText;
	public Text frameText;
	public CameraController mainCamera;
	public TextManager TextManager;

	public FrameController frameController;
	public int currentFrame {  get { return frameController.currentFrame; } }

	private FPSCounter fpsCounter;

	public List<LanternController> Lanterns = new List<LanternController>();

	public GameObject WolfPrefab;

	public IntroManager introManager;


	public bool RotateGravity = true;
	public Vector3 Up {  get { return player.Physics.Up; } }

	public bool BattleStarted { get { return introManager.BattleStarted; } }

	private bool flippedOnce = false;

	public void Awake()
	{
		GameManager.instance = this;
		frameController = gameObject.AddComponent<FrameController>();
		frameController.game = this;
		fpsCounter = GetComponent<FPSCounter>();


		introManager = GetComponentInChildren<IntroManager>();
    }

	public void Start()
	{
		Lanterns = new List<LanternController>(FindObjectsOfType<LanternController>());
		Application.targetFrameRate = 60;
		TextManager = FindObjectOfType<TextManager>();

		introManager.Init();
	}

	public void OnFlip()
	{
		if (flippedOnce)
		{
			return;
		}

		flippedOnce = true;

		wolf = Instantiate(WolfPrefab).GetComponent<WolfController>();
		wolf.transform.position = (player.transform.position.normalized * -2);
		Vector3 newpos = wolf.transform.position;
		newpos = Vector3.zero;
		newpos.z = 0;
		wolf.transform.position = newpos;
		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(((Vector2)(player.transform.position)).normalized * -1) + 180);
		wolf.transform.rotation = rotation;

	}




	public void TickFrame()
	{
		if (!introManager.IntroStarted)
		{
			if (Input.anyKey)
			{
				introManager.StartIntro();
			}
			return;
		}


		player.TickFrame();


		if (BattleStarted)
		{
			wolf.TickFrame();
		}

		foreach (LanternController lantern in Lanterns)
		{
			lantern.TickFrame();
		}

		mainCamera.TickFrame();

		fpsText.text = "FPS: " + fpsCounter.FPS;
		frameText.text = "Current Frame: " + frameController.currentFrame;
		if (wolf != null)
		{
			frameText.text += "\n\nWolf State : " + wolf.State;
			frameText.text += "\nWolf State Ticks: " + wolf.TicksLeftInState;
			frameText.text += "\nWolf hits: " + wolf.HitsRemaining;
		}
		//frameText.text += "\nWolf Anim State: " + wolf.animator.GetCurrentAnimatorStateInfo(0).IsName;
	}

	public void OnWolfFleed()
	{
		// do something
	}
}
