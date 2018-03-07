using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDropdown : MonoBehaviour {

    public Dropdown user_dropdown;

    private Connect2MySQL sql;

    // Use this for initialization
    void Start () {
        sql = FindObjectOfType<Connect2MySQL>();

        if (user_dropdown)
        {
            user_dropdown.ClearOptions();
            List<string> list = new List<string>();

            list = SetUserDropdownList();

            user_dropdown.AddOptions(list);  //新しく要素のリストを設定する
            if (list.Count == 2)
            {
                user_dropdown.value = 1;
            }
            else user_dropdown.value = 0; //デフォルトを設定
        }

    }

    List<string> SetUserDropdownList()
    {
        List<string> LIST = new List<string>();
        LIST = sql.GetUserList();
        LIST.Insert(0, "----");
        return LIST;
    }
}
