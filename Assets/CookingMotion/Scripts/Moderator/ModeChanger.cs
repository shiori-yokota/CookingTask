using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ModeChanger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClickRecordButton()
	{
		SceneManager.LoadScene("CookingMotionDemo");
	}

	public void onClickPlayBackButton()
	{
		SceneManager.LoadScene("PlayBackMode");
	}
}
