using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class CarnivalUnityApp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Start up the engine

		Carnival.StartEngine("6c566d865f4d647ab6f2c9f2653d670090751b80", ""); //Carnival Test App

		// Set up Handlers 
		Carnival.OnErrorEvent += (object sender, CarnivalErrorEventArgs e) => {
			Debug.Log (e.ErrorDescription);
		};
		Carnival.OnTagsRecievedEvent += (object sender, CarnivalTagsRecievedEvent e) => {
			foreach(string tag in e.Tags) {
				Debug.Log(tag);
			}
		};

		//Set some tags
		string[] tagsToSet = {"unity_app", "level1", "pro_player", "has_shared"};
		Carnival.SetTags(tagsToSet);

		// Get tags again
		Carnival.GetTags ();

		// Set Location
		Carnival.UpdateLocation (-44.01899F,176.565915F);

		//Log an event 
		Carnival.LogEvent ("User started playing game");

		//Set Some Custom Attributes
		Carnival.SetString ("Some String", "Unity-String");
		Carnival.SetBool (true, "Unity-Boolean");
		Carnival.SetFloat (3.141D, "Unity-Float");
		Carnival.SetDate (DateTime.Now, "Unity-Date");
		Carnival.SetInteger (123, "Unity-Integer");

		//Remove some Custom Attributes
		//Carnival.RemoveAttribute ("Unity-Integer");
	}
	public void OnClick() {
		Debug.Log ("Open Stream Clicked");
		Carnival.ShowMessageStream ();
	}
}
