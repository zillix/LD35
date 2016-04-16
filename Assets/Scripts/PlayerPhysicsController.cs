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
	public float Gravity = -20f;
	public float HoverDist = .05f;
	public float RaycastDist = .06f;
	public int MaxDodgeFrames = 10;
	
	public bool IsGrounded
	{
		get; private set;
	}

	public Vector3 Up { get; private set; }
	public Vector3 Right {  get { return new Vector3(Up.y, -Up.x, -Up.z); } }

	private Collider2D surface = null;
	private int currentDodgeFrames = 0;

	private int lastDirectionHeld = 0;

	int groundLayerMask;

	public bool IsDodging {  get { return currentDodgeFrames > 0; } }

	void Awake()
	{
		Velocity = Vector3.zero;
		Up = new Vector3(0, 1, 0);
		Position = transform.position;
		int groundLayer = LayerMask.NameToLayer("Ground");
		groundLayerMask = 1 << groundLayer;
	}

	public void TickFrame()
	{
		Velocity.x += Gravity * Up.x * Time.fixedDeltaTime;
		Velocity.y += Gravity * Up.y * Time.fixedDeltaTime;

		Velocity.x += acceleration.x * Right.x * Time.fixedDeltaTime;
		Velocity.y += acceleration.x * Right.y * Time.fixedDeltaTime;

		float rightDot = Vector3.Dot(Velocity, Right);

		// Cap velocity at max speed
		float maxSpeedX = IsDodging ? DodgeSpeed : MaxSpeed.x;
		if (rightDot >maxSpeedX)
		{
			float upDot = Vector3.Dot(Velocity, Up);
			Velocity = upDot * Up + (Vector3)(maxSpeedX * Right);
		}
		else if (rightDot < -maxSpeedX)
		{
			float upDot = Vector3.Dot(Velocity, Up);
			Velocity = upDot * Up + (Vector3) (-maxSpeedX * Right);
		}

		// Recalculate velocity
		rightDot = Vector3.Dot(Velocity, Right);

		// Lose velocity to friction
		if (!IsDodging
			&& (lastDirectionHeld == 0))
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

		RaycastHit2D hit = Physics2D.Raycast(Position, Up * -1, RaycastDist, groundLayerMask);
		if (hit.collider != null)
		{
			if (hit.distance == 0)
			{
				Debug.LogWarning("Collision with dist 0");
			}
			surface = hit.collider;
			Up = hit.normal;

			IsGrounded = true;

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

		if (Velocity.sqrMagnitude > .001f)
		{
			movePosition();
		}

		currentDodgeFrames--;
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
				Up = hit.normal;

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

	public void Move(int direction)
	{
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

	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(Position, .1f);
		GizmoUtil.GizmosDrawArrow(Position, Position + Velocity * .2f, Color.yellow);

		GizmoUtil.GizmosDrawArrow(Position, Position + Up, Color.white);
	}
}
