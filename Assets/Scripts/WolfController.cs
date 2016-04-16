using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WolfMovementController))]
public class WolfController : MonoBehaviour, ITickable {

	private Animator animator;
	private WolfMovementController movement;


	public float AttackDist = 20f;
	public float RecoilHitDist = 50f;
	public float HowlDist = 47f;
	public float SignalAttackDist = 52f;

	public WolfState State { get; private set; }
	private WolfStateData stateData;

	public int TicksLeftInState { get; private set; }

	private PlayerController player;

	public WolfStateData[] DataList = new WolfStateData[0];

	void Awake()
	{
		animator = GetComponent<Animator>();
		movement = GetComponent<WolfMovementController>();
	}

	void Start()
	{

		player = GameObject.FindObjectOfType<PlayerController>();
		setState(WolfState.Idle);
	}


	public void TickFrame()
	{
		movement.TickFrame();

		Vector3 up = (transform.position - movement.WorldCenter.position).normalized;
		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(up) + 180);
		transform.rotation = rotation;

		updateState();

	}

	private void setState(WolfState newState)
	{
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

			if (data.durationType == StateDurationType.Frames)
			{
				TicksLeftInState = (int)Random.Range(data.minDuration, data.maxDuration);
			}
			else if (data.durationType == StateDurationType.None)
			{
				Debug.LogWarning("Entered unexitable state " + data.state);
			}

			movement.AtGoal = false;
		}

		State = newState;
		stateData = data;
	}

	private void updateState()
	{
		if (stateData.anchorType == AnchorType.PlayerPosition)
		{
			movement.TargetAngleAnchor = player.transform.position;
		}

		if (TicksLeftInState > 0)
		{
			TicksLeftInState--;
			if (TicksLeftInState == 0)
			{
				advanceState();
			}
		}
		else if (stateData.durationType == StateDurationType.UntilTargetReached
			&& movement.AtGoal)
		{
			advanceState();
		}
	}

	private void advanceState()
	{
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
	UntilTargetReached
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
	None
}
