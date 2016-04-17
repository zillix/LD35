using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, ITickable {
	public static int FPS = 60;

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

	

	public void Awake()
	{
		GameManager.instance = this;
		frameController = gameObject.AddComponent<FrameController>();
		frameController.game = this;
		fpsCounter = GetComponent<FPSCounter>();
		wolf = Instantiate(WolfPrefab).GetComponent<WolfController>();
		wolf.transform.position = GameObject.Find("WolfSpawn").transform.position;

		introManager = GetComponentInChildren<IntroManager>();
    }

	public void Start()
	{
		Lanterns = new List<LanternController>(FindObjectsOfType<LanternController>());
		Application.targetFrameRate = 60;
		TextManager = FindObjectOfType<TextManager>();

		introManager.Init();
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
		frameText.text += "\n\nWolf State : " + wolf.State;
		frameText.text += "\nWolf State Ticks: " + wolf.TicksLeftInState;
		frameText.text += "\nWolf hits: " + wolf.HitsRemaining;
		//frameText.text += "\nWolf Anim State: " + wolf.animator.GetCurrentAnimatorStateInfo(0).IsName;
	}

	public void OnWolfFleed()
	{
		// do something
	}
}
