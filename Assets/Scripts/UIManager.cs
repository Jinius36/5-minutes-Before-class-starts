using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;

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
        ArrUI[(int)textUI.Goal].text = $"   ��ǥ : {GameManager.Instance.goal}";
        ArrUI[(int)textUI.Attend].text = $"   �޼� : {GameManager.Instance.attend}";
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
        ArrUI[(int)textUI.Attend].text = $"   �޼� : {GameManager.Instance.attend}";
    } // �޼� �ο� �߰�, ǥ�� ����

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
    } // ���� �� ǥ�ø� ����

    public void SetGoalUI()
    {
        ArrUI[(int)textUI.Goal].text = $"   ��ǥ : {GameManager.Instance.goal}";
    }
    #endregion


    #region ���������� ��ư
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
    } // ���� ��ư ��Ȱ��ȭ

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
    } // ���� ��ư Ȱ��ȭ
    #endregion
}
