using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region �̱���
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

    #region ǥ�� ����
    public void addStageTime(int i)
    {
        GameManager.Instance.stageTime += i;
        ArrUI[(int)textUI.stageTime].text = $"{11 + GameManager.Instance.stageNum} : {30 + GameManager.Instance.stageTime}"; 
    } // �������� �ð� �߰�, ǥ�� ����

    public void addAttend()
    {
        GameManager.Instance.attend++;
        ArrUI[(int)textUI.Attend].text = $"Attend: {GameManager.Instance.attend}";
    } // �޼� �ο� �߰�, ǥ�� ����

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
    } // ���� �� ǥ�ø� ����
    #endregion


    #region ���������� ��ư
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
    } // ���� ��ư ��Ȱ��ȭ

    public void enableElv()
    {
        for (int i = 0; i < (int)elvButton.MaxCount; i++)
        {
            ArrButton[i].interactable = true;
        }
    } // ���� ��ư Ȱ��ȭ
    #endregion
}
