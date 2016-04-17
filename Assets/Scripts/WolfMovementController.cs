using UnityEngine;
using System.Collections;


public class WolfMovementController : MonoBehaviour, ITickable
{
	private Vector3 position;
	public float StartRadius { get; set; }
	public float TargetRadius { get; set; }
	public Vector3 TargetAngleAnchor
	{
		get; set;
	}
	private float currentRadius;
	private Vector3 targetPosition;

	public Transform WorldCenter;

	public float AngleSpeed = 0f;

	public float RadiusSpeed = 20f;
	
	public bool AtGoal { get; set; }

	private float startZ;

	void Awake()
	{
		startZ = transform.position.z;
	}

	void Start()
	{
		WorldCenter = GameObject.Find("WorldCenter").transform;

		StartRadius = (transform.position - WorldCenter.position).magnitude;
		position = transform.position;
		TargetRadius = StartRadius;
		currentRadius = TargetRadius;

	}

	public void TickFrame()
	{

		TargetAngleAnchor = new Vector3(TargetAngleAnchor.x, TargetAngleAnchor.y, 0);
		targetPosition = (TargetAngleAnchor - WorldCenter.position).normalized * TargetRadius;



		// Actually perform the movement
		position = MathUtil.RotateVector(position, targetPosition, AngleSpeed * Time.fixedDeltaTime);
		
		if (Mathf.Abs(currentRadius - TargetRadius) < RadiusSpeed * Time.fixedDeltaTime)
		{
			currentRadius = TargetRadius;
		}
		else
		{
			if (currentRadius > TargetRadius)
			{
				currentRadius -= RadiusSpeed * Time.fixedDeltaTime;
			}
			else
			{
				currentRadius += RadiusSpeed * Time.fixedDeltaTime;
			}
		}

		position = position.normalized * currentRadius;
		position.z = startZ;

		transform.position = position;

		if( ((Vector2)(position - targetPosition)).magnitude < .5f)
		{
			AtGoal = true;
		}
		else
		{
			AtGoal = false;
		}
	}
}
