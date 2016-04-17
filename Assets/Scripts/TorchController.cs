using UnityEngine;
using System.Collections;

public class TorchController : MonoBehaviour, ITickable {

	public bool IsHeld { get; private set; }

	public float GrabDist = 5;
	public float HeldDistOFfGround = .5f;
	public float HeldDistForward = .5f;
	public float ThrowVelMult = 1.5f;
	public float LanternLightDist = 2f;

	public bool WasThrown = false;
	public bool IsLit = true;

	private GameEmitter emitter;

	private PlayerController player;

	public PlayerPhysicsController Physics;

	public GameObject LightAura;

	public int LightAuraPixels = 64;
	public int PPU = 16;
	public float LightAuraSize = 8f;

	public float ThrowSpeed = 20f;

	public float AuraDelta = 1f;
	public float AuraAngleSpeed = 180f;
	private float currentAuraAngle = 0f;

	public GameObject ExplosionPrefab;

	public float WolfHitShakeMagnitude = 5f;
	public int WolfHitShakeDurationFrames = 30;

	public SpriteRenderer spriteRender;
	public SoundBank sounds;

	void Start()
	{
		player = GameObject.FindObjectOfType<PlayerController>();
		spriteRender = GetComponent<SpriteRenderer>();
		Physics = GetComponent<PlayerPhysicsController>();
		IsHeld = true;
		emitter = GetComponent<GameEmitter>();

		Vector3 startVec = player.transform.position + Physics.Up * HeldDistOFfGround;
		startVec.z = 0;
		transform.position = startVec;

		sounds = GameObject.Find("SoundBank").GetComponent<SoundBank>();

	}

	public void TickFrame()
	{
		/*if (!GameManager.instance.introManager.PlayerControls)
		{
			LightAura.GetComponentInChildren<Renderer>().enabled = false;
			return;
		}
		else if (!GameManager.instance.introManager.BattleStarted)
		{
			LightAura.GetComponentInChildren<Renderer>().enabled = true;
		}*/

		if (player.IsInside)
		{ LightAura.GetComponentInChildren<Renderer>().enabled = true;
			spriteRender.enabled = true;
		}
		else
		{
			LightAura.GetComponentInChildren<Renderer>().enabled = false;
			spriteRender.enabled = false;
		}

		emitter.EmitActive = !IsHeld && Physics.IsGrounded;

		if (IsHeld)
		{
			transform.position = player.transform.position + Physics.Up * HeldDistOFfGround;
			transform.position += ((player.facing == Direction.Right) ? Physics.Right : -Physics.Right) * HeldDistForward;
			transform.rotation = player.transform.rotation;
			Physics.SetUp(player.Physics.Up);
			Physics.Position = transform.position;
			Physics.SetVelocity(Vector3.zero);
			Physics.IsGrounded = false;
		}
		else
		{
			if (!GameManager.instance.introManager.RotateWorld)
			{
				Physics.SetUp(Vector2.up);

			}

			bool wasGrounded = Physics.IsGrounded;
			Physics.TickFrame();

			if (!wasGrounded && Physics.IsGrounded)
			{
				sounds.player.PlayOneShot(sounds.torchLand);
			}

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


		currentAuraAngle += Time.fixedDeltaTime * AuraAngleSpeed;
		float auraSize = 0;
		if (IsLit)
		{
			auraSize = LightAuraSize;
			auraSize += Mathf.Cos(Mathf.Deg2Rad * currentAuraAngle) * AuraDelta;
		}

		float startSize = (float)LightAuraPixels / PPU;
		float scale = auraSize / startSize;
		Vector3 lightScale = new Vector3(scale, scale, scale);
		LightAura.transform.localScale = lightScale;

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

	public void OnHitWolf()
	{
		//IsLit = false;

		//LightAura.GetComponentInChildren<Renderer>().enabled = false;

		GameObject explosion = Instantiate(ExplosionPrefab);
		explosion.transform.position = transform.position;

		GameManager.instance.mainCamera.BeginCameraShake(WolfHitShakeDurationFrames, WolfHitShakeMagnitude);
	}

	public void Throw(Vector3 throwVel, bool voluntary)
	{
		if (GameManager.instance.introManager.RotateWorld)
		{

			throwVel *= ThrowVelMult;
			throwVel.x += Physics.Up.x * ThrowSpeed;
			throwVel.y += Physics.Up.y * ThrowSpeed;
		}
		else
		{
			throwVel.x = 0;
			throwVel.y = ThrowSpeed;
		}



		Physics.SetVelocity(throwVel);

		IsHeld = false;
		WasThrown = voluntary;
	}

}
