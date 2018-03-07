using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using SIGVerse.Common;
using MySql.Data.MySqlClient;


public class Connect2MySQL : MonoBehaviour
{
    private string currentPlayerName;
    private string currentSentece;

    public static bool UserNameIsUsed = false;

    private const string MySqlSchemaName = "cooking_task";
    private const string MySqlTableName_userInfo = "user_data";
    private const string MySqlTableName_userState = "user_state";
    private const string MySqlTableName_taskInfo = "mixing_task";
    private const string MySqlTableName_avatarInfo = "motion_data";
    private const string MySqlTableName_objectInfo = "object_data";
    private const string MySqlTableName_relevanceInfo = "relevance_data";

    private const string MySqlTableName_tutorialInfo = "tutorial_task";

	private const string CookingRecipeSchema = "cookpad_data";
	private const string CookingRecipeTable_recipe = "recipes";

	private const string mysqlIpInputField = "localhost";
    private const string mysqlPortInputField = "1234";
    private const string mysqlUserInputField = "cooking_app";
    private const string mysqlPassInputField = "cookingtask";

    private string savedUserSettings;

    private int task_num = 0;
    private int max_taskNum;
    private string step;
    private string userID;
    private string uniqueID;

	public string RecipeTitle = "";
	public List<string> RecipeSteps;
    
    private bool islogin = false;
    private bool FinishWritingMotion = false;
    private bool FinishWritingObject = false;
    private bool FinishWritingRelevance = false;
    public static bool isWriting = false;
    private bool FinishSelectingID = false;
    private bool FinishSelectingTitle = false;
    private bool FinishSelectingSteps = false;

    public static bool isSelecting = false;
    public static bool isAvatarMotionSelecting = false;
    public static bool isObjectMotionSelecting = false;
    public static bool isRelevanceSelecting = false;

    private string toolName;
    private string recording_id;

    public List<int> endedTask;
    private Dictionary<string, string> userDic = new Dictionary<string, string>();

    void Start()
    {
        DontDestroyOnLoad(this);
        this.endedTask = new List<int>();
    }


    // に接続してログイン ------------------------
    public void logIn(string name)
    {
        Debug.Log("Log In");
        MySqlConnection mysqlConn = null;
        MySqlCommand mySqlCommand = null;

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password =" + mysqlPassInputField + ";";

        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

            string selectSql = "SELECT * FROM " + MySqlSchemaName + "." + MySqlTableName_userInfo + " WHERE user_name='" + name + "'";

            mySqlCommand = new MySqlCommand(selectSql, mysqlConn);

            IAsyncResult iAsync = mySqlCommand.BeginExecuteReader();

            while (!iAsync.IsCompleted)
            {
                Thread.Sleep(100);
            }

            MySqlDataReader mySqlDataReader = mySqlCommand.EndExecuteReader(iAsync);

            List<string> userInfoList = new List<string>();
            int user_id = 0;

            while (mySqlDataReader.Read())
            {
                userInfoList.Add(mySqlDataReader.GetString("user_name"));
                user_id = int.Parse(mySqlDataReader.GetString("user_id"));
            }

            this.userID = user_id.ToString();

            mySqlDataReader.Close();
            mySqlCommand.Dispose();
            mysqlConn.Close();
            
            
            this.selectMaxTaskNum();
            this.checkTask(user_id.ToString());

            if (islogin) currentPlayerName = userInfoList[0];
            

        }
        catch(Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();

        }

    }

    // Sign up ------------------------
    public void signUp(string name)
    {
        MySqlConnection mysqlConn = null;
        MySqlCommand mysqlCommand = null;

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password=" + mysqlPassInputField + ";";

        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

            // Write user data
            string insertSql = string.Empty;
            this.savedUserSettings = string.Empty;
            this.savedUserSettings = name;

            this.CreateInsertQuery_user(ref insertSql, this.savedUserSettings);

            mysqlCommand = new MySqlCommand(insertSql, mysqlConn);
            mysqlCommand.ExecuteNonQuery();

            SIGVerseLogger.Info("Inserted " + "1" + " records.");

            mysqlCommand.Dispose();
            mysqlConn.Close();

            currentPlayerName = name;
            UserNameIsUsed = false;
        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();

            UserNameIsUsed = true;
        }
        
    }

    private void CreateInsertQuery_user(ref string insertSql, string UserName)
    {
        int userId = this.makeUserId(UserName);
        Debug.Log(userId);

        string valueString =
            "(" + userId + ",'" + UserName + "')";

        //Debug.Log(valueString);
        
        if (insertSql == string.Empty)
        {
            insertSql =
                "INSERT INTO " + MySqlSchemaName + "." + MySqlTableName_userInfo + " " +
                "(user_id, user_name) " +
                "VALUES " + valueString;
        }
        else
        {
            insertSql += "," + valueString;
        }
    }

    private int makeUserId(string seedString)
    {
        int seed = 0;
        char[] chars = seedString.ToCharArray();
        int len = seedString.Length - 1;
        foreach (var c in chars)
        {
            int asc = (int)c - 64;
            seed += asc * (int)Math.Pow((double)26, (double)len--);
        }
        System.Random rand = new System.Random(seed);
        int val = rand.Next();

        return val;
    }


    // return current user name --------------------
    public string currentPlayer()
    {
        return currentPlayerName;
    }

    public int startTaskNum()
    {
        Debug.Log("task_num: " + task_num);
        return task_num;
    }

    public List<int> EndedTaskList()
    {
        return endedTask;
    }

	public string getRecipeTitle()
	{
		return RecipeTitle;
	}

	public List<string> getRecipeMemo()
	{
		return RecipeSteps;
	}

    public string getStep()
    {
        return step;
    }

    public int getMaxTaskNumber()
    {
        return max_taskNum;
    }

    public List<string> GetUserList()
    {
        List<string> UserName = new List<string>();
        Select_Users();

        if (currentPlayerName == "owner")
        {
            foreach (string s in userDic.Keys)
            {
                if (s != "owner") UserName.Add(s);
            }
        }
        else
        {
            UserName.Add(currentPlayerName);
        }

        return UserName;
    }

    public List<string> GetFinishTaskList(string username)
    {
        List<string> taskList = new List<string>();
        userID = userDic[username];

        taskList = Select_FinishTask(userID);

        return taskList;
    }

    private void Select_Users()
    {
        MySqlConnection mysqlConn = null;
        MySqlCommand mySqlCommand = null;
        
        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password=" + mysqlPassInputField + ";";

        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

            string selectSql = "SELECT * FROM " + MySqlSchemaName + "." + MySqlTableName_userInfo;
            // Debug.Log(selectSql);

            mySqlCommand = new MySqlCommand(selectSql, mysqlConn);
            IAsyncResult iAsync = mySqlCommand.BeginExecuteReader();
            while (!iAsync.IsCompleted)
            {
                Thread.Sleep(100);
            }
            MySqlDataReader mySqlDataReader = mySqlCommand.EndExecuteReader(iAsync);

            while (mySqlDataReader.Read())
            {
                userDic.Add(mySqlDataReader.GetString("user_name"), mySqlDataReader.GetString("user_id"));
            }
            mySqlDataReader.Close();
            mySqlCommand.Dispose();
            mysqlConn.Close();
            
        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();
            
        }
    }

    private List<string> Select_FinishTask(string username)
    {
        MySqlConnection mysqlConn = null;
        MySqlCommand mySqlCommand = null;

        List<string> Tasks = new List<string>();

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password=" + mysqlPassInputField + ";";

        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

            string selectSql = "SELECT * FROM " + MySqlSchemaName + "." + MySqlTableName_userState + " WHERE user_id='" + username + "'";
            //Debug.Log(selectSql);

            mySqlCommand = new MySqlCommand(selectSql, mysqlConn);
            IAsyncResult iAsync = mySqlCommand.BeginExecuteReader();
            while (!iAsync.IsCompleted)
            {
                Thread.Sleep(100);
            }
            MySqlDataReader mySqlDataReader = mySqlCommand.EndExecuteReader(iAsync);

            while (mySqlDataReader.Read())
            {
                Tasks.Add(mySqlDataReader.GetString("task_id"));
            }
            mySqlDataReader.Close();
            mySqlCommand.Dispose();
            mysqlConn.Close();

            return Tasks;
        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();

            return Tasks;
        }
    }

    public string Select_taskInfo(bool isTutorial, int taskNum)
    {
        task_num = taskNum;
        MySqlConnection mysqlConn = null;
        MySqlCommand mySqlCommand = null;

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password=" + mysqlPassInputField + ";";

        int maxTaskId = 0;

        if (isTutorial) maxTaskId = 3;
        else maxTaskId = max_taskNum;

        if (taskNum > maxTaskId)
        {
            string message = "";
            if (isTutorial) message = "Finish!!\n challenge the collect mode";
            else message = "Finish!!\n Thank you for your cooperation :) ";
            return message;
        }
        else
        {
            try
            {
                mysqlConn = new MySqlConnection(connString);
                mysqlConn.Open();

                string selectSql = "SELECT * FROM " + MySqlSchemaName + "." + MySqlTableName_taskInfo + " WHERE task_id='" + taskNum + "'";

                //Debug.Log(selectSql);

                mySqlCommand = new MySqlCommand(selectSql, mysqlConn);

                IAsyncResult iAsync = mySqlCommand.BeginExecuteReader();

                while (!iAsync.IsCompleted)
                {
                    Thread.Sleep(100);
                }

                MySqlDataReader mySqlDataReader = mySqlCommand.EndExecuteReader(iAsync);

                List<string> RecipeIDList = new List<string>();
                List<string> MixingStep = new List<string>();

                while (mySqlDataReader.Read())
                {
					RecipeIDList.Add(mySqlDataReader.GetString("recipe_id"));
                    MixingStep.Add(mySqlDataReader.GetString("step"));
                }

                mySqlDataReader.Close();
                mySqlCommand.Dispose();
                mysqlConn.Close();

                FinishSelectingID = true;

                //Debug.Log("RecipeID is " + RecipeIDList[0]);
                //Debug.Log("MixingStep is " + MixingStep[0]);

                this.step = MixingStep[0];
                this.task_num = taskNum;
                this.uniqueID = this.userID.Substring(0,5) + taskNum.ToString("D4");
                //Debug.Log(uniqueID);

				RecipeTitle = SelectRecipeInfo("recipes", RecipeIDList[0])[0];
				RecipeSteps = SelectRecipeInfo("steps", RecipeIDList[0]);

                if (FinishSelectingID && FinishSelectingTitle && FinishSelectingSteps)
                {
                    isSelecting = false;
                    FinishSelectingID = false;
                    FinishSelectingTitle = false;
                    FinishSelectingSteps = false;
                }

                return "";

            }
            catch (Exception ex)
            {
                SIGVerseLogger.Error(ex.Message);
                SIGVerseLogger.Error(ex.StackTrace);

                if (mysqlConn != null)
                    mysqlConn.Close();
                Application.Quit();

				return "";

            }
        }
    }

	private List<string> SelectRecipeInfo(string tableName , string id)
	{
		MySqlConnection mysqlConn = null;
		MySqlCommand mySqlCommand = null;

		List<string> taskInfoList = new List<string>();

		string connString =
			"server =" + mysqlIpInputField + ";" +
			"port =" + mysqlPortInputField + ";" +
			"database =" + CookingRecipeSchema + ";" +
			"userid =" + mysqlUserInputField + ";" +
			"password=" + mysqlPassInputField + ";";

		string tmp = "", select = "", option = "";
		if (tableName == "recipes")
		{
			tmp = "id";
			select = "title";
			option = "";
		}
		else if (tableName == "steps")
		{
			tmp = "recipe_id";
			select = "memo";
			option = " ORDER BY position";
		}
		try
		{
			mysqlConn = new MySqlConnection(connString);
			mysqlConn.Open();

			string selectSql = "SELECT * FROM " + CookingRecipeSchema + "." + tableName + " WHERE " + tmp + "='" + id + "'" + option;
			// Debug.Log(selectSql);

			mySqlCommand = new MySqlCommand(selectSql, mysqlConn);
			IAsyncResult iAsync = mySqlCommand.BeginExecuteReader();
			while (!iAsync.IsCompleted)
			{
				Thread.Sleep(100);
			}
			MySqlDataReader mySqlDataReader = mySqlCommand.EndExecuteReader(iAsync);

			while (mySqlDataReader.Read())
			{
				taskInfoList.Add(mySqlDataReader.GetString(select));
			}
			mySqlDataReader.Close();
			mySqlCommand.Dispose();
			mysqlConn.Close();

            if (tableName == "recipes") FinishSelectingTitle = true;
            else if (tableName == "steps") FinishSelectingSteps = true;

            return taskInfoList;
		}
		catch (Exception ex)
		{
			SIGVerseLogger.Error(ex.Message);
			SIGVerseLogger.Error(ex.StackTrace);

			if (mysqlConn != null)
				mysqlConn.Close();
			Application.Quit();

			return taskInfoList;
		}
	}

    public void Select_RelevanceInfo()
    {
        MySqlConnection mysqlConn = null;
        MySqlCommand mySqlCommand = null;

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password=" + mysqlPassInputField + ";";

        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

            string selectSql = "SELECT * FROM " + MySqlSchemaName + "." + MySqlTableName_relevanceInfo + " WHERE user_id='" + userID + "' AND task_id='" + task_num + "'";
            //Debug.Log(selectSql);

            mySqlCommand = new MySqlCommand(selectSql, mysqlConn);
            IAsyncResult iAsync = mySqlCommand.BeginExecuteReader();

            while (!iAsync.IsCompleted)
            {
                Thread.Sleep(100);
            }
            MySqlDataReader mySqlDataReader = mySqlCommand.EndExecuteReader(iAsync);

            List<string> toolNames = new List<string>();
            List<string> recordingIDs = new List<string>();

            while (mySqlDataReader.Read())
            {
                toolNames.Add(mySqlDataReader.GetString("tool_name"));
                recordingIDs.Add(mySqlDataReader.GetString("recording_id"));
            }

            mySqlDataReader.Close();
            mySqlCommand.Dispose();
            mysqlConn.Close();
            
            this.toolName = toolNames[0];
            this.recording_id = recordingIDs[0];

            isRelevanceSelecting = false;
        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();
            
        }
    }

    public void selectMaxTaskNum()
    {
        MySqlConnection mysqlConn = null;
        MySqlCommand mySqlCommand = null;

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password=" + mysqlPassInputField + ";";
        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

            string selectSql = "SELECT table_rows FROM information_schema.tables WHERE table_schema='" + MySqlSchemaName + "' AND table_name='" + MySqlTableName_taskInfo + "'";
            // Debug.Log(selectSql);

            mySqlCommand = new MySqlCommand(selectSql, mysqlConn);
            IAsyncResult iAsync = mySqlCommand.BeginExecuteReader();
            while (!iAsync.IsCompleted)
            {
                Thread.Sleep(100);
            }
            MySqlDataReader mySqlDataReader = mySqlCommand.EndExecuteReader(iAsync);
            List<string> taskInfoList = new List<string>();
            while (mySqlDataReader.Read())
            {
                taskInfoList.Add(mySqlDataReader.GetString("table_rows"));
            }
            mySqlDataReader.Close();
            mySqlCommand.Dispose();
            mysqlConn.Close();

            max_taskNum = int.Parse(taskInfoList[0]);
            SIGVerseLogger.Info("Max task num is " + max_taskNum);
        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();
        }
    }

    public List<string> SelectAvatarMotions()
    {
        MySqlConnection mysqlConn = null;
        List<string> motionDataList = new List<string>();

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password =" + mysqlPassInputField + ";";
        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();
            string selectSql = "SELECT * FROM " + MySqlSchemaName + "." + MySqlTableName_avatarInfo + " WHERE recording_id='" + recording_id + "'";
            //Debug.Log(selectSql);

            MySqlCommand mysqlCommand = new MySqlCommand(selectSql, mysqlConn);
            IAsyncResult iAsync = mysqlCommand.BeginExecuteReader();
            while(!iAsync.IsCompleted)
            {
                Thread.Sleep(100);
            }

            MySqlDataReader mysqlDataReader = mysqlCommand.EndExecuteReader(iAsync);

            while(mysqlDataReader.Read())
            {
                motionDataList.Add(mysqlDataReader.GetString("motion_data"));
            }
            mysqlDataReader.Close();
            mysqlCommand.Dispose();
            mysqlConn.Close();

            isAvatarMotionSelecting = false;

            return motionDataList;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Debug.Log(ex.StackTrace);

            if (mysqlConn != null) { mysqlConn.Close(); }
            Application.Quit();
            return motionDataList;
        }
    }

    public List<string> SelectObjectMotions()
    {
        MySqlConnection mysqlConn = null;
        List<string> objectDataList = new List<string>();

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password =" + mysqlPassInputField + ";";
        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();
            string selectSql = "SELECT * FROM " + MySqlSchemaName + "." + MySqlTableName_objectInfo + " WHERE recording_id='" + recording_id + "'";
            
            MySqlCommand mysqlCommand = new MySqlCommand(selectSql, mysqlConn);
            IAsyncResult iAsync = mysqlCommand.BeginExecuteReader();
            while (!iAsync.IsCompleted)
            {
                Thread.Sleep(100);
            }

            MySqlDataReader mysqlDataReader = mysqlCommand.EndExecuteReader(iAsync);

            while (mysqlDataReader.Read())
            {
                objectDataList.Add(mysqlDataReader.GetString("object_data"));
            }
            mysqlDataReader.Close();
            mysqlCommand.Dispose();
            mysqlConn.Close();

            isObjectMotionSelecting = false;

            return objectDataList;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Debug.Log(ex.StackTrace);

            if (mysqlConn != null) { mysqlConn.Close(); }
            Application.Quit();
            return objectDataList;
        }
    }


    // ユーザごとの単位タスクの進捗状況をチェックする
    private void checkTask(string id)
    {
        MySqlConnection mysqlConn = null;
        MySqlCommand mySqlCommand = null;

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password =" + mysqlPassInputField + ";";
        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

             string selectSql = "SELECT * FROM " + MySqlSchemaName + "." + MySqlTableName_userState + " " + "WHERE user_id='" + id + "'";

            //Debug.Log(selectSql);

            mySqlCommand = new MySqlCommand(selectSql, mysqlConn);
            IAsyncResult iAsync = mySqlCommand.BeginExecuteReader();
            while (!iAsync.IsCompleted)
            {
                Thread.Sleep(100);
            }
            MySqlDataReader mySqlDataReader = mySqlCommand.EndExecuteReader(iAsync);

            while (mySqlDataReader.Read())
            {
                endedTask.Add(Int32.Parse(mySqlDataReader.GetString("task_id")));
            }
            mySqlDataReader.Close();
            mySqlCommand.Dispose();
            mysqlConn.Close();

            endedTask.Sort();

            for (int i = 1; i <= max_taskNum; i++)
            {
                if (!endedTask.Contains(i))
                {
                    this.task_num = i;
                    this.uniqueID = this.userID + i.ToString();
                    //Debug.Log(uniqueID);
                    this.islogin = true;
                    break;
                }
            }
            // SIGVerseLogger.Info("Start task is " + start_task);
        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();
        }
    }
    
    public void InsertAvatarMotions(string headerStrings, List<string> motionPattern)
    {
        MySqlConnection mysqlConn = null;
        MySqlCommand mysqlCommand = null;

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password =" + mysqlPassInputField + ";";

        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

            // Write user data
            string insertSql = string.Empty;
            SIGVerseLogger.Info(headerStrings);
            this.CreateInsertQuery_motion(ref insertSql, headerStrings, "Avatar");

            mysqlCommand = new MySqlCommand(insertSql, mysqlConn);
            mysqlCommand.ExecuteNonQuery();

            SIGVerseLogger.Info("Inserted header");

            int cnt= 0;
            insertSql = string.Empty;

            foreach (string savedMotionString in motionPattern)
            {
                this.CreateInsertQuery_motion(ref insertSql, savedMotionString, "Avatar");
                cnt++;

                if (cnt == 100)
                {
                    mysqlCommand = new MySqlCommand(insertSql, mysqlConn);
                    mysqlCommand.ExecuteNonQuery();

                    cnt = 0;
                    insertSql = string.Empty;
                }
            }

            if (cnt != 0)
            {
                mysqlCommand = new MySqlCommand(insertSql, mysqlConn);
                mysqlCommand.ExecuteNonQuery();
            }
            
            SIGVerseLogger.Info("Inserted " + motionPattern.Count + " records.");

            mysqlCommand.Dispose();
            mysqlConn.Close();

            this.FinishWritingMotion = true;
            if (FinishWritingMotion && FinishWritingObject && FinishWritingRelevance)
            {
                endedTask.Add(this.task_num);
                endedTask.Sort();
                isWriting = false;
                FinishWritingMotion = false;
                FinishWritingObject = false;
                FinishWritingRelevance = false;
            }

        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();
        }
    }

    public void InsertObjectMotions(string ObjectHeaderStr, List<string> objectMotions)
    {
        MySqlConnection mysqlConn = null;
        MySqlCommand mysqlCommand = null;

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password =" + mysqlPassInputField + ";";

        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

            // Write user data
            string insertSql = string.Empty;
            SIGVerseLogger.Info(ObjectHeaderStr);
            this.CreateInsertQuery_motion(ref insertSql, ObjectHeaderStr, "Objects");

            mysqlCommand = new MySqlCommand(insertSql, mysqlConn);
            mysqlCommand.ExecuteNonQuery();

            SIGVerseLogger.Info("Inserted header");

            int cnt = 0;
            insertSql = string.Empty;

            foreach (string savedObjectString in objectMotions)
            {
                this.CreateInsertQuery_motion(ref insertSql, savedObjectString, "Objects");
                cnt++;

                if (cnt == 100)
                {
                    mysqlCommand = new MySqlCommand(insertSql, mysqlConn);
                    mysqlCommand.ExecuteNonQuery();

                    cnt = 0;
                    insertSql = string.Empty;
                }
            }

            if (cnt != 0)
            {
                mysqlCommand = new MySqlCommand(insertSql, mysqlConn);
                mysqlCommand.ExecuteNonQuery();
            }

            SIGVerseLogger.Info("Inserted " + objectMotions.Count + " records.");

            mysqlCommand.Dispose();
            mysqlConn.Close();

            this.FinishWritingObject = true;
            if (FinishWritingMotion && FinishWritingObject && FinishWritingRelevance)
            {
                endedTask.Add(this.task_num);
                endedTask.Sort();
                isWriting = false;
                FinishWritingMotion = false;
                FinishWritingObject = false;
                FinishWritingRelevance = false;
            }

        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();
        }
    }

    private void CreateInsertQuery_motion(ref string insertSql, string dataStrings, string msg)
    {
        string[] dataStringsArray = dataStrings.Split("\t".ToCharArray(), 2);
        string[] headerArray = dataStringsArray[0].Split(",".ToCharArray());

        int elapsedTime = (int)(float.Parse(headerArray[0]) * 1000);
        int dataType = (int)(int.Parse(headerArray[1]));

        string valueString =
            "(" +
                this.uniqueID.ToString() + "," +
                elapsedTime + "," +
                dataType + "," +
                "'" + dataStrings + "'" +
            ")";

        if (insertSql == string.Empty)
        {
            if (msg == "Avatar")
            {
                insertSql =
                    "INSERT INTO " + MySqlSchemaName + "." + MySqlTableName_avatarInfo + " " +
                    "(recording_id, elapsed_time, data_type, motion_data) " +
                    "VALUES " + valueString;
            }
            else if (msg == "Objects")
            {
                insertSql =
                    "INSERT INTO " + MySqlSchemaName + "." + MySqlTableName_objectInfo + " " +
                    "(recording_id, elapsed_time, data_type, object_data) " +
                    "VALUES " + valueString;
            }
        }
        else
        {
            insertSql += "," + valueString;
        }
    }

    public void InsertRelevance(string tool_name)
    {
        MySqlConnection mysqlConn = null;
        MySqlCommand mysqlCommand = null;

        string connString =
            "server =" + mysqlIpInputField + ";" +
            "port =" + mysqlPortInputField + ";" +
            "database =" + MySqlSchemaName + ";" +
            "userid =" + mysqlUserInputField + ";" +
            "password =" + mysqlPassInputField + ";";

        try
        {
            mysqlConn = new MySqlConnection(connString);
            mysqlConn.Open();

            // Write user data
            string insertSql = string.Empty;

            this.CreateInsertQuery_relevance(ref insertSql, tool_name);

            mysqlCommand = new MySqlCommand(insertSql, mysqlConn);
            mysqlCommand.ExecuteNonQuery();


            insertSql = string.Empty;
            this.CreateInsertQuery_userState(ref insertSql);

            mysqlCommand = new MySqlCommand(insertSql, mysqlConn);
            mysqlCommand.ExecuteNonQuery();

            SIGVerseLogger.Info("Inserted " + "2" + " records.");

            mysqlCommand.Dispose();
            mysqlConn.Close();

            this.FinishWritingRelevance = true;
            if (FinishWritingMotion && FinishWritingObject && FinishWritingRelevance)
            {
                endedTask.Add(this.task_num);
                endedTask.Sort();
                isWriting = false;
                FinishWritingMotion = false;
                FinishWritingObject = false;
                FinishWritingRelevance = false;
            }


        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);

            if (mysqlConn != null)
                mysqlConn.Close();
            Application.Quit();
        }

    }

    private void CreateInsertQuery_relevance(ref string insertSql, string toolName)
    {
        string valueString =
            "(" +
                this.userID.ToString() + "," +
                this.task_num + "," +
                "'" + toolName + "'," +
                this.uniqueID.ToString() +
            ")";

        //Debug.Log(valueString);

        if (insertSql == string.Empty)
        {
            insertSql =
                "INSERT INTO " + MySqlSchemaName + "." + MySqlTableName_relevanceInfo + " " +
                "(user_id, task_id, tool_name, recording_id) " +
                "VALUES " + valueString;
        }
        else
        {
            insertSql += "," + valueString;
        }

    }

    private void CreateInsertQuery_userState(ref string insertSql)
    {
        string valueString =
            "(" +
                this.userID.ToString() + "," +
                this.task_num +
            ")";

        //Debug.Log(valueString);

        if (insertSql == string.Empty)
        {
            insertSql =
                "INSERT INTO " + MySqlSchemaName + "." + MySqlTableName_userState + " " +
                "(user_id, task_id) " +
                "VALUES " + valueString;
        }
        else
        {
            insertSql += "," + valueString;
        }
    }

    // シングルトン化する ------------------------
    //private Connect2MySQL instance = null;
    //void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(gameObject);

    //        string name = gameObject.name;
    //        gameObject.name = name + "(Singleton)";

    //        GameObject duplicater = GameObject.Find(name);
    //        if (duplicater != null)
    //        {
    //            Destroy(gameObject);
    //        }
    //        else
    //        {
    //            gameObject.name = name;
    //        }
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

}