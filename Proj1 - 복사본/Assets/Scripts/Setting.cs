using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    #region
    private static Setting _Instance;
    public static Setting Instance {  get { return _Instance; } }
    #endregion

    GameObject setting;
    public Button set_BTN;
    Button set_CloseBTN;
    Button exit_BTN;

    public void OpenSetting()
    {
        setting = Instantiate(Resources.Load("Popup_Setting") as GameObject);
        set_BTN.interactable = false;
        set_CloseBTN = GameObject.Find("CloseSetButton").GetComponent<Button>();
        set_CloseBTN.onClick.AddListener(CloseSetting);
        exit_BTN = GameObject.Find("ExitGameButton_Set").GetComponent<Button>();
        exit_BTN.onClick.AddListener(ExitGame);
    }

    public void CloseSetting()
    {
        Destroy(setting);
        set_BTN.interactable = true;
    }

    public void GameStart() // GameStartButton이 보유
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
