using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        stageTime,
        Goal,
        Attend,
        Floor,

        MaxCount
    }

    public enum elvButton
    {
        B2,B1,F1,F2,F3,F4,F5,F6,F7,F8,F9,MaxCount
    }

    TextMeshProUGUI[] ArrUI = new TextMeshProUGUI[(int)textUI.MaxCount];
    Button[] ArrButton = new Button[(int)elvButton.MaxCount];

    void Start()
    {
        for(int i = 0; i < (int)textUI.MaxCount; i++)
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

        ArrUI[(int)textUI.stageTime].text = $"{11 + GameManager.Instance.stageNum} : 30";
        ArrUI[(int)textUI.Goal].text = $"Goal: {GameManager.Instance.goal}";
        ArrUI[(int)textUI.Attend].text = $"Attend: {GameManager.Instance.attend}";
        ArrUI[(int)textUI.Floor].text = "B2";
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
        ArrUI[(int)textUI.Attend].text = $"Attend: {GameManager.Instance.attend}";
    } // 달성 인원 추가, 표시 변경

    public void changeFloor()
    {
        if (GameManager.Instance.floor == 0 || GameManager.Instance.floor == 1)
        {
            ArrUI[(int)textUI.Floor].text = $"B{2 - GameManager.Instance.floor}";
        }

        else
        {
            ArrUI[(int)textUI.Floor].text = $"{GameManager.Instance.floor - 1}F";
        }
    } // 현재 층 표시만 변경
    #endregion


    #region 엘리베이터 버튼
    public void MoveElevator(int i)
    {
        disableElv();
        int f = GameManager.Instance.floor;
        if (f < i)
            StartCoroutine(GameManager.Instance.UpElevator(i));
        else
            StartCoroutine(GameManager.Instance.DownElevator(i));
    }

    public void disableElv()
    {
        for(int i = 0; i < (int)elvButton.MaxCount; i++)
        {
            ArrButton[i].interactable = false;
        }
    } // 엘베 버튼 비활성화

    public void enableElv()
    {
        for (int i = 0; i < (int)elvButton.MaxCount; i++)
        {
            ArrButton[i].interactable = true;
        }
    } // 엘베 버튼 활성화
    #endregion
}
