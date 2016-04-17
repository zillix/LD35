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

	private GameEmitter emitter;

	private PlayerController player;

	public PlayerPhysicsController Physics;

	public GameObject LightAura;

	public float ThrowSpeed = 20f;

	void Start()
	{
		player = GameObject.FindObjectOfType<PlayerController>();
		Physics = GetComponent<PlayerPhysicsController>();
		IsHeld = true;
		emitter = GetComponent<GameEmitter>();
	}

	public void TickFrame()
	{
		emitter.EmitActive = !IsLit;

		if (IsHeld)
		{
			transform.position = player.transform.position + Physics.Up * HeldDistOFfGround;
			transform.rotation = player.transform.rotation;
			Physics.SetUp(player.Physics.Up);
			Physics.Position = transform.position;
			Physics.SetVelocity(Vector3.zero);
			Physics.IsGrounded = false;
		}
		else
		{
			Physics.TickFrame();
			transform.position = Physics.Position;

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
			&& Physics.IsGrounded
			&& (player.transform.position - transform.position).magnitude < GrabDist)
		{
			IsHeld = true;
		}

		if (Physics.IsGrounded)
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
		throwVel.x += Physics.Up.x * ThrowSpeed;
		throwVel.y += Physics.Up.y * ThrowSpeed;

		Physics.SetVelocity(throwVel);

		IsHeld = false;
		WasThrown = voluntary;
	}

}
