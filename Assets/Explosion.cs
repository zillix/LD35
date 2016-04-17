using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public float AngleSpeed = 360f;
	public float SizeGrowthSpeed = 5f;

	private float currentAngle = 0f;
	private float currentSize;

	void Start()
	{
		currentSize = 4f;
	}
	

// Update is called once per frame
	void Update () {

		currentSize += Time.fixedDeltaTime * SizeGrowthSpeed;
		currentAngle += Time.fixedDeltaTime * AngleSpeed;


		float startSize = (float)64 / 16; // size / ppu
		float scale = currentSize / startSize;
		Vector3 lightScale = new Vector3(scale, scale, scale);
		transform.localScale = lightScale;

		transform.rotation = Quaternion.Euler(0, 0, currentAngle);
	}
}
