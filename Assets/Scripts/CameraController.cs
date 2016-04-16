using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour, ITickable {

	public float speed;
	public float deadDist = 2f;
	private PlayerController player;

	public Vector2 InsideOffset = new Vector2(0, 3);
	public Vector2 OutsideOffset = new Vector2(0, -10);
	public float InsideDist = -10f;
	public float OutsideDist = -20f;
	public float ZoomSpeed = 12f;

	public float lerpSpeed = 1f;

	private Camera mainCamera;

	// Use this for initialization
	void Start () {
		player = GameManager.instance.player;
		transform.position = calculateTargetPosition();
		mainCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	public void TickFrame () {
		int cullingMask = -1;

		if (player.IsInside)
		{
			cullingMask &= ~LayerUtil.GetLayerMask(LayerUtil.MOON);
			cullingMask &= ~LayerUtil.GetLayerMask(LayerUtil.WOLF);
		}
		else
		{
			cullingMask &= ~LayerUtil.GetLayerMask(LayerUtil.FOREST_GROUND);
			cullingMask &= ~LayerUtil.GetLayerMask(LayerUtil.FOREST_BACKGROUND);
		}

		mainCamera.cullingMask = cullingMask;

		float cacheZ = transform.position.z;
		Vector3 targetPosition = calculateTargetPosition();

		Vector2 dist = (Vector2)(targetPosition - transform.position);
		if (dist.magnitude > deadDist)
		{

			if (dist.magnitude - deadDist < speed * Time.fixedDeltaTime)
			{
				// Set exactly to dead dist
				transform.position = (Vector2)targetPosition - dist.normalized * deadDist;
			}
			else
			{
				transform.position += (Vector3)(speed * Time.fixedDeltaTime * dist.normalized);
			}

			dist = targetPosition - transform.position;

			Vector3 transPos = transform.position;
			transPos.z = cacheZ;
			transform.position = transPos;
		}

		float targetDist = player.IsInside ? InsideDist : OutsideDist;
		Vector3 newPos = transform.position;
		if (Mathf.Abs(transform.position.z - targetDist) < Time.fixedDeltaTime * ZoomSpeed)
		{
			newPos.z = targetDist;
		}
		else
		{
			if (targetDist > transform.position.z)
			{
				newPos.z += Time.fixedDeltaTime * ZoomSpeed;
			}
			else
			{
				newPos.z -= Time.fixedDeltaTime * ZoomSpeed;
			}
		}
		transform.position = newPos;

		Quaternion targetRotation = player.transform.rotation;
		if (player.Side== Side.Inside)
		{
			Vector3 eulers = targetRotation.eulerAngles;
			eulers.z += 180;
			targetRotation.eulerAngles = eulers;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed);
	}

	private Vector3 calculateTargetPosition()
	{
		Vector3 targetPosition = player.transform.position;

		Vector2 offset = player.IsInside ? InsideOffset : OutsideOffset;
		targetPosition.x += player.Physics.Up.x * offset.y;

		targetPosition.y += player.Physics.Up.y * offset.y;

		return targetPosition;
	}
}
