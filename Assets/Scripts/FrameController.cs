using UnityEngine;
using System.Collections;
using System;

public class FrameController : MonoBehaviour {

	public int currentFrame;
	float lastFrameTime = 0;
	public GameManager game;

	public bool paused = false;

	public int maxFrames = 5;

	public void Awake()
	{
		lastFrameTime = Time.time;
	}

	public void Update()
	{
		if (!paused)
		{
			int framesToTick = (int)((Time.time - lastFrameTime) * GameManager.FPS);
			framesToTick = Math.Min(maxFrames, framesToTick);
			for (int i = 0; i < framesToTick; ++i)
			{
				tick();
			}
			lastFrameTime += framesToTick * Time.fixedDeltaTime;
		}

		if (GameManager.DEBUG)
		{
			if (Input.GetButtonDown("freeze"))
			{
				paused = !paused;
				if (!paused)
				{
					lastFrameTime = Time.time;
				}
			}

			if (paused && Input.GetButtonDown("step"))
			{
				tick();
			}
		}
	}

	private void tick()
	{
		game.TickFrame();
		currentFrame++;
	}
}
