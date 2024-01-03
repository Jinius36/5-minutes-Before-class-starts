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
    Button retry_BTN;
    Button exit_BTN;
    AudioSource buttonSound;
    AudioClip setSound;

    public void OpenSetting()
    {
        GameManager.Instance.PlaySound2(GameManager.Instance.sounds[(int)GameManager.soundList.Button]);
        setting = Instantiate(Resources.Load("Popup_Setting") as GameObject);
        set_BTN.interactable = false;
        UIManager.Instance.disableElv();
        Student.isOnSetting = true;
        set_CloseBTN = GameObject.Find("CloseSetButton").GetComponent<Button>();
        set_CloseBTN.onClick.AddListener(CloseSetting);
        retry_BTN= GameObject.Find("RetryButton_Set").GetComponent<Button>();
        retry_BTN.onClick.AddListener(GameStart);
        exit_BTN = GameObject.Find("ExitGameButton_Set").GetComponent<Button>();
        exit_BTN.onClick.AddListener(ExitGame);
    }

    public void OpenSettingAtStart()
    {
        PlaySoundSet();
        setting = Instantiate(Resources.Load("Popup_Setting") as GameObject);
        retry_BTN = GameObject.Find("RetryButton_Set").GetComponent<Button>();
        retry_BTN.gameObject.SetActive(false);
        set_BTN.interactable = false;
        set_CloseBTN = GameObject.Find("CloseSetButton").GetComponent<Button>();
        set_CloseBTN.onClick.AddListener(CloseSettingAtStart);
        exit_BTN = GameObject.Find("ExitGameButton_Set").GetComponent<Button>();
        exit_BTN.onClick.AddListener(ExitGame);
    }

    public void CloseSetting()
    {
        PlaySoundSet();
        Destroy(setting);
        set_BTN.interactable = true;
        UIManager.Instance.enableElv();
        Student.isOnSetting = false;
    }

    public void CloseSettingAtStart()
    {
        PlaySoundSet();
        Destroy(setting);
        set_BTN.interactable = true;
    }

    public void GameStart() // GameStartButton이 보유
    {
        StartCoroutine(gameStart());
    }

    public void ExitGame()
    {
        StartCoroutine(exitGame());
    }

    void PlaySoundSet()
    {
        AudioClip clip = setSound;
        buttonSound.clip = clip;
        buttonSound.Play();
    }

    IEnumerator gameStart()
    {
        PlaySoundSet();
        Student.isOnSetting = false;
        yield return new WaitForSeconds(0.35f);
        SceneManager.LoadScene(1);
    }

    IEnumerator exitGame()
    {
        PlaySoundSet();
        Student.isOnSetting = false;
        yield return new WaitForSeconds(0.35f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    private void Start()
    {
        setSound = Resources.Load<AudioClip>("Sound/Button");
        buttonSound = gameObject.AddComponent<AudioSource>();
    }
}
