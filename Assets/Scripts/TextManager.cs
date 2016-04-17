using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextManager : MonoBehaviour {

	public List<PlayText> textQueue;

	private PlayText currentText;
	private float currentDuration;

	private ITextBox textBox;
	public GameObject textBoxObject;
	private SoundBank sounds;

	void Awake()
	{
		textQueue = new List<PlayText> ();

	}

	// Use this for initialization
	void Start () {
		textBox = (ITextBox)textBoxObject.GetComponent (typeof(ITextBox));
		textBoxObject.SetActive (true);
		textBox.hide ();

		sounds = GameObject.Find("SoundBank").GetComponent<SoundBank>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown
			&& currentText != null
			&& currentText.skippable)
		{
			advanceText();
			return;
		}


		if (currentText != null) {
			if (currentDuration < currentText.duration)
			{
				currentDuration += Time.deltaTime;
				if (currentDuration >= currentText.duration)
				{
					advanceText();
				}

			}
		}
		else if (textQueue.Count > 0)
		{
			advanceText();
		}
	}
	
	public void enqueue(List<PlayText> text, bool force = false)
	{
		if (text.Count == 0) {
			return;
		}

		if (force) {
			textQueue.Clear ();
			currentText = null;
			currentDuration = 0;
		}
		textQueue.AddRange (text);
	}

	public void enqueue(string text, float duration = -1, PlayText.Callback callback = null, bool skippable = true)
	{
		textQueue.Add (new PlayText (text, duration, callback, skippable));
	}

	private void advanceText()
	{
		currentDuration = 0;

		if (currentText != null) {
			if (currentText.callback != null)
			{
				currentText.callback();
			}
		}

		if (textQueue.Count > 0) {
			if (textQueue[0].text.Length > 0)
			{
				sounds.player.PlayOneShot(sounds.textBubble, .2f);
				textBox.show();
			}
			currentText = textQueue [0];
			textQueue.RemoveAt (0);
			textBox.text = currentText.text;
		//	textBox.target = currentText.speaker;
		} else {
			reset();
		}
	}

	private void reset()
	{
		currentText = null;
		currentDuration = 0;
		textBox.hide();
	}

	public bool isBusy
	{
		get {
			return currentText != null;
		}
	}


}
