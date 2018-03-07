using System;
using SIGVerse.Common;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections;

public class ChooseTask : MonoBehaviour
{
	public string[] scenarioIDs = new string[]{ };
    public int now_scenarioID;
    public Image CheckedImg;
    public Image BackPanel;
    public ScrollRect ScrollPanel;

    public Text Title;
    public Text Steps;
        
    private int maxTutorialTask = 3;
    private int maxTask = 0;
    private bool isSetting = false;

    private Connect2MySQL sql;
    private string strStep = "";

    private void Start()
	{
        Title.text = "";
        Steps.text = "";
        now_scenarioID = 1;
        CheckedImg.enabled = false;
        sql = FindObjectOfType<Connect2MySQL>();
        ScrollPanel.verticalNormalizedPosition = 1.0f;
        if (sql.startTaskNum() != 0) startTask();
        else Steps.text = "SQLの認証に失敗しました\n再起動してください";
    }

    public void startTask()
    {
        if (!FindObjectOfType<TutorialMode>().GetTutorialMode()) //記録モード
        {
            now_scenarioID = sql.startTaskNum();
            maxTask = sql.getMaxTaskNumber();
            Debug.Log("start task id : " + now_scenarioID);
            setRecipe();
        }
        else // チュートリアルモード
        {
            Title.text = "<size=100>【操作方法について】</size>";
            List<string> memo = new List<string>();
            memo.Add("表示されている文章を見る場合は，右手のスティックを上下に倒してください");
            memo.Add("左手の親指の位置の下にあるXボタンを1回押して，このパネルを黄色にしましょう\n（表示されているタスクを選択します）");
            memo.Add("タスクの選択を取り消したい場合は，左手の親指の位置の上にあるYボタンを1回押して，このパネルを青色に戻してください");
            memo.Add("このパネルが黄色の状態で，Xボタンをさらに押すと，表示されているタスクを決定したことになります");
            memo.Add("<color=#dc143c>卵と醤油を混ぜてください</color>");
            memo.Add("上のStepのように，動作してほしいStepは赤色で表示されています\nすでにボウルに卵と醤油が入っているとします");
            memo.Add("左手にはボウル，右手には菜箸を持ちましょう\n中指のトリガーを押している間，手に触れている物体を把持することができます\n道具を落としてしまった場合は，右手コントローラーのスティックを押し込んでください（物体が最初の位置に戻ります）");
            memo.Add("動作の記録を行います\n右手の親指の位置の下にあるAボタンを1回押して，動作記録の合図を出します\nRecと画面上部に表示されている間の動作が記録されます");
            memo.Add("記録を終えるために，再度右手の親指の位置の下にあるAボタンを1回押しましょう");
            memo.Add("記録が終わると，毎回チュートリアルモードを続けるかを聞かれます\n左手のスティックを左右に倒すと「はい」「いいえ」を選択できます\n決定する場合は右手のAボタンを押してください");

            string SetSentence = "";
            for (int i = 0; i < memo.Count; i++)
            {
                SetSentence += "<color=#696969>Step " + (i + 1).ToString("D2") + ":\n" + memo[i] + "</color>\n\n";
            }
            Steps.text = SetSentence;
        }
    }

    public void setRecipe()
    {
        if (!this.isSetting) this.StartCoroutine(SetRecipeSteps());
    }

    private IEnumerator SetRecipeSteps()
    {
        this.isSetting = true;
        Connect2MySQL.isSelecting = true;
            
        Debug.Log("Set task no." + now_scenarioID + " sentence");

        Thread threadSelectData = new Thread(new ParameterizedThreadStart(setSentence));
        bool tutorial = FindObjectOfType<TutorialMode>().GetTutorialMode();
        threadSelectData.Start(tutorial);

        while (Connect2MySQL.isSelecting)
        {
            yield return new WaitForSeconds(0.1f);
            Title.text = "";
            Steps.text = "<size=160>\n  料理レシピを検索中</size>";
            BackPanel.color = new Color(231.0f / 255.0f, 252.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
            CheckedImg.enabled = false;
        }
        Debug.Log("Recipe Sentence is selected");

        string SetSentence = "";
        string SetTitle = "";
        if (strStep != "") SetSentence = strStep;
        else
        {
            string title = FindObjectOfType<Connect2MySQL>().getRecipeTitle();
            int step = Int32.Parse(FindObjectOfType<Connect2MySQL>().getStep());
            List<string> memo = FindObjectOfType<Connect2MySQL>().getRecipeMemo();
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

        List<int> Ended = new List<int>();
        if (!FindObjectOfType<TutorialMode>().GetTutorialMode()) Ended = FindObjectOfType<Connect2MySQL>().EndedTaskList();
        else Ended = FindObjectOfType<TutorialMode>().GetTutorialEndTask();

        if (Ended.Contains(now_scenarioID)) EndedTask();
        else
        {
            BackPanel.color = new Color(231.0f / 255.0f, 252.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
            CheckedImg.enabled = false;
        }
        this.isSetting = false;
    }

    private void setSentence(object o)
    {
        bool tutorial = (bool)o;
        strStep = sql.Select_taskInfo(tutorial, now_scenarioID);
    }


    public void EndedTask()
    {
        // 赤
        BackPanel.color = new Color(255.0f / 255.0f, 231.0f / 255.0f, 250.0f / 255.0f, 255.0f / 255.0f);
        CheckedImg.enabled = true;
    }

	public void FirstScenceID()
	{
        Title.text = "";
		Steps.text = "<size=160>\n左手のアナログスティックを\n<color=red>右</color>に倒してください</size>";
	}

	public void NextScenceID()
	{
        Debug.Log("Next --> ");
        now_scenarioID = now_scenarioID + 1;
        int max = 0;
        if (FindObjectOfType<TutorialMode>().GetTutorialMode()) max = maxTutorialTask + 1;
        else max = maxTask + 1;
        if (now_scenarioID > max)
        {
            Title.text = "";
            Steps.text = "<size=160>\n左手のアナログスティックを\n<color=red>左</color>に倒してください</size>";
            now_scenarioID = max;
        }
        else setRecipe();
    }

	public void BackScenceID()
	{
        Debug.Log("<-- Back ");
        now_scenarioID = now_scenarioID - 1;
        if (now_scenarioID <= 0)
        {
            FirstScenceID();
            now_scenarioID = 0;
        }
        else setRecipe();
    }

    public void ScrollMessage(float num)
    {
        num = num / 300;
        ScrollPanel.verticalNormalizedPosition += num;
        if (ScrollPanel.verticalNormalizedPosition < 0.0f) ScrollPanel.verticalNormalizedPosition = 0.0f;
        //Debug.Log(ScrollPanel.verticalNormalizedPosition);
    }

    public bool getSettingState()
    {
        return isSetting;
    }
}