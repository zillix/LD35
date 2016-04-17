using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayText {

	public string text;
	public float duration;
	public delegate void Callback(); 
	public Callback callback;
	public bool skippable;

	public static float DEFAULT_DURATION = 3f;

	public PlayText(string Text = "", float Duration = -1, Callback callbackFn = null, bool skippable = true)
	{
		if (Duration < 0) {
			Duration = DEFAULT_DURATION;
		}
		callback = callbackFn;
		text = Text;
		duration = Duration;
		this.skippable = skippable;
	}

	public static void addText(List<PlayText> textList, string text,float duration = -1, Callback callback = null, bool skippable = true)
	{
		if (duration < 0) {
			duration = DEFAULT_DURATION;
		}
		textList.Add (new PlayText(text, duration, callback, skippable));
	}
}
