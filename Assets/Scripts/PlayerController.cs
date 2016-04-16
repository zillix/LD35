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
	public PlayerPhysicsController Physics { get; private set; }
	private Animator animator;

	private Direction facing = Direction.Right;
	public Side Side { get; private set; }
	public bool IsInside {  get { return Side == Side.Inside; } }
	public bool IsOutside {  get { return Side == Side.Outside; } }

	void Awake()
	{
		Side = Side.Inside;
		Physics = GetComponent<PlayerPhysicsController>();
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
			Physics.Dodge(facing == Direction.Left ? -1 : 1);
		}

		if (!Physics.IsDodging)
		{
			if (Physics.IsGrounded
				&& input.GetButtonDown(Button.Flip))
			{
				Physics.Flip();
				facing = facing == Direction.Left ? Direction.Right : Direction.Left;
				Side = IsInside ? Side.Outside : Side.Inside;
			}
			else
			{
				if (input.GetButton(Button.Left))
				{
					Physics.Move(IsInside ? 1 : -1);
					facing = IsInside ? Direction.Right : Direction.Left;
				}
				else if (input.GetButton(Button.Right))
				{
					Physics.Move(IsInside ? -1 : 1);
					facing = IsInside ? Direction.Left : Direction.Right;
				}
				else
				{
					Physics.Move(0);
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

		Physics.TickFrame();
		animator.SetBool("Dodging", Physics.IsDodging);
		animator.SetFloat("Speed", Mathf.Abs(Physics.Velocity.magnitude));

		transform.position = Physics.Position;

		Vector3 up = Physics.Up;
		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(up) - 90);
		transform.rotation = rotation;
	}
}
