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
	private float cameraAngle = 0;
	public float InsideOrtho = 60f;
	public float OutsideOrtho = 120f;
	public float AssessOrtho = 160f;

	private Camera mainCamera;

	// Use this for initialization
	void Start () {
		player = GameManager.instance.player;
		transform.position = calculateTargetPosition();
		mainCamera = GetComponent<Camera>();
		mainCamera.orthographicSize = InsideOrtho;
	}

	// Update is called once per frame
	public void TickFrame()
	{
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

		float targetOrthographicSize = player.IsInside ? InsideOrtho : OutsideOrtho;

		if (GameManager.instance.wolf.State == WolfState.Assess)
		{
			targetOrthographicSize = AssessOrtho;
		}
		if (Mathf.Abs(mainCamera.orthographicSize - targetOrthographicSize) < Time.fixedDeltaTime * ZoomSpeed)
		{
			mainCamera.orthographicSize = targetOrthographicSize;
		}
		else
		{
			if (targetOrthographicSize > mainCamera.orthographicSize)
			{
				mainCamera.orthographicSize += Time.fixedDeltaTime * ZoomSpeed;
			}
			else
			{
				mainCamera.orthographicSize -= Time.fixedDeltaTime * ZoomSpeed;
			}
		}

		transform.position = newPos;


		if (GameManager.instance.introManager.RotateWorld)
		{
			float offset = 270f;
			if (!GameManager.instance.introManager.InvertInteriorCamera)
			{
				offset = 90f;
			}
			float targetAngle = MathUtil.VectorToAngle(player.transform.position) + offset;
			if (player.IsInside)
			{
				//targetAngle += 180;
			}
			cameraAngle = MathUtil.RotateAngle(cameraAngle, targetAngle, lerpSpeed * Time.fixedDeltaTime);

			transform.rotation = Quaternion.Euler(0, 0, cameraAngle);
		}
	}

	private Vector3 calculateTargetPosition()
	{
		Vector3 targetPosition = player.transform.position;

		Vector2 offset = player.IsInside ? InsideOffset : OutsideOffset;
		targetPosition.x += player.RotationUp.x * offset.y;

		targetPosition.y += player.RotationUp.y * offset.y;

		return targetPosition;
	}
}
