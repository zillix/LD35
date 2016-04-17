using UnityEngine;
using System.Collections;

public class PlayerPhysicsController : MonoBehaviour, ITickable {
	public float MoveAcceleration = 20f;


	public Vector3 Velocity;
	private Vector3 acceleration;
	public Vector3 Position;

	public Vector3 Friction = new Vector3(20, 2);
	public Vector3 MaxSpeed = new Vector3(25, 20);
	public float DodgeSpeed = 20;
	public float GroundGravity = -20f;
	public float AirGravity = -20f;
	public float HoverDist = .05f;
	public float RaycastDist = .06f;
	public int MaxDodgeFrames = 10;
	
	public bool IsGrounded
	{
		get; set;
	}

	public Vector3 Up { get; private set; }
	public Vector3 Right {  get { return new Vector3(Up.y, -Up.x, -Up.z); } }

	public void SetUp(Vector3 up) { Up = up; }

	private Collider2D surface = null;
	private int currentDodgeFrames = 0;

	private int lastDirectionHeld = 0;

	int groundLayerMask;

	public bool DisableGravity = false;

	public bool IsDodging {  get { return currentDodgeFrames > 0; } }

	public bool UncapSpeeds { get; set; }

	private GameObject leftBound;
	private GameObject rightBound;

	void Awake()
	{
		Velocity = Vector3.zero;
		Up = new Vector3(0, 1, 0);
		Position = transform.position;
		groundLayerMask = LayerUtil.GetLayerMask(LayerUtil.MOON);
		Position.z = 0;
	}

	void Start()
	{
		leftBound = GameObject.Find("LeftBound");
		rightBound = GameObject.Find("RightBound");
	}

	public void TickFrame()
	{
		if (!DisableGravity)
		{
			if (IsGrounded)
			{
				Velocity.x += GroundGravity * Up.x * Time.fixedDeltaTime;
				Velocity.y += GroundGravity * Up.y * Time.fixedDeltaTime;
			}
			else
			{
				Velocity.x += AirGravity * Up.x * Time.fixedDeltaTime;
				Velocity.y += AirGravity * Up.y * Time.fixedDeltaTime;
			}
		}

		Velocity.x += acceleration.x * Right.x * Time.fixedDeltaTime;
		Velocity.y += acceleration.x * Right.y * Time.fixedDeltaTime;

		float rightDot = Vector3.Dot(Velocity, Right);

		// Cap velocity at max speed
		if (!UncapSpeeds)
		{
			float maxSpeedX = IsDodging ? DodgeSpeed : MaxSpeed.x;



			if (!GameManager.instance.introManager.BattleStarted && !IsDodging)
			{
				maxSpeedX = 3;
	        }

			if (rightDot > maxSpeedX)
			{
				float upDot = Vector3.Dot(Velocity, Up);
				Velocity = upDot * Up + (Vector3)(maxSpeedX * Right);
			}
			else if (rightDot < -maxSpeedX)
			{
				float upDot = Vector3.Dot(Velocity, Up);
				Velocity = upDot * Up + (Vector3)(-maxSpeedX * Right);
			}
		}

		// Recalculate velocity
		rightDot = Vector3.Dot(Velocity, Right);

		// Lose velocity to friction
		if (!IsDodging
			&& (lastDirectionHeld == 0)
			&& IsGrounded
			&& Velocity.sqrMagnitude > .01f)
		{
			if (rightDot > 0)
			{
				float upDot = Vector3.Dot(Velocity, Up);
				Velocity = upDot * Up + (Vector3)(Mathf.Max(0, rightDot - Friction.x) * Right);
			}
			else if (rightDot < 0)
			{
				float upDot = Vector3.Dot(Velocity, Up);
				Velocity = upDot * Up + (Vector3)(Mathf.Min(0, rightDot + Friction.x) * Right);
			}
		}

		// Don't check feet if moving up
		if (Vector3.Dot(Velocity, Up) < .1f)
		{
			RaycastHit2D hit = Physics2D.Raycast(Position, Up * -1, RaycastDist, groundLayerMask);
			if (hit.collider != null)
			{
				if (hit.distance == 0)
				{
					Debug.LogWarning("Collision with dist 0");
				}
				surface = hit.collider;

				//if (GameManager.instance.introManager.RotateWorld)
				{
					Up = hit.normal;
				}

				IsGrounded = true;
				UncapSpeeds = false;

				Position += Up * -1 * (hit.distance - HoverDist);
				// Set position to the collision point
				//position = hit.point;

				// Cancel out velocity parallel to the normal
				float dot = Vector3.Dot(Velocity, hit.normal);
				Vector3 normalProjection = dot * hit.normal;
				Velocity -= normalProjection;
			}
			else
			{
				IsGrounded = false;

			}
		}
		else
		{
			IsGrounded = false;
		}

		if (Velocity.sqrMagnitude > .001f)
		{
			movePosition();
		}

		currentDodgeFrames--;

		if (!GameManager.instance.introManager.RotateWorld)
		{
			if (Position.x <= leftBound.transform.position.x
				|| Position.x >= rightBound.transform.position.x)
			{
				Velocity = new Vector3(0, -10, 0);
				GameManager.instance.introManager.hitEdge();
			}
			Position.x = Mathf.Max(leftBound.transform.position.x, Mathf.Min(rightBound.transform.position.x, Position.x));
		}

		Up = new Vector3(Up.x, Up.y, 0);
		Position.z = 0;
	}

	private void movePosition()
	{
		float cachedVelocityMagnitude = Velocity.magnitude;
		float totalDistToTravel = Velocity.magnitude * Time.fixedDeltaTime;
		float distRemainingToTravel = totalDistToTravel;
		
		int maxIterations = 10;
		while (maxIterations > 0
			&& distRemainingToTravel > 0)
		{
			RaycastHit2D hit = Physics2D.Raycast(Position, Velocity.normalized, distRemainingToTravel, groundLayerMask);
			if (hit.collider != null && hit.distance == 0)
			{
				Debug.LogWarning("Collision with dist 0");
			}


			if (hit.collider != null)
			{
				distRemainingToTravel -= hit.distance;

				// Move up to the collision
				Position += Velocity.normalized * (hit.distance - HoverDist);

				surface = hit.collider;

				// Update 'up'
				//if (GameManager.instance.introManager.RotateWorld)
				{
					Up = hit.normal;
				}

				// Cancel out velocity parallel to the normal
				float dot = Vector3.Dot(Velocity, hit.normal);
				Vector3 normalProjection = dot * hit.normal;
				Velocity -= normalProjection;
				// Restore the velocity to what it was before (effectively just rotating the velocity to parallel the surface)
				Velocity = Velocity.normalized * cachedVelocityMagnitude;
			}
			else
			{
				// Move the whole way, unimpeded
				Position += Velocity.normalized * distRemainingToTravel;
				distRemainingToTravel = 0;
			}
		}

		if (maxIterations == 0)
		{
			Debug.LogWarning("Caught an infinite loop");
		}
	}

	public void SetVelocity(Vector3 velocity)
	{
		Velocity = velocity;
	}

	public void Move(int direction)
	{
		if (IsDodging)
		{
			return;
		}

		if (direction == lastDirectionHeld)
		{
			return;
		}

		lastDirectionHeld = direction;
		float speed = direction * MoveAcceleration;
		acceleration.x = speed;
	}

	public void Dodge(int direction)
	{
		if (IsDodging)
		{
			Debug.Log("Dodge ignored");
			return;
		}
		float speed = direction * DodgeSpeed;
		Velocity = Vector3.zero;
		Velocity.x = speed * Right.x;
		Velocity.y = speed * Right.y;
		currentDodgeFrames = MaxDodgeFrames;
	}

	public void Flip()
	{
		Vector3 newPos = transform.position + Up * -.3f;
		Position = newPos;
		transform.position = newPos;
		Up *= -1;
		acceleration.x *= -1;

		GameManager.instance.OnFlip();

	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(Position, .1f);
		GizmoUtil.GizmosDrawArrow(Position, Position + Velocity * .2f, Color.yellow);

		GizmoUtil.GizmosDrawArrow(Position, Position + Up, Color.white);
	}
}
