using UnityEngine;
using System.Collections;

public class SoundBank : MonoBehaviour
{

	public AudioSource player;

	public AudioClip textBubble;
	public AudioClip torchLand;
	public AudioClip dash;
	public AudioClip awakenWolf;
	public AudioClip damageWolf;
	public AudioClip getHit;
	public AudioClip bopNose;
	public AudioClip killWolf;
	public AudioClip lightLantern;
	public AudioClip flipScreen;

	// Use this for initialization
	void Start()
	{
		player = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
