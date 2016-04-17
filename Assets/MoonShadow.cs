using UnityEngine;
using System.Collections;

public class MoonShadow : MonoBehaviour {

	public float Speed = 2f;
	public float Radius = 30f;
// Update is called once per frame
	void Update () {
		float targetX = -Radius;

		targetX += Radius / 4 * GameManager.instance.introManager.torchesLit;

		if (!GameManager.instance.introManager.WolfHiding)
		{
			targetX += Radius / 4;
		}

		if (GameManager.instance.wolf != null)
		{
			targetX += (3 - GameManager.instance.wolf.HitsRemaining) * Radius / 4;
		}

		if (GameManager.instance.introManager.gameEnded)
		{
			targetX += Radius / 4;
		}

		if (transform.position.x < targetX)
		{
			Vector3 newpos = transform.position;
			newpos.x += Speed * Time.fixedDeltaTime;
			transform.position = newpos;
		}
	}
}
