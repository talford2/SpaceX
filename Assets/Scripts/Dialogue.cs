﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dialogue
{
	//public List<DialogueScripts> FriendlyFire;

	private static Dialogue _dialogue;


	private static Dictionary<string, List<string>> _dialogueDictionary;

	public static Dictionary<string, List<string>> DialogueDictionary
	{
		get
		{
			if (_dialogueDictionary == null)
			{
				_dialogueDictionary = new Dictionary<string, List<string>>();
				_dialogueDictionary.Add("FriendlyFire", new List<string>
				{
					"Hey {0}! I'm on your side!",
					"STOP IT!",
					"Stop It {0}!",
					"{0}!!!",
					"Why are you hurting me!?",
					"What did I do to you, {0}?",
					"Don't shoot at your friends!",
					"Friendly Fire!",
					"{0}, I'm FRIENDLY!",
					"Hey wise guy, I'm on your side!",
					"Save it for the enemy!",
					"YOU FUCKER!",
					"Fuck Off {0}!",
					"You're going to regret that!",
					"What is wrong with you?!",
					"What's wrong with you {0}?!",
					"{0}, what the hell?!!",
					"{0} Don't!",
					"{0} don't you like me?",
					"Try it again {0}, I dare you!",
					"I was always nice to You {0}!"
				});
			}
			return _dialogueDictionary;
		}
	}
}