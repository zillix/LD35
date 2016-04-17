using UnityEngine;
using System.Collections;

public class EnragedEyeBlur : MonoBehaviour {

	public float FadeRate = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in rendererObjects)
		{
			Color color = renderer.material.color;
			color.a -= Time.fixedDeltaTime * FadeRate;
			renderer.material.color = color;
			if (color.a <= 0)
			{
				Destroy(gameObject);
			}
		}
	}
}
