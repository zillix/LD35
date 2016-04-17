using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WolfMovementController))]
public class WolfController : MonoBehaviour, ITickable {

	public Animator animator;
	private WolfMovementController movement;

	public bool IsHiding = false;

	public int TotalHits = 3;
	public int HitsRemaining
	{
		get; private set;
	}


	public float AttackDist = 20f;
	public float RecoilHitDist = 50f;
	public float HowlDist = 47f;
	public float SignalAttackDist = 52f;

	public WolfState State { get { return stateData.state; } }
	private WolfStateData stateData;

	public int TicksLeftInState { get; private set; }

	private PlayerController player;

	public WolfStateData[] DataList = new WolfStateData[0];

	private bool enraged = false;

	public Material EnragedNoseMaterial;
	private Material initialNoseMaterial;
	public GameObject Nose;
	private Renderer noseRenderer;

	public GameEmitter EyeEmitter;
	public GameEmitter NoseEmitter;



	void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		animator.applyRootMotion = false;
		movement = GetComponent<WolfMovementController>();
		HitsRemaining = TotalHits;

	}

	void Start()
	{

		player = GameObject.FindObjectOfType<PlayerController>();
		setState(WolfState.Idle);
		noseRenderer = Nose.GetComponent<Renderer>();
		initialNoseMaterial = noseRenderer.material;
	}
	public void Update()
	{
		if (Input.GetButtonDown("DebugToggle"))
		{
			enraged = !enraged;
		}

		animator.SetBool("IsHiding", GameManager.instance.introManager.WolfHiding);
	}


	public void TickFrame()
	{
		EyeEmitter.EmitActive = enraged && !movement.AtGoal;
		NoseEmitter.EmitActive = enraged && !movement.AtGoal;

		movement.TickFrame();

		Vector3 up = (transform.position - movement.WorldCenter.position).normalized;
		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(up) + 180);
		transform.rotation = rotation;

		updateState();

		if (enraged)
		{
			noseRenderer.material = EnragedNoseMaterial;
		}
		else
		{
			noseRenderer.material = initialNoseMaterial;
		}

		animator.SetBool("Enraged", enraged);

	}

	private void setState(WolfState newState)
	{
		if (stateData != null)
		{
			Debug.Log("Entering state " + newState + " from " + stateData.state);

			if (stateData.state == WolfState.Howl)
			{
				if (HitsRemaining == 0)
				{
					newState = WolfState.Flee;
				}
			}
		}

		WolfStateData data = getData(newState);
		if (data != null)
		{
			Vector3 anchor = transform.position;
			switch (data.anchorType)
			{
				case AnchorType.PlayerPosition:
					anchor = player.transform.position;
					break;

				case AnchorType.Random:
					anchor = MathUtil.AngleToVector(Random.Range(0, 360));
					break;

				default:
				case AnchorType.CurrentPosition:
					anchor = transform.position;
					break;
			}


			movement.TargetRadius = data.radius;
			movement.TargetAngleAnchor = anchor;
			movement.AngleSpeed = data.angleSpeed;
			movement.RadiusSpeed = data.radiusSpeed;

			if (data.durationType == StateDurationType.Frames
				|| data.durationType == StateDurationType.FramesOrTarget)
			{
				TicksLeftInState = (int)Random.Range(data.minDuration, data.maxDuration);
			}
			else if (data.durationType == StateDurationType.None)
			{
				Debug.LogWarning("Entered unexitable state " + data.state);
			}

			movement.AtGoal = false;





			animator.SetInteger("State", getAnimState(data.state));
		}

		stateData = data;
	}

	private void updateState()
	{
		if (stateData.anchorType == AnchorType.PlayerPosition)
		{
			if (player.IsOutside)
			{
				movement.TargetAngleAnchor = player.transform.position;
			}
			else
			{
				setState(WolfState.Idle);
			}
		}

		if ((stateData.durationType == StateDurationType.UntilTargetReached
		   || stateData.durationType == StateDurationType.FramesOrTarget)
		   && movement.AtGoal)
		{
			advanceState();
		}
		else if (TicksLeftInState > 0)
		{
			TicksLeftInState--;
			if (TicksLeftInState == 0)
			{
				advanceState();
			}
		}
	}

	private void advanceState()
	{
		if (stateData.state == WolfState.Flee)
		{
			GameManager.instance.OnWolfFleed();
			return;
		}

		float weightSum = 0;

		if (stateData.nextStates.Length == 1)
		{
			setState(stateData.nextStates[0].state);
			return;
		}

		for (int i = 0; i < stateData.nextStates.Length; ++i)
		{
			weightSum += stateData.nextStates[i].weight;
		}

		float rolled = Random.Range(0, weightSum);
		float checkSum = 0;
		WolfState nextState = WolfState.None;

		for (int i = 0; i < stateData.nextStates.Length; ++i)
		{
			float weight = stateData.nextStates[i].weight;
			if (rolled < checkSum + weight)
			{
				nextState = stateData.nextStates[i].state;
				break;
			}
			else
			{
				checkSum += weight;
			}
		}

		if (nextState != WolfState.None)
		{
			setState(nextState);
        }
		else
		{
			Debug.Log("Failed to advance state from " + State);
		}

	}

	private WolfStateData getData(WolfState state)
	{
		foreach (WolfStateData data in DataList)
		{
			if (data.state == state)
			{
				return data;
			}
		}

		return null;
	}

	private int getAnimState(WolfState state)
	{
		switch (state)
		{
			case WolfState.Idle:
				return 0;
			case WolfState.Pursue:
				return 1;
			case WolfState.SignalAttack:
				return 2;
			case WolfState.AttackApproach:
				return 3;
			case WolfState.Bite:
				return 4;
			case WolfState.RecoilMiss:
				return 5;
			case WolfState.RecoilHit:
				return 6;
			case WolfState.Howl:
				return 7;
			case WolfState.Flee:
				return 8;
			case WolfState.Hiding:
				return 9;

			case WolfState.Assess:
				return 10;

		}

		return 0;
    }

	public void ReceiveDamage()
	{


		setState(WolfState.RecoilHit);
		enraged = true;
		HitsRemaining--;
	}

	public void GetBopped()
	{
		enraged = false;
	}
}

[System.Serializable]
public class WolfStateData
{
	public WolfState state;
	public float radius;
	public AnchorType anchorType;
	public float angleSpeed;
	public float radiusSpeed;
	public StateDurationType durationType;
	public int minDuration;
	public int maxDuration;
	public int oscillationMagnitude;

	public NextStateData[] nextStates;
}

[System.Serializable]
public class NextStateData
{
	public WolfState state;
	public float weight;
}

public enum AnchorType
{
	None,
	CurrentPosition,
	PlayerPosition,
	Random
}

public enum StateDurationType
{
	None,
	Frames,
	UntilTargetReached,
	FramesOrTarget
}

public enum WolfState
{
	Idle,
	Pursue,
	SignalAttack,
	AttackApproach,
	Bite,
	RecoilMiss,
	RecoilHit,
	Howl,
	Flee,
	None,
	Hiding,
	Appear,
	Assess
}
