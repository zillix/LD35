using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WolfMovementController))]
public class WolfController : MonoBehaviour, ITickable {

	private Animator animator;
	private WolfMovementController movement;

	void Awake()
	{
		animator = GetComponent<Animator>();
		movement = GetComponent<WolfMovementController>();
	}


	public void TickFrame()
	{
		movement.TickFrame();

		Vector3 up = (transform.position - movement.WorldCenter.position).normalized;
		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(up) + 180);
		transform.rotation = rotation;

	}
}
