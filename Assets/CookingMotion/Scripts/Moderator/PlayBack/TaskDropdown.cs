using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class TaskDropdown : MonoBehaviour {

    public Dropdown task_dropdown;  //操作するオブジェクトの設定
    public Dropdown user_dropdown;

    private Connect2MySQL sql;
    private bool isSelecting = false;
    private List<string> TaskList = new List<string>();

    // Use this for initialization
    void Start () {
        sql = FindObjectOfType<Connect2MySQL>();

        if (task_dropdown)
        {
            task_dropdown.ClearOptions();
            List<string> list = new List<string>();

            list.Insert(0, "----");

            task_dropdown.AddOptions(list);  //新しく要素のリストを設定する
            task_dropdown.value = 0; //デフォルトを設定
            task_dropdown.interactable = false;
        }
    }

    public void OnValueChanged(int value)
    {
        task_dropdown.interactable = false;
        task_dropdown.ClearOptions();

        StartCoroutine(SelectTaskInfo(user_dropdown.options[value].text));
        
    }

    private IEnumerator SelectTaskInfo(string username)
    {
        isSelecting = true;
        
        Thread threadSelectTask = new Thread(new ParameterizedThreadStart(SetTaskDropdownList));
        threadSelectTask.Start(username);

        while (isSelecting)
        {
            yield return new WaitForSeconds(0.1f);
            task_dropdown.interactable = false;
            user_dropdown.interactable = false;
        }

        TaskList.Insert(0, "----");

        task_dropdown.AddOptions(TaskList);  //新しく要素のリストを設定する
        task_dropdown.value = 0; //デフォルトを設定

        task_dropdown.interactable = true;
        user_dropdown.interactable = true;
        isSelecting = false;

    }

    private void SetTaskDropdownList(object o)
    {
        string username = (string)o;
        TaskList = sql.GetFinishTaskList(username);

        isSelecting = false;
    }
}
