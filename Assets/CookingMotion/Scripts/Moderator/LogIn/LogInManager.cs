using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogInManager : MonoBehaviour {
    
    private bool logInButton;
    private bool signUpMenuButton;
    private bool signUpButton;

	public InputField UserName;
    private static bool tutorial;
    
	public Toggle TutorialToggle;
	public Toggle RecordToggle;
	public Toggle PlaybackToggle;

    public Button SignUpButton;

    private Connect2MySQL sql;

    private GameObject UserError;

	// Use this for initialization
	void Start () {
        sql = FindObjectOfType<Connect2MySQL>();
        UserError = GameObject.Find("UserError");
        UserError.SetActive(false);

    }

	private void Update()
    {
        if (PlaybackToggle.isOn)
        {
            TutorialToggle.isOn = false;
            TutorialToggle.interactable = false;
            SignUpButton.interactable = false;
        }
        else
        {
            TutorialToggle.interactable = true;
            SignUpButton.interactable = true;
        }

        if (sql.currentPlayer() != null)
        {
            if (TutorialToggle.isOn) tutorial = true;
            else tutorial = false;

            if (RecordToggle.isOn) SceneManager.LoadScene("CookingMotionDemo");
            else if (PlaybackToggle.isOn) SceneManager.LoadScene("PlayBackMode");
        }

        if (Connect2MySQL.UserNameIsUsed) UserError.SetActive(true);
    }
    
    public void OnClickLogInButton()
    {
        sql.logIn(UserName.text);
    }

    public void OnClickSignUpButton()
    {
        sql.signUp(UserName.text);
    }

    public static bool GetToggleBool()
    {
        return tutorial;
    }

}
