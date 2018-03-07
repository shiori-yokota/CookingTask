using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using SIGVerse.Common;

public class UpdatingTransformList
{
    public float ElapsedTime { get; set; }
    private List<UpdatingTransformData> updatingTransformList;

    public UpdatingTransformList()
    {
        this.updatingTransformList = new List<UpdatingTransformData>();
    }

    public void AddUpdatingTransform(UpdatingTransformData updatingTransformData)
    {
        this.updatingTransformList.Add(updatingTransformData);
    }

    public List<UpdatingTransformData> GetUpdatingTransformList()
    {
        return this.updatingTransformList;
    }
}

public class ShowRecipe : MonoBehaviour {

    public GameObject ControllerPanel;
    public Image CheckedImg;
    public ScrollRect ScrollPanel;
    public Dropdown user_dropdown;
    public Dropdown task_dropdown;
    public Button PlayButton;

    public Text Title;
    public Text Steps;

    public List<string> AvatarMotions;
    public List<string> ObjectMotions;

    public bool isTextSetting = false;
    public bool isRelevanceSetting = false;
    public bool isAvatarMotionSetting = false;
    public bool isObjectMotionSetting = false;
    
    private Connect2MySQL sql;
    private string strStep = "";
    private int scenarioID;
    private bool isMotionInit = false;
    

    private List<GameObject> targetObjects;
    private PlaybackerCommon playbackerCommon;
    private Dictionary<string, Transform> targetObjectsPathMap = new Dictionary<string, Transform>();
    private List<UpdatingTransformList> playingTransformList;


    // Use this for initialization
    void Start () {
        Title.text = "";
        Steps.text = "";
        ControllerPanel.SetActive(false);
        CheckedImg.enabled = false;

        sql = FindObjectOfType<Connect2MySQL>();
        ScrollPanel.verticalNormalizedPosition = 1.0f;

        Steps.text = "エピソードを選択してください";

        playbackerCommon = FindObjectOfType<PlaybackerCommon>();
        SetTargetObject();
        
    }

    private void SetTargetObject()
    {
        this.targetObjects = playbackerCommon.GetTargetObjects();

        foreach (GameObject targetObj in targetObjects)
        {
            Transform[] transforms = targetObj.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms)
            {
                this.targetObjectsPathMap.Add(PlaybackerCommon.GetLinkPath(transform), transform);
            }
        }

    }

    public void OnClick()
    {
        this.isTextSetting = false;
        this.isRelevanceSetting = false;
        this.isAvatarMotionSetting = false;
        this.isObjectMotionSetting = false;
        this.isMotionInit = false;

        scenarioID = task_dropdown.value;
        Debug.Log("no." + scenarioID);
        setRecipe();
        LoadRelevanceInfo();
    }

    private void setRecipe()
    {
        isTextSetting = false;
        StartCoroutine(SetRecipeSteps());
    }

    private IEnumerator SetRecipeSteps()
    {
        Connect2MySQL.isSelecting = true;

        Thread threadSelectData = new Thread(new ThreadStart(setSentence));
        threadSelectData.Start();

        while (Connect2MySQL.isSelecting)
        {
            yield return new WaitForSeconds(0.1f);
            Title.text = "";
            Steps.text = "<size=160>\n  料理レシピを検索中</size>";
            PlayButton.interactable = false;
            user_dropdown.interactable = false;
            task_dropdown.interactable = false;
        }
        Debug.Log("Recipe Sentence is selected");

        string SetSentence = "";
        string SetTitle = "";
        if (strStep != "") SetSentence = strStep;
        else
        {
            string title = sql.getRecipeTitle();
            int step = Int32.Parse(sql.getStep());
            List<string> memo = sql.getRecipeMemo();
            SetTitle = "<size=100>【" + title + "】</size>";

            for (int i = 0; i < memo.Count; i++)
            {
                memo[i] = memo[i].Trim();
                if ((i + 1) == step) SetSentence += "<color=#dc143c>Step " + (i + 1).ToString("D2") + ": " + memo[i] + "</color>\n";
                else SetSentence += "<color=#696969>Step " + (i + 1).ToString("D2") + ": " + memo[i] + "</color>\n";
            }

        }
        Title.text = SetTitle;
        Steps.text = SetSentence;

        isTextSetting = true;
        user_dropdown.interactable = true;
        task_dropdown.interactable = true;


        PlayButton.interactable = true;

        if (this.isRelevanceSetting && !this.isMotionInit) LoadMotion();
    }

    private void setSentence()
    {
        strStep = sql.Select_taskInfo(false, scenarioID);
    }

    private void LoadRelevanceInfo()
    {
        StartCoroutine(loadRelevance());
    }

    private IEnumerator loadRelevance()
    {
        Thread threadSelectRelevance = new Thread(new ThreadStart(getRelevance));
        threadSelectRelevance.Start();

        while (Connect2MySQL.isRelevanceSelecting)
        {
            yield return new WaitForSeconds(0.1f);
        }

        isRelevanceSetting = true;
        if (this.isTextSetting && !this.isMotionInit) LoadMotion();
    }

    private void getRelevance()
    {
        sql.Select_RelevanceInfo();
    }

    private void LoadMotion()
    {
        isMotionInit = true;
        this.playingTransformList = new List<UpdatingTransformList>();
        StartCoroutine(SelectAvatarMotions());
        StartCoroutine(SelectObjectMotions());
    }

    private IEnumerator SelectAvatarMotions()
    {
        Connect2MySQL.isAvatarMotionSelecting = true;

        Thread threadSelectAvatarMotion = new Thread(new ThreadStart(getAvatarMotions));
        threadSelectAvatarMotion.Start();

        while (Connect2MySQL.isAvatarMotionSelecting)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("get avatar motions");

        this.isAvatarMotionSetting = true;
        //CreatePlayingTransformList(AvatarMotions, PlaybackerCommon.TypeDefMotion, PlaybackerCommon.TypeValMotion);
    }

    private IEnumerator SelectObjectMotions()
    {
        Connect2MySQL.isObjectMotionSelecting = true;
        
        Thread threadSelectObjectMotion = new Thread(new ThreadStart(getObjectsMotions));
        threadSelectObjectMotion.Start();

        while (Connect2MySQL.isObjectMotionSelecting)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("get object motions");

        this.isObjectMotionSetting = true;
        CreatePlayingTransformList(ObjectMotions, PlaybackerCommon.TypeDefObject, PlaybackerCommon.TypeValObject);
    }

    private void getAvatarMotions()
    {
        AvatarMotions = sql.SelectAvatarMotions();
    }

    private void getObjectsMotions()
    {
        ObjectMotions = sql.SelectObjectMotions();
    }

    private void CreatePlayingTransformList(List<string> motionsDataList, int TypeDef, int TypeVal)
    {
        List<Transform> transformOrder = new List<Transform>();

        foreach (string motionsData in motionsDataList)
        {
            string[] columnArray = motionsData.Split(new char[] { '\t' }, 2);

            if (columnArray.Length < 2) { continue; }

            string headerStr = columnArray[0];
            string dataStr = columnArray[1];

            string[] headerArray = headerStr.Split(',');
            string[] dataArray = dataStr.Split('\t');

            // Definition
            if (int.Parse(headerArray[1]) == TypeDef)
            {
                transformOrder.Clear();

                //				Debug.Log("data num=" + dataArray.Length);

                foreach (string transformPath in dataArray)
                {
                    if (!this.targetObjectsPathMap.ContainsKey(transformPath))
                    {
                        throw new Exception("Couldn't find the object that path is " + transformPath);
                    }

                    transformOrder.Add(this.targetObjectsPathMap[transformPath]);
                }
            }
            // Value
            else if (int.Parse(headerArray[1]) == TypeVal)
            {
                if (transformOrder.Count == 0) { continue; }

                UpdatingTransformList timeSeriesMotionsData = new UpdatingTransformList();

                timeSeriesMotionsData.ElapsedTime = float.Parse(headerArray[0]);

                for (int i = 0; i < dataArray.Length; i++)
                {
                    string[] transformValues = dataArray[i].Split(',');

                    UpdatingTransformData transformPlayer = new UpdatingTransformData();
                    transformPlayer.UpdatingTransform = transformOrder[i];

                    transformPlayer.LocalPosition = new Vector3(float.Parse(transformValues[0]), float.Parse(transformValues[1]), float.Parse(transformValues[2]));
                    transformPlayer.LocalRotation = new Vector3(float.Parse(transformValues[3]), float.Parse(transformValues[4]), float.Parse(transformValues[5]));

                    if (transformValues.Length == 9)
                    {
                        transformPlayer.LocalScale = new Vector3(float.Parse(transformValues[6]), float.Parse(transformValues[7]), float.Parse(transformValues[8]));
                    }

                    timeSeriesMotionsData.AddUpdatingTransform(transformPlayer);
                }

                this.playingTransformList.Add(timeSeriesMotionsData);
            }
        }
    }

    public List<UpdatingTransformList> GetPlayingTransformList()
    {
        return this.playingTransformList;
    }

    public Dictionary<string, Transform> GetTargetObjectsPathMap()
    {
        return this.targetObjectsPathMap;
    }
}
