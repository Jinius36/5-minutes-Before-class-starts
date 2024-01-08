using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region �̱���
    private static UIManager _Instance;
    public static UIManager Instance
    {
        get
        {
            return _Instance;
        }
    }
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
    }
    UIManager() { }
    #endregion

    enum textUI
    {
        stageTime, Goal, Attend, MaxCount
    }
    TextMeshProUGUI[] textUIs = new TextMeshProUGUI[(int)textUI.MaxCount]; // �ؽ�Ʈ UI��
    public Button[] elv_BTNs; // ���������� ��ư��
    Sprite[] elv_BTN_Ons; // ���������� ��ư ������ �̹�����
    Sprite[] elv_BTN_Offs; // ���������� ��ư ȸ�� �̹�����

    #region ǥ�� ����
    int hour = 0;
    int minute = 0;
    public Image[] floorPannels = new Image[2]; // ���� �� ǥ�� �г� [0]�� ���� 2���� B
    public RectTransform pannelTransform; // ���� 2���� �� �� �ٸ� �������� ���� �г� ��ġ ������
    Sprite[] numberPannels; // L ���� ���� �г� �̹�����
    public void changeFloor() // ���� �� ǥ�� ����
    {
        if (GameManager.Instance.floor == 0) // ���� 2���� ���
        {
            floorPannels[0].enabled = true; // 'B' �г� Ȱ��ȭ
            pannelTransform.anchoredPosition = new Vector3(189, 1165, 0); // ���� �г� ����������
            floorPannels[1].sprite = numberPannels[2];
        }
        else
        {
            floorPannels[0].enabled = false; // 'B' �г� ��Ȱ��ȭ
            pannelTransform.anchoredPosition = new Vector3(134, 1165, 0); // ���� �г� ��������
            floorPannels[1].sprite = numberPannels[GameManager.Instance.floor - 1];
        }
    }
    public void addStageTime(int i) // �������� �ð� �߰�, ǥ�� ����
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
    public void addAttend() // �޼� �ο� �߰�, ǥ�� ����
    {
        GameManager.Instance.attend++;
        textUIs[(int)textUI.Attend].text = $"   �޼� : {GameManager.Instance.attend}";
    }
    public void SetGoalUI()
    {
        textUIs[(int)textUI.Goal].text = $"   ��ǥ : {GameManager.Instance.goal}";
    }
    #endregion

    #region ���������� ��ư
    public void MoveElevator(int i)
    {
        Setting.Instance.PlaySFX(Setting.Instance.sounds[(int)Setting.soundList.ElevatorButton]);
        int f = GameManager.Instance.floor;
        if (f != i)
        {
            disableElv(i);
            if (f < i)
            {
                StartCoroutine(GameManager.Instance.UpElevator(i));
            }
            else
            {
                StartCoroutine(GameManager.Instance.DownElevator(i));
            }
        }
    }
    public void disableElv(int k = -1) // ���� ��ư ��Ȱ��ȭ
    {
        for (int i = 0; i < 11; i++)
        {
            elv_BTNs[i].interactable = false;
        }
        if (k != -1) // �Ű������� �ִ� ���(Ư�� ������ �̵� ��ư�� ���� ���) �ش� �� ��ư�� ������ ǥ��
        {
            elv_BTNs[k].image.sprite = elv_BTN_Ons[k];
        }
    }
    public void enableElv(int k = -1) // ���� ��ư Ȱ��ȭ
    {
        for (int i = 0; i < 11; i++)
        {
            elv_BTNs[i].interactable = true;
        }
        if (k != -1) //�Ű������� �ִ� ���(Ư�� ������ �̵� ��ư�� ���� ���) �ش� �� ��ư �����·� ����
        {
            elv_BTNs[k].image.sprite = elv_BTN_Offs[k];
        }
    }
    #endregion

    #region �������� �׽�Ʈ ��ư
    public GameObject testBTNs;
    Button[] stageBTN;
    public void testStage(int k)
    {
        Student.isOnSetting = false;
        Student.isElvMoving = false;
        GameManager.Instance.stageNum = k;
        PlayerPrefs.SetInt("savedStage", k);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }
    #endregion

    void Start()
    {
        stageBTN = testBTNs.GetComponentsInChildren<Button>();
        foreach(Button button in stageBTN)
        {
            int buttonIndex= System.Array.IndexOf(stageBTN, button);
            button.onClick.AddListener(() => testStage(buttonIndex + 1));
        }
        for (int i = 0; i < (int)textUI.MaxCount; i++)
        {
            textUI UIname = (textUI)i;
            textUIs[i] = GameObject.Find(UIname.ToString()).GetComponent<TextMeshProUGUI>();
        }
        for (int i = 0; i < 11; i++)
        {
            int index = i;
            elv_BTNs[i].onClick.AddListener(() => MoveElevator(index));
        }
        elv_BTN_Offs= Resources.LoadAll<Sprite>("Sprites/Elevator_Buttons_Off"); // ���������� ��ư �̹��� �ҷ�����
        elv_BTN_Ons = Resources.LoadAll<Sprite>("Sprites/Elevator_Buttons_On");
        numberPannels = Resources.LoadAll<Sprite>("Sprites/Number_Pannels"); // ���� �г� �̹��� �ҷ�����
        textUIs[(int)textUI.stageTime].text = $"{11 + GameManager.Instance.stageNum} : 30";
        textUIs[(int)textUI.Attend].text = $"   �޼� : {GameManager.Instance.attend}";
    }
}