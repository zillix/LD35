using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour, ITickable {

	public float speed;
	public float deadDist = 2f;
	private PlayerController player;

	public float lerpSpeed = 1f;

	// Use this for initialization
	void Start () {
		player = GameManager.instance.player;
	}
	
	// Update is called once per frame
	public void TickFrame () {
		float cacheZ = transform.position.z;
		Vector2 dist = (Vector2)(player.transform.position - transform.position);
		if (dist.magnitude > deadDist)
		{

			if (dist.magnitude - deadDist < speed * Time.fixedDeltaTime)
			{
				// Set exactly to dead dist
				transform.position = (Vector2)player.transform.position - dist.normalized * deadDist;
			}
			else
			{
				transform.position += (Vector3)(speed * Time.fixedDeltaTime * dist.normalized);
			}

			dist = player.transform.position - transform.position;

			Vector3 transPos = transform.position;
			transPos.z = cacheZ;
			transform.position = transPos;
		}

		Quaternion targetRotation = player.transform.rotation;
		if (player.Side== Side.Inside)
		{
			Vector3 eulers = targetRotation.eulerAngles;
			eulers.z += 180;
			targetRotation.eulerAngles = eulers;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed);
	}
}
