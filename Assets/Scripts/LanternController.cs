﻿using UnityEngine;
using System.Collections;

public class LanternController : MonoBehaviour, ITickable {

	public int MaxFlareFrames = 60;
	public GameObject LightAura;
	public float SmallLightAura = 3f;
	public float FlareLightAura = 6f;
	public int LightAuraPixels = 64;
	public int PPU = 16;
	
	private int flareFramesRemaining = 10;
	
	public bool IsLit { get; private set; }
	public bool IsFlaring {  get { return flareFramesRemaining > 0; } }

	void Awake()
	{

	}

	public void TickFrame () {
		Vector3 up = GameManager.instance.Up;
		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(up) - 90);
		transform.rotation = rotation;

		flareFramesRemaining--;

		float auraSize = 0;
		if (IsLit)
		{
			auraSize = IsFlaring ? FlareLightAura : SmallLightAura;
		}

		float startSize =(float)LightAuraPixels / PPU;
		float scale = auraSize / startSize;
		Vector3 lightScale = new Vector3(scale, scale, scale);
		LightAura.transform.localScale = lightScale;
	}

	public void Flare()
	{
		IsLit = true;
		flareFramesRemaining = MaxFlareFrames;
	}
}
