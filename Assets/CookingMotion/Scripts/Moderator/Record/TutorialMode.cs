using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class TutorialMode : MonoBehaviour {

    public Toggle tutorialToggle;
    private bool logInInfo;

    public GameObject PauseUIPrefab;

    private OculusTouchRecordModerator TouchModerator;
    private ChooseTask choosetask;

	private Button YesButton;
    private Button NoButton;
	private bool PauseReady = false;
    private bool isChoose = false;

	public List<int> TutorialEndTask = new List<int>();

	// Use this for initialization
	void Awake() {
        PauseUIPrefab.SetActive(false);
        tutorialToggle.interactable = false;
        logInInfo = LogInManager.GetToggleBool();
        tutorialToggle.isOn = logInInfo;
        TouchModerator = FindObjectOfType<OculusTouchRecordModerator>();
        choosetask = FindObjectOfType<ChooseTask>();
            
	}

    // Update is called once per frame
    void Update() {
		if(PauseReady && EventSystem.current.currentSelectedGameObject == YesButton.gameObject)
		{
			Debug.Log("Change");
            YesButton.interactable = true;
            NoButton.interactable = true;
            PauseReady = false;
            isChoose = true;
        }
        if (isChoose && EventSystem.current.currentSelectedGameObject == YesButton.gameObject)
        {
            NoButton.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            YesButton.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        }
        if (isChoose && EventSystem.current.currentSelectedGameObject == NoButton.gameObject)
        {
            NoButton.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
            YesButton.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }

    public bool GetTutorialMode()
    {
        if (tutorialToggle.isOn) return true;
        else return false;
    }

	public List<int> GetTutorialEndTask()
	{
		return TutorialEndTask;
	}

    public void TaskEnd()
    {
        if (tutorialToggle.isOn)
        {
			TutorialEndTask.Add(choosetask.now_scenarioID);
			PauseReady = true;
            Debug.Log("続けますか");
            PauseUIPrefab.SetActive(true);
            TouchModerator.SetPause(true);
            YesButton = GameObject.Find("PauseCanvas/PausePanel/YesButton").GetComponent<Button>();
            NoButton = GameObject.Find("PauseCanvas/PausePanel/NoButton").GetComponent<Button>();
            YesButton.interactable = false;
            NoButton.interactable = false;
			EventSystem.current.SetSelectedGameObject(YesButton.gameObject, null);
		}
    }

    public void TutorialFinish()
    {
		Debug.Log("Finish");
        PauseUIPrefab.SetActive(false);
        TouchModerator.SetPause(false);
        EventSystem.current.SetSelectedGameObject(null);
        tutorialToggle.isOn = false;
        isChoose = false;
        choosetask.startTask();
    }

    public void TutorialContinue()
    {
		Debug.Log("Continue");
		PauseUIPrefab.SetActive(false);
		TouchModerator.SetPause(false);
		EventSystem.current.SetSelectedGameObject(null);
        isChoose = false;
        choosetask.NextScenceID();
	}
}