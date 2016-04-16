using UnityEngine;
using System.Collections;

public class WolfMovementController : MonoBehaviour, ITickable
{
	private Vector3 position;
	private float startRadius;
	private float targetRadius;
	private Vector3 targetPosition;

	public Transform WorldCenter;
	private PlayerController player;

	public float MaxAngleSpeed = 10f;
	public float AngleSpeed = 0f;

	void Awake()
	{

	}

	void Start()
	{
		WorldCenter = GameObject.Find("WorldCenter").transform;
		player = GameObject.FindObjectOfType<PlayerController>();

		startRadius = (transform.position - WorldCenter.position).magnitude;
		position = transform.position;
	}

	public void TickFrame()
	{
		targetRadius = startRadius;
		AngleSpeed = MaxAngleSpeed; 
		targetPosition = (player.transform.position - WorldCenter.position).normalized * targetRadius;

		position = MathUtil.RotateVector(position, targetPosition, AngleSpeed * Time.fixedDeltaTime);
		transform.position = position;
	}
}
