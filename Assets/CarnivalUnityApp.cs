using UnityEngine;
using System.Collections;

public class CarnivalUnityApp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Start up the engine
		Carnival.StartEngine("6c566d865f4d647ab6f2c9f2653d670090751b80"); //Carnival Test App

		//Set some tags
		string[] tagsToSet = {"unity_app", "level1", "pro_player"};
		Carnival.SetTagsInBackground(tagsToSet);

		// Get tags again
		Carnival.GetTagsInBackground ("CarnivalUnityApp", "GetTags", "GetTagsError");

		// Set Location
		Carnival.UpdateLocation (-44.018990, -176.565915);
	}
	public void OnClick() {
		Debug.Log ("Open Stream Clicked");
		Carnival.ShowMessageStream ();
	}

	public void GetTags(string tags){
		Debug.Log (tags);
	}
	public void GetTagsError(string error){
		Debug.Log (error);
	}

}
