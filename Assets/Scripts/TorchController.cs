using UnityEngine;
using System.Collections;

public class TorchController : MonoBehaviour, ITickable {

	public bool IsHeld { get; private set; }

	public float GrabDist = 5;
	public float HeldDistOFfGround = .5f;
	public float ThrowVelMult = 1.5f;
	public float LanternLightDist = 2f;

	public bool WasThrown = false;
	public bool IsLit = true;

	private PlayerController player;

	private PlayerPhysicsController physics;

	public GameObject LightAura;

	public float ThrowSpeed = 20f;

	void Start()
	{
		player = GameObject.FindObjectOfType<PlayerController>();
		physics = GetComponent<PlayerPhysicsController>();
		IsHeld = true;
	}

	public void TickFrame()
	{
		if (IsHeld)
		{
			transform.position = player.transform.position + physics.Up * HeldDistOFfGround;
			transform.rotation = player.transform.rotation;
			physics.SetUp(player.Physics.Up);
			physics.Position = transform.position;
			physics.SetVelocity(Vector3.zero);
			physics.IsGrounded = false;
		}
		else
		{
			physics.TickFrame();
			transform.position = physics.Position;

			foreach (LanternController lantern in GameManager.instance.Lanterns)
			{
				if ((lantern.transform.position - transform.position).magnitude < LanternLightDist)
				{
					lantern.Flare();
					BecomeLit();
				}
			}

		}

		if (!IsHeld
			&& physics.IsGrounded
			&& (player.transform.position - transform.position).magnitude < GrabDist)
		{
			IsHeld = true;
		}

		if (physics.IsGrounded)
		{
			WasThrown = false;
		}
	}

	public void BecomeLit()
	{
		IsLit = true;
		LightAura.GetComponentInChildren<Renderer>().enabled = true;
	}

	public void Extinguish()
	{
		IsLit = false;

		LightAura.GetComponentInChildren<Renderer>().enabled = false;
	}

	public void Throw(Vector3 throwVel, bool voluntary)
	{
		throwVel *= ThrowVelMult;
		throwVel.x += physics.Up.x * ThrowSpeed;
		throwVel.y += physics.Up.y * ThrowSpeed;

		physics.SetVelocity(throwVel);

		IsHeld = false;
		WasThrown = voluntary;
	}

}
