using UnityEngine;
using System.Collections;

public class WolfTongue : MonoBehaviour {

	private PolygonCollider2D tongue;
	private WolfController wolf;

	void Awake()
	{
		tongue = GetComponent<PolygonCollider2D>();
	}

	// Use this for initialization
	void Start () {
		wolf = FindObjectOfType<WolfController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		TorchController torch = other.gameObject.GetComponent<TorchController>();
        if (torch != null && !torch.IsHeld)
		{
			wolf.ReceiveDamage();
		}
	}
}
