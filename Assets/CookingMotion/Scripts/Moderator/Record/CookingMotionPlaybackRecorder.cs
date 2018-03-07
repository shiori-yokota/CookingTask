#pragma warning disable 0414

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using SIGVerse.Common;
using System.Collections;
using System.Linq;
using System.Threading;

[System.Serializable]
public class CookingMotionSentenceInfo
{
    public string id;
    public string sentence;
}

[RequireComponent(typeof(PlaybackerCommon))]
public class CookingMotionPlaybackRecorder : MonoBehaviour//, IPlaybackDataHandler2
{
    [HeaderAttribute("Avatar")]
    public Transform avatar;
    public List<Transform> VRDevices;
    //public List<Transform> avatarJoints;


    [HeaderAttribute("Parameters")]
    [TooltipAttribute("milliseconds")]
    public int recordInterval = 20;

    //-----------------------------------------------------

    private List<Transform> targetTransformInstances;
    private List<Transform> GraspObjectInstances;

    private bool isRecording = false;

    private StreamWriter streamWriter;
    //private StreamWriter streamWriterForCommand;

    private float elapsedTime = 0.0f;
    private float previousRecordedTime = 0.0f;

    private List<string> savedMotionStrings;
    private List<string> savedObjectStrings;
    private string savedMotionHeaderStrings;
    private string savedObjectHeaderStrings;
    //private List<string> dataLines;

    public string startTime = null;

    private OculusTouchRecordModerator OTM;
    public string graspObjectName = string.Empty;

    private Connect2MySQL connect2MySQL;
    private ChooseTask chooseTask;
    private TutorialMode tutorial;
    private bool istutorial;

    // Use this for initialization
    void Start()
    {
        OTM = FindObjectOfType<OculusTouchRecordModerator>();
        connect2MySQL = FindObjectOfType<Connect2MySQL>();
        chooseTask = FindObjectOfType<ChooseTask>();
        tutorial = FindObjectOfType<TutorialMode>();
        istutorial = FindObjectOfType<TutorialMode>().GetTutorialMode();
    }


    // Update is called once per frame
    void Update()
    {
        this.elapsedTime += Time.deltaTime;

        if (this.isRecording && OTM.PressRecordButton)  // OculusÇ≈ééÇπÇÈèÍçáÇÕÉRÉÅÉìÉgÇÕÇ∏Ç∑
        {
            this.SaveMotions();
        }

	}

    public void OnRecord(string ObjectName)
    {
        if (!this.isRecording)
        {
            this.graspObjectName = ObjectName;
            SIGVerseLogger.Info("grasped " + this.graspObjectName);
            this.StartRecording();
        }
    }

    public void OnStop()
	{
		if (this.isRecording)
		{
			this.StartCoroutine(StopRecording());
		}
	}

	private IEnumerator OnStopCoroutine()
	{
		if (this.isRecording)
		{
			yield return new WaitForSeconds(3.0f);
			this.StopRecording();
		}
	}

	private void StartRecording()
	{

        SIGVerseLogger.Info("Start Recording");

        DateTime dataTime = DateTime.Now;

		this.targetTransformInstances = new List<Transform>();
        this.GraspObjectInstances = new List<Transform>();

        List<string> linkPathMotionList = new List<string>();
        List<string> linkPathObjectList = new List<string>();

        this.savedMotionHeaderStrings = string.Empty;
        this.savedObjectHeaderStrings = string.Empty;

        this.savedMotionHeaderStrings += "0.0," + PlaybackerCommon.TypeDefMotion;
        this.savedObjectHeaderStrings += "0.0," + PlaybackerCommon.TypeDefObject;

        this.targetTransformInstances.AddRange((from transform in this.VRDevices where transform != null select transform).ToList());
            
        // Avatar
        foreach (Transform avatarTransform in this.targetTransformInstances)
        {
            string linkPath = PlaybackerCommon.GetLinkPath(avatarTransform);

            this.savedMotionHeaderStrings += "\t" + linkPath;

            if (linkPathMotionList.Contains(linkPath))
            {
                SIGVerseLogger.Error("Objects in the same path exist. path = " + linkPath);
                throw new Exception("Objects in the same path exist.");
            }

            linkPathMotionList.Add(linkPath);
        }

        // íÕÇﬂÇÈï®ëÃ
        foreach (GameObject graspingCandidate in GameObject.FindGameObjectsWithTag("Graspables"))
        {
            this.GraspObjectInstances.Add(graspingCandidate.transform);

            string linkPath = PlaybackerCommon.GetLinkPath(graspingCandidate.transform);

            this.savedObjectHeaderStrings += "\t" + linkPath;

            if (linkPathObjectList.Contains(linkPath))
            {
                SIGVerseLogger.Error("Objects in the same path exist. path = " + linkPath);
                throw new Exception("Objects in the same path exist.");
            }

            linkPathObjectList.Add(linkPath);
        }

        this.savedMotionStrings = new List<string>();
        this.savedObjectStrings = new List<string>();

        // Reset elapsed time
        this.elapsedTime = 0.0f;
        this.previousRecordedTime = 0.0f;

        this.isRecording = true;

	}

	private IEnumerator StopRecording()
	{
		this.isRecording = false;

        Connect2MySQL.isWriting = true;

        if (!istutorial)
        {
            Thread threadWriteData = new Thread(new ThreadStart(this.InsertMotionsToMySQL));
            threadWriteData.Start();
                
            while (Connect2MySQL.isWriting)
            {
                yield return new WaitForSeconds(0.1f);
            }

			SIGVerseLogger.Info("Playback file is saved");
            chooseTask.EndedTask();
			FindObjectOfType<EnvironmentLoader>().resetEnvironment();

		} else {
            SIGVerseLogger.Info("Ended!");
            chooseTask.EndedTask();
			FindObjectOfType<EnvironmentLoader>().resetEnvironment();
			tutorial.TaskEnd();
        }
    }

    private void InsertMotionsToMySQL()
    {
        connect2MySQL.InsertAvatarMotions(savedMotionHeaderStrings, savedMotionStrings);
        connect2MySQL.InsertObjectMotions(savedObjectHeaderStrings, savedObjectStrings);
        connect2MySQL.InsertRelevance(this.graspObjectName);
    }

    private void SaveMotions()
	{
		if (1000.0 * (this.elapsedTime - this.previousRecordedTime) < recordInterval) { return; }

		string motionLineStr = string.Empty;
        string objectLineStr = string.Empty;

        motionLineStr += Math.Round(this.elapsedTime, 4, MidpointRounding.AwayFromZero) + "," + PlaybackerCommon.TypeValMotion;
        objectLineStr += Math.Round(this.elapsedTime, 4, MidpointRounding.AwayFromZero) + "," + PlaybackerCommon.TypeValObject;

		// Avatar
        foreach(Transform transform in this.targetTransformInstances)
		{
            motionLineStr += "\t" +
				Math.Round(transform.localPosition.x,    4, MidpointRounding.AwayFromZero) + "," +
				Math.Round(transform.localPosition.y,    4, MidpointRounding.AwayFromZero) + "," +
				Math.Round(transform.localPosition.z,    4, MidpointRounding.AwayFromZero) + "," +
				Math.Round(transform.localEulerAngles.x, 4, MidpointRounding.AwayFromZero) + "," +
				Math.Round(transform.localEulerAngles.y, 4, MidpointRounding.AwayFromZero) + "," +
				Math.Round(transform.localEulerAngles.z, 4, MidpointRounding.AwayFromZero);
				//Math.Round(transform.localScale.x,       4, MidpointRounding.AwayFromZero) + "," +
				//Math.Round(transform.localScale.y,       4, MidpointRounding.AwayFromZero) + "," +
				//Math.Round(transform.localScale.z,       4, MidpointRounding.AwayFromZero);
		}
		this.savedMotionStrings.Add(motionLineStr);

        // Object
        foreach(Transform transform in this.GraspObjectInstances)
        {
            objectLineStr += "\t" +
                Math.Round(transform.localPosition.x, 4, MidpointRounding.AwayFromZero) + "," +
                Math.Round(transform.localPosition.y, 4, MidpointRounding.AwayFromZero) + "," +
                Math.Round(transform.localPosition.z, 4, MidpointRounding.AwayFromZero) + "," +
                Math.Round(transform.localEulerAngles.x, 4, MidpointRounding.AwayFromZero) + "," +
                Math.Round(transform.localEulerAngles.y, 4, MidpointRounding.AwayFromZero) + "," +
                Math.Round(transform.localEulerAngles.z, 4, MidpointRounding.AwayFromZero);
        }
        this.savedObjectStrings.Add(objectLineStr);

        this.previousRecordedTime = this.elapsedTime;
	}

    //private static string GetLinkPath(Transform transform)
    //{
    //	string path = transform.name;

    //	while (transform.parent != null)
    //	{
    //		transform = transform.parent;
    //		path = transform.name + "/" + path;
    //	}

    //	return path;
    //}

    //public void OnPlaybackData(string playbackData)
    //{
    //	this.dataLines.Add(playbackData);
    //}

    //private string GetStartTime()
    //{
    //    DateTime now = DateTime.Now;
    //    string startTime = now.ToString("yyyyMMddHHmmssfff");

    //    return startTime;
    //}
}
