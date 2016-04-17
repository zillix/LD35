using UnityEngine;
using System.Collections;

public class LanternController : MonoBehaviour, ITickable {

	public int MaxFlareFrames = 60;
	public GameObject LightAura;
	public float SmallLightAura = 3f;
	public float FlareLightAura = 6f;
	public int LightAuraPixels = 64;
	public int PPU = 16;

	private Animator animator;
	
	private int flareFramesRemaining = 10;

	public float AuraDelta = 1f;
	public float AuraAngleSpeed = 180f;
	private float currentAuraAngle = 0f;

	public bool IsLit { get; private set; }
	public bool IsFlaring {  get { return flareFramesRemaining > 0; } }


	private SoundBank sounds;
	void Awake()
	{
		IsLit = false;
		animator = GetComponentInChildren<Animator>();
		sounds = GameObject.Find("SoundBank").GetComponent<SoundBank>();
	}

	void Start()
	{

		LightAura.transform.localScale = Vector3.zero;
	}

	public void TickFrame () {

		animator.SetBool("Lit", IsLit);

		Vector3 up = GameManager.instance.Up;
		if (!GameManager.instance.introManager.RotateWorld)
		{
			up = Vector2.up;
		}

		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(up) - 90);
		transform.rotation = rotation;

		flareFramesRemaining--;

		currentAuraAngle += Time.fixedDeltaTime * AuraAngleSpeed;


		float auraSize = 0;
		if (IsLit)
		{
			auraSize = IsFlaring ? FlareLightAura : SmallLightAura;
			auraSize += Mathf.Cos(Mathf.Deg2Rad * currentAuraAngle) * AuraDelta;
		}

		float startSize =(float)LightAuraPixels / PPU;
		float scale = auraSize / startSize;
		Vector3 lightScale = new Vector3(scale, scale, scale);
		LightAura.transform.localScale = lightScale;
	}

	public void Flare()
	{
		if (!IsLit)
		{
			GameManager.instance.introManager.OnTorchLit();
			sounds.player.PlayOneShot(sounds.lightLantern);
		}
		IsLit = true;
		flareFramesRemaining = MaxFlareFrames;
	}

	public void Extinguish()
	{
		Destroy(gameObject);
	}
}
