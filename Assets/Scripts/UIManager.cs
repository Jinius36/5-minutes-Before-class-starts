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

    Image[] ArrFloor = new Image[2];
    RectTransform pannel;
    TextMeshProUGUI[] ArrUI = new TextMeshProUGUI[(int)textUI.MaxCount];
    Button[] ArrButton = new Button[(int)elvButton.MaxCount];

    void Start()
    {
        ArrFloor[0] = GameObject.Find("Floor_B").GetComponent<Image>();
        ArrFloor[1] = GameObject.Find("Floor").GetComponent<Image>();
        pannel = ArrFloor[1].GetComponent<RectTransform>();

        for (int i = 0; i < (int)textUI.MaxCount; i++)
        {
            textUI UIname = (textUI)i;
            ArrUI[i] = GameObject.Find(UIname.ToString()).GetComponent<TextMeshProUGUI>();
        }

        for (int i = 0; i < (int)elvButton.MaxCount; i++)
        {
            int index = i;
            elvButton eButton = (elvButton)index;
            ArrButton[i] = GameObject.Find(eButton.ToString()).GetComponent<Button>();
            ArrButton[i].onClick.RemoveAllListeners();
            ArrButton[i].onClick.AddListener(() => {MoveElevator(index); });
        }

        ArrFloor[0].sprite = Resources.Load<Sprite>("Sprites/Pannel_Images/Pannel_B") as Sprite;
        ArrFloor[1].sprite = Resources.Load<Sprite>("Sprites/Pannel_Images/Pannel_2") as Sprite;
        ArrUI[(int)textUI.stageTime].text = $"{11 + GameManager.Instance.stageNum} : 30";
        ArrUI[(int)textUI.Goal].text = $"   목표 : {GameManager.Instance.goal}";
        ArrUI[(int)textUI.Attend].text = $"   달성 : {GameManager.Instance.attend}";
    }

    #region 표시 변경
    public void addStageTime(int i)
    {
        GameManager.Instance.stageTime += i;
        ArrUI[(int)textUI.stageTime].text = $"{11 + GameManager.Instance.stageNum} : {30 + GameManager.Instance.stageTime}"; 
    } // 스테이지 시간 추가, 표시 변경

    public void addAttend()
    {
        GameManager.Instance.attend++;
        ArrUI[(int)textUI.Attend].text = $"   달성 : {GameManager.Instance.attend}";
    } // 달성 인원 추가, 표시 변경

    public void changeFloor() 
    {
        if (GameManager.Instance.floor == 0)
        {
            ArrFloor[0].enabled = true;
            pannel.anchoredPosition = new Vector3(189, 1165, 0);
            ArrFloor[0].sprite = Resources.Load<Sprite>("Sprites/Pannel_Images/Pannel_B") as Sprite;
            ArrFloor[1].sprite = Resources.Load<Sprite>("Sprites/Pannel_Images/Pannel_2") as Sprite;
        }
        else if (GameManager.Instance.floor == 1)
        {
            ArrFloor[0].enabled = false;
            pannel.anchoredPosition = new Vector3(134, 1165, 0);
            ArrFloor[1].sprite = Resources.Load<Sprite>("Sprites/Pannel_Images/Pannel_L") as Sprite;
        }
        else
        {
            ArrFloor[1].sprite = Resources.Load<Sprite>($"Sprites/Pannel_Images/Pannel_{GameManager.Instance.floor - 1}") as Sprite;
        }
    } // 현재 층 표시만 변경

    public void SetGoalUI()
    {
        ArrUI[(int)textUI.Goal].text = $"   목표 : {GameManager.Instance.goal}";
    }
    #endregion


    #region 엘리베이터 버튼
    public void MoveElevator(int i)
    {
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

    public void disableElv(int k = -1)
    {
        for(int i = 0; i < (int)elvButton.MaxCount; i++)
        {
            ArrButton[i].interactable = false;
        }
        if (k != -1)
        {
            ArrButton[k].image.sprite=Resources.Load<Sprite>($"Sprites/Button_Images/Button_{((elvButton)k).ToString()}") as Sprite;
        }
    } // 엘베 버튼 비활성화

    public void enableElv(int k = -1)
    {
        for (int i = 0; i < (int)elvButton.MaxCount; i++)
        {
            ArrButton[i].interactable = true;
        }
        if (k != -1)
        {
            ArrButton[k].image.sprite = Resources.Load<Sprite>($"Sprites/Button_Images/Button_{((elvButton)k).ToString()}_off") as Sprite;
        }
    } // 엘베 버튼 활성화
    #endregion
}
