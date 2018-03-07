using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OculusTouchRecordModerator : MonoBehaviour {
		
	private bool isfirst = false;   //record
    public bool PressRecordButton = false;

	private CookingMotionPlaybackRecorder recoder;
	private ChooseTask chooseTask;
	private TutorialMode tutorialMode;
	public EnvironmentLoader reset;
    public NewtonVR.NVRHand rightHand;
    public TextMesh RecText;
    public TextMesh CountdownText;

    private float time = 6.0f, firstTime = 6.0f;
    private int Xcount = 0;
    private string graspingObjectName = "Hand";

	private bool isPause = false;

	private GameObject playbackMode;
	private bool ischoseplayBack = false;
        
	// Use this for initialization
	void Start () {
        chooseTask = FindObjectOfType<ChooseTask>();
		tutorialMode = FindObjectOfType<TutorialMode>();
        recoder = FindObjectOfType<CookingMotionPlaybackRecorder>();
        RecText.GetComponent<Renderer>().enabled = false;
        CountdownText.GetComponent<Renderer>().enabled = false;
		playbackMode = GameObject.Find("PlayBackButton");

	}
	
	// Update is called once per frame
	void Update () {
		if (tutorialMode.tutorialToggle.isOn) playbackMode.SetActive(false);
		else playbackMode.SetActive(true);


        if (!isPause && !chooseTask.getSettingState())
        {
            /* Time */
            if (this.PressRecordButton)
            {
                time -= Time.deltaTime;
                if (time < 0) CountdownText.text = "Finish !";
                else CountdownText.text = ((int)time).ToString() + " [s]";
            }

            if (!this.PressRecordButton /*&& !this.isfirst*/)
            {
                Vector2 stickR = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
                chooseTask.ScrollMessage(stickR.y);
            }

            if (this.rightHand.IsInteracting)
            {
                if (this.rightHand.HoldButtonDown)
                {
                    if (this.rightHand.CurrentlyInteracting.tag == "Graspables")
                    {
                        this.graspingObjectName = this.rightHand.CurrentlyInteracting.name;
                        // Debug.Log(this.graspingObjectName);
                    }
                }
            }
            if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickLeft))
            {
                if (!this.PressRecordButton && !this.isfirst && Xcount == 0) chooseTask.BackScenceID();
            }
            if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickRight))  // press next trigger
            {
                if (!this.PressRecordButton && !this.isfirst && Xcount == 0) chooseTask.NextScenceID();
            }
            if (OVRInput.GetDown(OVRInput.RawButton.X)) // press select button
            {
                Xcount += 1;
                chooseTask.BackPanel.color = new Color(251.0f / 255.0f, 252.0f / 255.0f, 235.0f / 255.0f, 255.0f / 255.0f);
                if (Xcount == 2)
                {
                    Debug.Log("::: Start :::");
                    CountdownText.GetComponent<Renderer>().enabled = true;
                    CountdownText.text = ((int)time).ToString() + " [s]";
                    this.isfirst = true;
                }
            }
            if (OVRInput.GetDown(OVRInput.RawButton.Y))
            {
                Xcount = 0;
                chooseTask.BackPanel.color = new Color(231.0f / 255.0f, 252.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
            }
            if (OVRInput.GetDown(OVRInput.RawButton.A)) // press record button
            {
                if (this.isfirst)   // record
                {
                    checkOnRecord();
                    RecText.GetComponent<Renderer>().enabled = true;
                    this.PressRecordButton = true;
                    this.isfirst = false;
                }
                else // stop
                {
                    recoder.OnStop();
                    RecText.GetComponent<Renderer>().enabled = false;
                    CountdownText.GetComponent<Renderer>().enabled = false;
                    this.PressRecordButton = false;
					Xcount = 0;
					time = firstTime;
                }
            }
            if (OVRInput.GetDown(OVRInput.RawButton.RThumbstick)) // press environment reset button
            {
                Debug.Log("Reset");
                reset.resetEnvironment();
                Xcount = 0;
            }
        }
		if (!chooseTask.getSettingState())
		{
			if (OVRInput.GetDown(OVRInput.RawButton.X))
			{
				Debug.Log("Choose Task Number");
				ischoseplayBack = true;
			}
			if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickLeft))
			{
				if (!this.ischoseplayBack) chooseTask.BackScenceID();
			}
			if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickRight))  // press next trigger
			{
				if (!this.ischoseplayBack) chooseTask.NextScenceID();
			}
			if (this.ischoseplayBack && OVRInput.GetDown(OVRInput.RawButton.A))
			{
				Debug.Log("playBack Start");
			}

		}

    }

	private void checkOnRecord()
	{
		Debug.Log("Checked On Record");
		recoder.OnRecord(this.graspingObjectName);
	}

    public void SetPause(bool isP)
    {
        isPause = isP;
        Debug.Log("いまはPause状態である" + isPause);
    }
}