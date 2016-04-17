using UnityEngine;
using System.Collections;

public class GameEmitter : MonoBehaviour {


	public float EmitFrequency = .2f;
	public bool EmitActive = false;
	public GameObject prefab;
	public GameObject source;

	private GameObject dynamicObjects;

	// Use this for initialization
	void Start () {


		dynamicObjects = GameObject.Find("DynamicObjects");
		InvokeRepeating("Emit", EmitFrequency, EmitFrequency);
	}



	void Emit()
	{
		if (EmitActive)
		{
			GameObject blur = Instantiate(prefab);
			blur.transform.position = source.transform.position;
			blur.transform.rotation = source.transform.rotation;
			blur.transform.SetParent(dynamicObjects.transform, true);
		}
	}
}
