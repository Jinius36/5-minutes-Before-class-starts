using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Setting : MonoBehaviour
{
    #region 싱글톤
    private static Setting _Instance;
    public static Setting Instance 
    {  
        get 
        { 
            return _Instance; 
        } 
    }
    private void Awake()
    {
        //if(SceneManager.GetActiveScene().buildIndex == 0)
        //    DontDestroyOnLoad(bgmPlayer);
        if (_Instance == null)
        {
            _Instance = this;
        }
    }
    Setting() { }
    #endregion

    public GameObject setting; // 세팅창 자체 
    public Button set_BTN; // 세팅창 여는 버튼

    #region 설정창 열기, 닫기
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
        setting.SetActive(false);
        set_BTN.interactable = true;
        UIManager.Instance.enableElv();
        Student.isOnSetting = false;
        Student.isElvMoving = false;
    }
    public void CloseSettingAtStart()
    {
        PlaySFX(sounds[(int)soundList.Button]);
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
    #endregion

    #region 사운드
    public AudioSource sfxSound; // 게임내의 모든 버튼을 눌렀을 때 나오는 사운드
    public AudioSource sfxSound2; // 엘리베이터 이동, 학생들 이동 소리
    public AudioSource bgmSound; // 배경음악
    public enum soundList // 사운드 이름들
    {
        Arrive, Button, Clear, ElevatorButton, Fail, MainSound, SlideHow, MaxCount
    }
    public AudioClip[] sounds; // 사운드 리스트
    public AudioMixer sfxMixer; // 효과음 믹서
    public Slider sfxSlider; // 효과음 조절 슬라이드
    public AudioMixer bgmMixer; // 배경음 믹서
    public Slider bgmSlider; // 배경음 조절 슬라이드
    public Button toggleBGM; // 배경음 음소거 버튼
    public Button toggleSFX; // 효과음 음소거 버튼
    Sprite[] toggleBGM_images = new Sprite[2]; // 배경음 음소거 버튼 온오프 이미지
    Sprite[] toggleSFX_images = new Sprite[2]; // 효과음 음소거 버튼 온오프 이미지
    public void PlaySFX(AudioClip clip) // 효과음 재생
    {
        sfxSound.clip = clip;
        sfxSound.Play();
    }
    public void PlaySFX2(AudioClip clip) // 효과음 재생2
    {
        sfxSound2.clip = clip;
        sfxSound2.Play();
    }
    public void PlayBGM(AudioClip clip) // 배경음 재생
    {
        bgmSound.clip = clip;
        bgmSound.Play();
    }
    public void SFXControl() // 효과음 슬라이드 조절
    {
        float volume = sfxSlider.value;
        if (volume == -40)
        {
            sfxMixer.SetFloat("SFX", -80);
        }
        else
        {
            sfxMixer.SetFloat("SFX", volume);
        }
        PlayerPrefs.SetFloat("savedSFX", sfxSlider.value);
        PlayerPrefs.Save();
    }
    public void BGMControl() // 배경음 슬라이드 조절
    {
        float volume = bgmSlider.value;
        if (volume == -40)
        {
            bgmMixer.SetFloat("BGM", -80);
        }
        else
        {
            bgmMixer.SetFloat("BGM", volume);
        }
        PlayerPrefs.SetFloat("savedBGM", bgmSlider.value);
        PlayerPrefs.Save();
    }
    public void OnOffSFX() // 효과음 온오프
    {
        if(!sfxSound.mute)
        {
            sfxSound.mute = true;
            sfxSound2.mute = true;
            toggleSFX.image.sprite= toggleSFX_images[1];
            PlayerPrefs.SetInt("toggledSFX", 0);
            PlayerPrefs.Save();
        }
        else
        {
            sfxSound.mute = false;
            sfxSound2.mute = false;
            toggleSFX.image.sprite = toggleSFX_images[0];
            PlayerPrefs.SetInt("toggledSFX", 1);
            PlayerPrefs.Save();
        }
    }
    public void OnOffBGM() // 배경음 온오프
    {
        if (!bgmSound.mute)
        {
            bgmSound.mute = true;
            toggleBGM.image.sprite = toggleBGM_images[1];
            PlayerPrefs.SetInt("toggledBGM", 0);
            PlayerPrefs.Save();
        }
        else
        {
            bgmSound.mute = false;
            toggleBGM.image.sprite = toggleBGM_images[0];
            PlayerPrefs.SetInt("toggledBGM", 1);
            PlayerPrefs.Save();
        }
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
    public void OpenHowToPlay() // 게임 방법 창 열기
    {
        PlaySFX(sounds[(int)soundList.Button]);
        teduri.SetActive(true);
        btn_NextPage.SetActive(true);
        btn_CloseHow.SetActive(true);
        tuto.gameObject.SetActive(true);
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
        {
            btn_NextPage.SetActive(false);
        }
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
        {
            btn_PrevPage.SetActive(false);
        }
    }
    #endregion

    void Start()
    {
        sounds = Resources.LoadAll<AudioClip>("Sound"); // 사운드 불러오기
        toggleBGM_images[0] = Resources.Load<Sprite>("Sprites/Button_Images/Button_BGM"); // 음소거 버튼 이미지 불러오기
        toggleBGM_images[1] = Resources.Load<Sprite>("Sprites/Button_Images/Button_BGM_off");
        toggleSFX_images[0] = Resources.Load<Sprite>("Sprites/Button_Images/Button_SFX");
        toggleSFX_images[1] = Resources.Load<Sprite>("Sprites/Button_Images/Button_SFX_off");
        if (!PlayerPrefs.HasKey("savedSFX")) 
        {
            sfxSlider.value = -20;
        }
        else
        {
            sfxSlider.value = PlayerPrefs.GetFloat("savedSFX");
        }
        if (!PlayerPrefs.HasKey("savedBGM"))
        {
            bgmSlider.value = -20;
        }
        else
        {
            bgmSlider.value = PlayerPrefs.GetFloat("savedBGM");
        }
        if (!PlayerPrefs.HasKey("toggledBGM"))
        {
            PlayerPrefs.SetInt("toggledBGM", 1);
            PlayerPrefs.Save();
        }
        else
        {
            bool onBGM = (PlayerPrefs.GetInt("toggledBGM") == 1);
            bgmSound.mute = onBGM ? false : true;
            toggleBGM.image.sprite = toggleBGM_images[onBGM ? 0 : 1];
        }
        if (!PlayerPrefs.HasKey("toggledSFX"))
        {
            PlayerPrefs.SetInt("toggledSFX", 1);
            PlayerPrefs.Save();
        }
        else
        {
            bool onSFX = (PlayerPrefs.GetInt("toggledSFX") == 1);
            sfxSound.mute = onSFX ? false : true;
            sfxSound2.mute = onSFX ? false : true;
            toggleSFX.image.sprite = toggleSFX_images[onSFX ? 0 : 1];
        }

        PlayBGM(sounds[(int)soundList.MainSound]);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            tuto_Images[0] = Resources.Load<Sprite>("Sprites/Tut1");
            tuto_Images[1] = Resources.Load<Sprite>("Sprites/Tut2");
            tuto_Images[2] = Resources.Load<Sprite>("Sprites/Tut3");
        }
    }
}