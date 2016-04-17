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

	public FrameController frameController;
	public int currentFrame {  get { return frameController.currentFrame; } }

	private FPSCounter fpsCounter;

	public List<LanternController> Lanterns = new List<LanternController>();

	public bool RotateGravity = true;
	public Vector3 Up {  get { return player.Physics.Up; } }

	public void Awake()
	{
		GameManager.instance = this;
		frameController = gameObject.AddComponent<FrameController>();
		frameController.game = this;
		fpsCounter = GetComponent<FPSCounter>();
	}

	public void Start()
	{
		Lanterns = new List<LanternController>(FindObjectsOfType<LanternController>());
		Application.targetFrameRate = 60;
	}

	public void TickFrame()
	{
		player.TickFrame();
		wolf.TickFrame();

		foreach (LanternController lantern in Lanterns)
		{
			lantern.TickFrame();
		}

		mainCamera.TickFrame();

		fpsText.text = "FPS: " + fpsCounter.FPS;
		frameText.text = "Current Frame: " + frameController.currentFrame;
		frameText.text += "\n\nWolf State : " + wolf.State;
		frameText.text += "\nWolf State Ticks: " + wolf.TicksLeftInState;
	}
}
