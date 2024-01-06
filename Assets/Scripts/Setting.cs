using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Setting : MonoBehaviour
{
    #region 싱글톤
    private static Setting _Instance;
    public static Setting Instance {  get { return _Instance; } }
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
    }
    #endregion

    public GameObject setting; // 세팅창 자체
    public Button set_BTN; // 세팅창 여는 버튼

    public void OpenSetting()
    {
        PlaySFX(sounds[(int)soundList.Button]);
        setting.SetActive(true);
        set_BTN.interactable = false;
        UIManager.Instance.disableElv();
        Student.isOnSetting = true;
    }
    public void OpenSettingAtStart()
    {
        PlaySFX(sounds[(int)soundList.Button]);
        setting.SetActive(true);
        set_BTN.interactable = false;
    }
    public void CloseSetting()
    {
        PlaySFX(sounds[(int)soundList.Button]);
        PlayerPrefs.SetFloat("savedSFX", sfxSlider.value);
        PlayerPrefs.Save();
        setting.SetActive(false);
        set_BTN.interactable = true;
        UIManager.Instance.enableElv();
        Student.isOnSetting = false;
    }
    public void CloseSettingAtStart()
    {
        PlaySFX(sounds[(int)soundList.Button]);
        PlayerPrefs.SetFloat("savedSFX", sfxSlider.value);
        PlayerPrefs.Save();
        setting.SetActive(false);
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
    IEnumerator gameStart()
    {
        PlaySFX(sounds[(int)soundList.Button]);
        Student.isOnSetting = false;
        yield return new WaitForSeconds(0.35f);
        SceneManager.LoadScene(1);
    }
    IEnumerator exitGame()
    {
        PlaySFX(sounds[(int)soundList.Button]);
        Student.isOnSetting = false;
        yield return new WaitForSeconds(0.35f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    #region 사운드
    public AudioSource sfxSound; // 게임내의 모든 버튼을 눌렀을 때 나오는 사운드
    //[SerializeField] AudioSource sfxSound2;
    public enum soundList
    {
        StudentMove, ElevatorMove, Arrive, Button, ElvButton, Clear, Fail, SlideHow, MaxCount
    }
    public AudioClip[] sounds;
    public AudioMixer sfxMixer;
    public Slider sfxSlider;
    bool isSavedSXF;
    public void PlaySFX(AudioClip clip)
    {
        sfxSound.clip = clip;
        sfxSound.Play();
    }
    public void PlayBGM(AudioClip clip)
    {

    }
    public void SFXControl()
    {
        float volume = sfxSlider.value;
        if (volume == -40) sfxMixer.SetFloat("SFX", -80);
        else sfxMixer.SetFloat("SFX", volume);
        
    }
    #endregion

    #region HowToPlay
    public GameObject teduri;
    public GameObject btn_NextPage;
    public GameObject btn_PrevPage;
    public GameObject btn_CloseHow;
    public Image tuto;
    Sprite[] tuto_Images = new Sprite[3];
    int nowPage = 0;
    public void OpenHowToPlay() // 게임 방법 창 열기, 시작 화면 HowToPlayButton이 보유
    {
        teduri.SetActive(true);
        btn_NextPage.SetActive(true);
        btn_CloseHow.SetActive(true);
        tuto.gameObject.SetActive(true);
        PlaySFX(sounds[(int)soundList.Button]);
    }
    public void CloseHowToPlay() // 게임 방법 창 닫기
    {
        PlaySFX(sounds[(int)soundList.Button]);
        teduri.SetActive(false);
        btn_NextPage.SetActive(false);
        btn_PrevPage.SetActive(false);
        btn_CloseHow.SetActive(false);
        tuto.gameObject.SetActive(false);
        nowPage = 0;
        tuto.sprite = tuto_Images[nowPage];
    }
    public void NextPage()
    {
        btn_PrevPage.SetActive(true);
        if (nowPage != 2)
        {
            PlaySFX(sounds[(int)soundList.SlideHow]);
            nowPage++;
            tuto.sprite = tuto_Images[nowPage];
        }
        if (nowPage == 2)
            btn_NextPage.SetActive(false);
    }
    public void PrevPage()
    {
        btn_NextPage.SetActive(true);
        if (nowPage != 0)
        {
            PlaySFX(sounds[(int)soundList.SlideHow]);
            nowPage--;
            tuto.sprite = tuto_Images[nowPage];
        }
        if (nowPage == 0)
            btn_PrevPage.SetActive(false);
    }
    #endregion
    void Start()
    {
        isSavedSXF = PlayerPrefs.HasKey("savedSFX");
        if (!isSavedSXF) 
        {
            sfxSlider.value = -20;
        }
        else
        {
            sfxSlider.value = PlayerPrefs.GetFloat("savedSFX");
        }

        sounds = new AudioClip[(int)soundList.MaxCount];
        for (int i = 0; i < (int)soundList.MaxCount; i++)
        {
            soundList soundName = (soundList)i;
            sounds[i] = Resources.Load<AudioClip>($"Sound/{soundName.ToString()}");
        }
        sfxSound = gameObject.AddComponent<AudioSource>();
        sfxSound.outputAudioMixerGroup = sfxMixer.FindMatchingGroups("Master")[0];
        //sfxSound2 = gameObject.AddComponent<AudioSource>();
        //sfxSound2.outputAudioMixerGroup = sfxMixer.FindMatchingGroups("Master")[0];

        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            tuto_Images[0] = Resources.Load<Sprite>("Sprites/Tut1");
            tuto_Images[1] = Resources.Load<Sprite>("Sprites/Tut2");
            tuto_Images[2] = Resources.Load<Sprite>("Sprites/Tut3");
        }
    }
}