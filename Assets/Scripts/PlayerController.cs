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

	public TorchController Torch;
	private Renderer myrenderer;

	private SoundBank sounds;

	public Direction facing = Direction.Right;
	public Side Side { get; private set; }
	public bool IsInside {  get { return Side == Side.Inside; } }
	public bool IsOutside {  get { return Side == Side.Outside; } }

	public bool IsTorchHeld {  get { return Torch.IsHeld;  } }

	public float BlinkFrequency = .3f;
	public int MaxBlinks = 5;
	private int blinksLeft = 0;

	public Vector3 KnockBackAmount = new Vector3(-5f, 10f);
	public Vector3 BopKnockBackAmount = new Vector3(-5f, -10f);

	public bool IsInvincible {  get { return blinksLeft > 0; } }

	public float ScaleSize = 2f;

	public int PlayerHitShakeDurationFrames = 20;
	public float PlayerHitShakeMagnitude = 3f;

	public int WolfBopShakeDurationFrames = 20;
	public float WolfBopShakeMagnitude = 3f;

	void Awake()
	{
		Side = Side.Inside;
		Physics = GetComponent<PlayerPhysicsController>();
		animator = GetComponent<Animator>();
		myrenderer = GetComponentInChildren(typeof(Renderer)) as Renderer;
		sounds = GameObject.Find("SoundBank").GetComponent<SoundBank>();
	}

	// Use this for initialization
	void Start () {

		GameManager.instance.player = this;
	}

	void Update()
	{
		if (!GameManager.instance.introManager.PlayerControls)
		{
			return;
		}

		if (input.GetButtonDown(Button.Dodge) && Physics.IsGrounded)
		{
			Debug.Log("Dodge pressed");
			Physics.Dodge(facing == Direction.Left ? -1 : 1);
			sounds.player.PlayOneShot(sounds.dash);
		}



		if (IsInside
			&& IsTorchHeld
			&& input.GetButtonDown(Button.Up))
		{
			throwTorch(true);
		}

		if (!Physics.IsDodging
			&& Physics.IsGrounded
				&& input.GetButtonDown(Button.Flip)
				&& GameManager.instance.introManager.AllTorchesLit)
		{
			Physics.Flip();
			sounds.player.PlayOneShot(sounds.flipScreen);
			facing = facing == Direction.Left ? Direction.Right : Direction.Left;
			Side = IsInside ? Side.Outside : Side.Inside;
		}
    }
	
	// Update is called once per frame
	public void TickFrame () {
		
		if (!GameManager.instance.introManager.PlayerControls)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
			Torch.TickFrame();
			return;
		}


		if (!Physics.IsDodging)
		{
			if (!IsInvincible || Physics.IsGrounded)
			{
				bool invertControls = IsInside && GameManager.instance.introManager.BattleStarted;

				if (input.GetButton(Button.Left))
				{
					Physics.Move(invertControls ? 1 : -1);
					facing = invertControls ? Direction.Right : Direction.Left;
				}
				else if (input.GetButton(Button.Right))
				{
					Physics.Move(invertControls ? -1 : 1);
					facing = invertControls ? Direction.Left : Direction.Right;
				}
				else
				{
					Physics.Move(0);
				}
			}
			else
			{
				Physics.Move(0);
			}

			if (facing == Direction.Left)
			{
				transform.localScale = new Vector3(-ScaleSize, ScaleSize, ScaleSize);
			}
			else
			{
				transform.localScale = new Vector3(ScaleSize, ScaleSize, ScaleSize);
			}
		}
		//Physics.DisableGravity = IsInside;

		bool wasGrounded = Physics.IsGrounded;
		Physics.TickFrame();

	



		animator.SetBool("Dodging", Physics.IsDodging);
		animator.SetFloat("Speed", Mathf.Abs(Physics.Velocity.magnitude));
		animator.SetBool("Fox",IsOutside);

		transform.position = Physics.Position;

		if (GameManager.instance.introManager.RotateWorld)
		{
			if (!Physics.IsGrounded && IsOutside)
			{
				// This is garbage
				// basically says don't go flying off
				Physics.SetUp(transform.position.normalized);
			}


			Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(RotationUp) - 90);
			transform.rotation = rotation;

		}

		Torch.TickFrame();
	}

	public Vector3 RotationUp
	{
		get
		{
			Vector3 up = transform.position.normalized;
			if (IsInside)
			{
				up *= -1;
			}
			return up;
		}
	}

	private void throwTorch(bool voluntary)
	{
		Torch.Throw(Physics.Velocity, voluntary);
	}

	/*public void OnCollisionEnter2D(Collision2D collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Tongue"))
		{
			ReceiveHit();
		}

	}

	public void OnCollisionStay2D(Collision2D collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Tongue"))
		{
			ReceiveHit();
		}
	}*/

	public void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Teeth")
			||
			collider.gameObject.layer == LayerMask.NameToLayer("Wolf") ||
			collider.gameObject.layer == LayerMask.NameToLayer("TongueTouch"))
		{
			ReceiveHit();
		}

		if (collider.gameObject.layer == LayerMask.NameToLayer("Nose"))
		{
			BopNose();
		}

    }

	public void OnTriggerStay2D(Collider2D collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Teeth")
			||
			collider.gameObject.layer == LayerMask.NameToLayer("Wolf") ||
			collider.gameObject.layer == LayerMask.NameToLayer("TongueTouch"))
		{
			ReceiveHit();
		}

		if (collider.gameObject.layer == LayerMask.NameToLayer("Nose"))
		{
			BopNose();
        }
	}

	private void BopNose()
	{
		if (IsInvincible)
		{
			return;
		}

		
		// Knock back
		Vector3 knockBack = Vector3.zero;

		float knockbackAmountX = BopKnockBackAmount.x;

		// Always hit away from wolf
		Vector3 towardsWolf = GameManager.instance.wolf.transform.position - transform.position;
		if (Vector3.Dot(Physics.Right, towardsWolf) > 0)
		{
			knockbackAmountX *= -1;
		}

		// right component
		knockBack.x += knockbackAmountX * Physics.Right.x;
		knockBack.y += knockbackAmountX * Physics.Right.y;

		knockBack.x += BopKnockBackAmount.y * Physics.Up.x;
		knockBack.y += BopKnockBackAmount.y * Physics.Up.y;

		facing = facing == Direction.Left ? Direction.Right : Direction.Left;

		Physics.SetVelocity(knockBack);
		Physics.IsGrounded = false;

		if (GameManager.instance.wolf.enraged)
		{
			sounds.player.PlayOneShot(sounds.bopNose);

			GameManager.instance.wolf.GetBopped();
			GameManager.instance.introManager.OnWolfBopped();

			GameManager.instance.mainCamera.BeginCameraShake(WolfBopShakeDurationFrames, WolfBopShakeMagnitude);
		}
	}

	private void ReceiveHit()
	{
		if (IsInvincible)
		{
			return;
		}

		sounds.player.PlayOneShot(sounds.getHit);

		GameManager.instance.mainCamera.BeginCameraShake(PlayerHitShakeDurationFrames, PlayerHitShakeMagnitude);


		if (IsOutside)
		{
			GameManager.instance.introManager.OnOutsideHit();
		}
		else
		{
			GameManager.instance.introManager.OnInsideHit();
		}

		if (IsTorchHeld && IsInside)
		{
			throwTorch(false);
			//Torch.Extinguish();
		}

		// Knock back
		Vector3 knockBack = Vector3.zero;

		float knockbackAmountX = KnockBackAmount.x;

		// Always hit away from wolf
		Vector3 towardsWolf = GameManager.instance.wolf.transform.position - transform.position;
		if (Vector3.Dot(Physics.Right, towardsWolf) < -.1f)
		{
			knockbackAmountX *= -1;
		}

		// right component
		knockBack.x += knockbackAmountX * Physics.Right.x;
		knockBack.y += knockbackAmountX * Physics.Right.y;
		// up

		if (Physics.IsGrounded)
		{
			knockBack.x += KnockBackAmount.y * Physics.Up.x;
			knockBack.y += KnockBackAmount.y * Physics.Up.y;
		}

		if (GameManager.instance.introManager.WolfHiding)
		{
			knockBack /= 2f;
			GameManager.instance.introManager.OnWolfTouch();
		}

		Physics.SetVelocity(knockBack);
		Physics.IsGrounded = false;
		Physics.UncapSpeeds = true;

		facing = facing == Direction.Left ? Direction.Right : Direction.Left;

		blinksLeft = MaxBlinks;
		InvokeRepeating("Blink", BlinkFrequency, BlinkFrequency);
	}


	private void Blink()
	{
		myrenderer.enabled = !myrenderer.enabled;
		if (--blinksLeft == 0)
		{
			CancelInvoke("Blink");
			myrenderer.enabled = true;
			Physics.UncapSpeeds = false;
		}
	}
}
