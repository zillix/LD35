using UnityEngine;
using System.Collections;

public class TorchSmokeScript : MonoBehaviour {

	private Vector3 up;
	private Vector3 right;
	public float maxXVelocity = 5f;
	public float minYVelocity = 8f;
	public float maxYVelocity = 14f;
	public float yVelocity;
	public float xVelocity;

	void Start()
	{
		xVelocity = Random.Range(-maxXVelocity, maxXVelocity);
		yVelocity = Random.Range(minYVelocity, maxYVelocity);
		up = GameManager.instance.player.Torch.Physics.Up;
		right = GameManager.instance.player.Torch.Physics.Right;

		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(up) - 90);
		transform.rotation = rotation;
	}

	void Update()
	{
		Vector3 newpos = transform.position;

		// up component
		newpos += yVelocity * up * Time.fixedDeltaTime;
		//right component
		newpos += xVelocity * right * Time.fixedDeltaTime;

		transform.position = newpos;
	}
}
