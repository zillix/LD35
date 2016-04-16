using UnityEngine;
using System.Collections;

public enum Direction
{
	None,
	Left,
	Right
}

public enum Side
{
	None,
	Inside,
	Outside
}

[RequireComponent(typeof(PlayerPhysicsController))]
public class PlayerController : MonoBehaviour, ITickable {

	private InputController input = new InputController();
	private PlayerPhysicsController physics;
	private Animator animator;

	private Direction facing = Direction.Right;
	public Side Side { get; private set; }

	void Awake()
	{
		Side = Side.Inside;
		physics = GetComponent<PlayerPhysicsController>();
		animator = GetComponent<Animator>();
	}

	// Use this for initialization
	void Start () {

		GameManager.instance.player = this;
	}
	
	// Update is called once per frame
	public void TickFrame () {
		if (input.GetButtonDown(Button.Dodge))
		{
			physics.Dodge(facing == Direction.Left ? -1 : 1);
		}

		if (!physics.IsDodging)
		{
			if (physics.IsGrounded
				&& input.GetButtonDown(Button.Flip))
			{
				physics.Flip();
				facing = facing == Direction.Left ? Direction.Right : Direction.Left;
				Side = Side == Side.Inside ? Side.Outside : Side.Inside;
			}
			else
			{
				if (input.GetButton(Button.Left))
				{
					physics.Move(-1);
					facing = Direction.Left;
				}
				else if (input.GetButton(Button.Right))
				{
					physics.Move(1);
					facing = Direction.Right;
				}
				else
				{
					physics.Move(0);
				}
			}

			if (facing == Direction.Left)
			{
				transform.localScale = new Vector3(-1, 1, 1);
			}
			else
			{
				transform.localScale = new Vector3(1, 1, 1);
			}
		}

		physics.TickFrame();
		animator.SetBool("Dodging", physics.IsDodging);
		animator.SetFloat("Speed", Mathf.Abs(physics.Velocity.magnitude));

		transform.position = physics.Position;

		Vector3 up = physics.Up;
		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(up) - 90);
		transform.rotation = rotation;
	}
}
