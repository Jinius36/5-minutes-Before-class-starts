using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;

public class UIManager : MonoBehaviour
{
    #region 싱글톤
    static UIManager instance;
    UIManager() { }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    enum textUI
    {
        stageTime, Goal, Attend, MaxCount
    }
    public enum elvButton
    {
        B2,L,F1,F2,F3,F4,F5,F6,F7,F8,F9,MaxCount
    }
    TextMeshProUGUI[] textUIs = new TextMeshProUGUI[(int)textUI.MaxCount];
    Button[] elv_BTNs = new Button[(int)elvButton.MaxCount];
    Sprite[] elv_BTN_Ons = new Sprite[(int)elvButton.MaxCount];
    Sprite[] elv_BTN_Offs = new Sprite[(int)elvButton.MaxCount];

    #region 표시 변경
    int hour = 0;
    int minute = 0;
    Image[] floorPannels = new Image[2];
    RectTransform pannelTransform;
    Sprite[] numberPannels = new Sprite[10];
    public void changeFloor() // 현재 층 표시만 변경
    {
        if (GameManager.Instance.floor == 0)
        {
            floorPannels[0].enabled = true;
            pannelTransform.anchoredPosition = new Vector3(189, 1165, 0);
            floorPannels[1].sprite = numberPannels[2];
        }
        else
        {
            floorPannels[0].enabled = false;
            pannelTransform.anchoredPosition = new Vector3(134, 1165, 0);
            floorPannels[1].sprite = numberPannels[GameManager.Instance.floor - 1];
        }
    }
    public void addStageTime(int i) // 스테이지 시간 추가, 표시 변경
    {
        GameManager.Instance.stageTime += i;
        if ((GameManager.Instance.stageTime + 30) % 60 == 0)
        {
            hour++;
            minute = GameManager.Instance.stageTime + 30;
        }
        textUIs[(int)textUI.stageTime].text = $"{11 + hour + GameManager.Instance.stageNum} : {30 - minute + GameManager.Instance.stageTime}";
        if (30 - minute + GameManager.Instance.stageTime == 0)
            textUIs[(int)textUI.stageTime].text += "0";
    } 
    public void addAttend() // 달성 인원 추가, 표시 변경
    {
        GameManager.Instance.attend++;
        textUIs[(int)textUI.Attend].text = $"   달성 : {GameManager.Instance.attend}";
    }
    public void SetGoalUI()
    {
        textUIs[(int)textUI.Goal].text = $"   목표 : {GameManager.Instance.goal}";
    }
    #endregion

    #region 엘리베이터 버튼
    public void MoveElevator(int i)
    {
        Setting.Instance.PlaySFX(Setting.Instance.sounds[(int)Setting.soundList.ElvButton]);
        int f = GameManager.Instance.floor;
        if (f != i)
        {
            disableElv(i);
            if (f < i)
                StartCoroutine(GameManager.Instance.UpElevator(i));
            else
                StartCoroutine(GameManager.Instance.DownElevator(i));
        }
    }
    public void disableElv(int k = -1) // 엘베 버튼 비활성화
    {
        for (int i = 0; i < (int)elvButton.MaxCount; i++)
        {
            elv_BTNs[i].interactable = false;
        }
        if (k != -1)
        {
            elv_BTNs[k].image.sprite = elv_BTN_Ons[k];
        }
    }
    public void enableElv(int k = -1) // 엘베 버튼 활성화
    {
        for (int i = 0; i < (int)elvButton.MaxCount; i++)
        {
            elv_BTNs[i].interactable = true;
        }
        if (k != -1)
        {
            elv_BTNs[k].image.sprite = elv_BTN_Offs[k];
        }
    }
    #endregion

    void Start()
    {
        floorPannels[0] = GameObject.Find("Floor_B").GetComponent<Image>();
        floorPannels[1] = GameObject.Find("Floor").GetComponent<Image>();
        pannelTransform = floorPannels[1].GetComponent<RectTransform>();

        for (int i = 0; i < (int)textUI.MaxCount; i++)
        {
            textUI UIname = (textUI)i;
            textUIs[i] = GameObject.Find(UIname.ToString()).GetComponent<TextMeshProUGUI>();
        }

        for (int i = 0; i < (int)elvButton.MaxCount; i++)
        {
            int index = i;
            elvButton eButton = (elvButton)index;
            elv_BTNs[i] = GameObject.Find(eButton.ToString()).GetComponent<Button>();
            elv_BTNs[i].onClick.RemoveAllListeners();
            elv_BTNs[i].onClick.AddListener(() => {MoveElevator(index); });
        }

        elv_BTN_Offs[0] = Resources.Load<Sprite>("Sprites/Button_Images/Button_B2_off");
        elv_BTN_Offs[1] = Resources.Load<Sprite>("Sprites/Button_Images/Button_L_off");
        elv_BTN_Ons[0] = Resources.Load<Sprite>("Sprites/Button_Images/Button_B2");
        elv_BTN_Ons[1] = Resources.Load<Sprite>("Sprites/Button_Images/Button_L");
        for(int i=2;i< (int)elvButton.MaxCount; i++)
        {
            elv_BTN_Offs[i] = Resources.Load<Sprite>($"Sprites/Button_Images/Button_F{i - 1}_off");
            elv_BTN_Ons[i] = Resources.Load<Sprite>($"Sprites/Button_Images/Button_F{i - 1}");
        }

        numberPannels[0] = Resources.Load<Sprite>("Sprites/Pannel_Images/Pannel_L");
        for (int i = 1; i < 10; i++)
        {
            numberPannels[i] = Resources.Load<Sprite>($"Sprites/Pannel_Images/Pannel_{i}");
        }
        
        floorPannels[0].sprite = Resources.Load<Sprite>("Sprites/Pannel_Images/Pannel_B");
        floorPannels[1].sprite = numberPannels[2];
        textUIs[(int)textUI.stageTime].text = $"{11 + GameManager.Instance.stageNum} : 30";
        textUIs[(int)textUI.Attend].text = $"   달성 : {GameManager.Instance.attend}";
    }
}