using UnityEngine;
using System.Collections;

public class FadeAway : MonoBehaviour {

	public float Fade = .5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in rendererObjects)
		{
			Color color = renderer.material.color;
			color.a -= Time.fixedDeltaTime * Fade;
			renderer.material.color = color;
			if (color.a <= 0)
			{
				Destroy(gameObject);
			}
		}
	}
}
