using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Setting : MonoBehaviour
{
    #region �̱���
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

    public GameObject setting; // ����â ��ü 
    public Button set_BTN; // ����â ���� ��ư

    #region ����â ����, �ݱ�
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
    public void GameStart() // GameStartButton�� ����
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
        Application.Quit(); // ���ø����̼� ����
#endif
    }
    #endregion

    #region ����
    public AudioSource sfxSound; // ���ӳ��� ��� ��ư�� ������ �� ������ ����
    public AudioSource sfxSound2; // ���������� �̵�, �л��� �̵� �Ҹ�
    public AudioSource bgmSound; // �������
    public enum soundList // ���� �̸���
    {
        Arrive, Button, Clear, ElevatorButton, Fail, MainSound, SlideHow, MaxCount
    }
    public AudioClip[] sounds; // ���� ����Ʈ
    public AudioMixer sfxMixer; // ȿ���� �ͼ�
    public Slider sfxSlider; // ȿ���� ���� �����̵�
    public AudioMixer bgmMixer; // ����� �ͼ�
    public Slider bgmSlider; // ����� ���� �����̵�
    public Button toggleBGM; // ����� ���Ұ� ��ư
    public Button toggleSFX; // ȿ���� ���Ұ� ��ư
    Sprite[] toggleBGM_images = new Sprite[2]; // ����� ���Ұ� ��ư �¿��� �̹���
    Sprite[] toggleSFX_images = new Sprite[2]; // ȿ���� ���Ұ� ��ư �¿��� �̹���
    public void PlaySFX(AudioClip clip) // ȿ���� ���
    {
        sfxSound.clip = clip;
        sfxSound.Play();
    }
    public void PlaySFX2(AudioClip clip) // ȿ���� ���2
    {
        sfxSound2.clip = clip;
        sfxSound2.Play();
    }
    public void PlayBGM(AudioClip clip) // ����� ���
    {
        bgmSound.clip = clip;
        bgmSound.Play();
    }
    public void SFXControl() // ȿ���� �����̵� ����
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
    public void BGMControl() // ����� �����̵� ����
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
    public void OnOffSFX() // ȿ���� �¿���
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
    public void OnOffBGM() // ����� �¿���
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
    public void OpenHowToPlay() // ���� ��� â ����
    {
        PlaySFX(sounds[(int)soundList.Button]);
        teduri.SetActive(true);
        btn_NextPage.SetActive(true);
        btn_CloseHow.SetActive(true);
        tuto.gameObject.SetActive(true);
    }
    public void CloseHowToPlay() // ���� ��� â �ݱ�
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
        sounds = Resources.LoadAll<AudioClip>("Sound"); // ���� �ҷ�����
        toggleBGM_images[0] = Resources.Load<Sprite>("Sprites/Button_Images/Button_BGM"); // ���Ұ� ��ư �̹��� �ҷ�����
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