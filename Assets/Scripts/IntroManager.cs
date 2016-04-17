﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour {

	private TextManager text;

	public bool IntroStarted = false;
	public bool IntroCompleted = false;

	public int torchesLit = 0;

	public bool BattleStarted = false;
	public bool RotateWorld = false;

	public bool AllTorchesLit = false;

	private PlayerController player;
	private GameObject cottage;

	public float CottageDist = 1f;

	public bool OutForLamp = false;
	public bool PlayerControls = false;
	public bool InvertInteriorCamera = false;
	public bool WolfHiding { get; private set; }

	private bool reportedThrowUp = false;

	private int torchCountReported = 0;

	public bool gameEnded = false;
	public bool wolfFled = false;

	public GameObject blackScreen;

	public Text title;
	public Text zillix;

	public void Awake()
	{
		WolfHiding = true;

	}

	public void Init()
	{
		text = GameManager.instance.TextManager;
		player = GameManager.instance.player;
		cottage = GameObject.Find("Cottage");
	}

	public void StartIntro()
	{
		IntroStarted = true;

		StartCoroutine(startGame());
	}

	private IEnumerator startGame()
	{
		yield return new WaitForSeconds(3f);

		firstText();
	}

	public void firstText()
	{
		text.enqueue("tonight is a special night");
		text.enqueue("let me tell you the tale of the wolf and the moon", 4f);



		text.enqueue("once, on a night of the full moon");
		text.enqueue("wolf saw the moon in the sky, and decided:");
		text.enqueue("\"I shall eat the moon\"", 4);

		text.enqueue("when fox heard this, he was upset");
		text.enqueue("for fox loved to hunt by moonlight");
		text.enqueue("so fox decided to play a trick on wolf", 4);


		text.enqueue("it's time to light the first lamp, liza");

		PlayText.Callback sendForTorch = delegate () { OutForLamp = true; PlayerControls = true; };
		text.enqueue("here, take this torch", -1, sendForTorch);
	}

	void Update()
	{
		if ((OutForLamp && torchesLit > torchCountReported && !AllTorchesLit)
				|| (!gameEnded && wolfFled))
		{
			if ((player.transform.position - cottage.transform.position).magnitude < CottageDist)
			{
				OutForLamp = false;
				PlayerControls = false;
				torchCountReported = torchesLit;

				OnReturnHome();
			}
		}

		if (IntroStarted)
		{
			Color color = title.color;
			color.a -= Time.deltaTime;
			title.color = color;
			zillix.color = color;
		}
	}

	public void OnTorchLit()
	{
		torchesLit++;

		if (torchesLit == 1)
		{
			//PlayerControls = false;
			text.enqueue("hurry back!", 2f, null, false);
			text.enqueue("[press space to roll]", -1, null, false);
		}
		else if (torchesLit == 2)
		{
		//	PlayerControls = false;
			text.enqueue("look to the sky, child", -1, null, false);
			text.enqueue("see how wolf prepares for his first bite?", -1, null, false);
		}
		else if (torchesLit == 3)
		{
			cottage.GetComponent<SpriteRenderer>().enabled = false;
			AllTorchesLit = true;
		}
	}

	public void hitEdge()
	{
		if (torchesLit == 0 && !reportedThrowUp)
		{
			reportedThrowUp = true;
			text.enqueue("go on now!", 2f);
			text.enqueue("if you can't reach, toss the torch [up]", -1);
		}
	}

	public void OnReturnHome()
	{
		if (wolfFled)
		{
			gameEnded = true;
			text.enqueue("what kept you, dear?");
			text.enqueue("would you like to know how the story ends?");
			text.enqueue("a third time fox slipped a hot pepper onto wolf's tongue");
			text.enqueue("and a third time wolf howled in pain");
			text.enqueue("but this time, wolf had had enough");
			text.enqueue("look to the moon! wolf has fled!");
			text.enqueue("what a clever fox!", -1, delegate () { endGame(); });
			
			return;
		}

		if (torchesLit == 1)
		{
			text.enqueue("so fox went to wolf, and he said-");
			text.enqueue("\"how brave you are, wolf-\"");
			text.enqueue("\"to eat a whole firebird egg!\"");
			text.enqueue("\"I hear they are quite hot!\"", 4);


			text.enqueue("\"nonsense\", growled wolf, preparing to take a bite");
			text.enqueue("\"this is no firebird egg\"", 4);
			text.enqueue("\"well\", suggested fox");
			text.enqueue("\"perhaps you lick it first, to be sure?\"",4);


			text.enqueue("so wolf licked the moon, and just as he did-");
			text.enqueue("fox slipped a hot pepper onto wolf's tongue",4);


			text.enqueue("\"YOWCH\", howled wolf in pain", 4);

			PlayText.Callback sendForTorch = delegate () { OutForLamp = true; PlayerControls = true; };

			text.enqueue("now it's time to light the second lamp", -1, sendForTorch);
		}
		else if (torchesLit == 2)
		{
			text.enqueue("while wolf was howling, fox bopped his nose to get his attention");
			text.enqueue("\"let me help, wolf! together, we can eat the whole thing!\"",4);
			text.enqueue("when wolf saw fox lick the moon without pain,");
			text.enqueue("he began to get jealous");
			text.enqueue("once more, he licked the moon");
			text.enqueue("once more, fox slipped a hot pepper onto wolf's tongue");
			text.enqueue("once more, wolf howled in pain");
			text.enqueue("and once more, fox bopped wolf's nose to calm him",4, delegate() { OutForLamp = true; PlayerControls = true; RotateWorld = true; });


			text.enqueue("the time has come to light the final lamp");
		}

	}

	public void OnWolfTouch()
	{
		WolfHiding = false;
		BattleStarted = true;
	}

	public void OnWolfFlee()
	{
		wolfFled = true;
	}

	private void endGame()
	{

		StartCoroutine(fadeOut(2));
		queueRestartGame();
	}

	public IEnumerator fadeOut(float fadeDur)
	{
		SpriteRenderer sprite = blackScreen.GetComponent<SpriteRenderer>();
		blackScreen.SetActive(true);

		for (float timer = fadeDur; timer >= 0; timer -= Time.deltaTime)
		{
			Color color = sprite.color;
			color.a = 1f - timer / fadeDur;
			sprite.color = color;
			yield return null;
		}

		Color color2 = sprite.color;
		color2.a = 1f;
		sprite.color = color2;


	}

	private void queueRestartGame()
	{
		StartCoroutine(restartGame());
	}


	private IEnumerator restartGame()
	{
		yield return new WaitForSeconds(3f);

		SceneManager.LoadScene(0);
	}



}
