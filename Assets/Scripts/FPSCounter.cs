using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour {

	public int FPS
	{
		get; private set;
	}

	private int framesThisSecond;
	private float lastRecordedTime;

	void Update()
	{
		++framesThisSecond;
		if (Time.time >= lastRecordedTime + 1.0f)
		{
			recordTime();
		}
	}

	private void recordTime()
	{
		lastRecordedTime = Time.time;
		FPS = framesThisSecond;
		framesThisSecond = 0;
	}
}
