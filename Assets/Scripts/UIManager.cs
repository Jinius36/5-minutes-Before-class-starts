using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region 싱글톤
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
    TextMeshProUGUI[] textUIs = new TextMeshProUGUI[(int)textUI.MaxCount]; // 텍스트 UI들
    public Button[] elv_BTNs; // 엘리베이터 버튼들
    Sprite[] elv_BTN_Ons; // 엘리베이터 버튼 빨간색 이미지들
    Sprite[] elv_BTN_Offs; // 엘리베이터 버튼 회색 이미지들

    #region 표시 변경
    int hour = 0;
    int minute = 0;
    public Image[] floorPannels = new Image[2]; // 현재 층 표시 패널 [0]은 지하 2층의 B
    public RectTransform pannelTransform; // 지하 2층과 그 외 다른 층에서의 숫자 패널 위치 조정용
    Sprite[] numberPannels; // L 포함 숫자 패널 이미지들
    public void changeFloor() // 현재 층 표시 변경
    {
        if (GameManager.Instance.floor == 0) // 지하 2층인 경우
        {
            floorPannels[0].enabled = true; // 'B' 패널 활성화
            pannelTransform.anchoredPosition = new Vector3(189, 1165, 0); // 숫자 패널 오른쪽으로
            floorPannels[1].sprite = numberPannels[2];
        }
        else
        {
            floorPannels[0].enabled = false; // 'B' 패널 비활성화
            pannelTransform.anchoredPosition = new Vector3(134, 1165, 0); // 숫자 패널 왼쪽으로
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
    public void disableElv(int k = -1) // 엘베 버튼 비활성화
    {
        for (int i = 0; i < 11; i++)
        {
            elv_BTNs[i].interactable = false;
        }
        if (k != -1) // 매개변수가 있는 경우(특정 층으로 이동 버튼을 누른 경우) 해당 층 버튼만 빨간색 표시
        {
            elv_BTNs[k].image.sprite = elv_BTN_Ons[k];
        }
    }
    public void enableElv(int k = -1) // 엘베 버튼 활성화
    {
        for (int i = 0; i < 11; i++)
        {
            elv_BTNs[i].interactable = true;
        }
        if (k != -1) //매개변수가 있는 경우(특정 층으로 이동 버튼을 누른 경우) 해당 층 버튼 원상태로 복구
        {
            elv_BTNs[k].image.sprite = elv_BTN_Offs[k];
        }
    }
    #endregion

    #region 스테이지 테스트 버튼
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
        elv_BTN_Offs= Resources.LoadAll<Sprite>("Sprites/Elevator_Buttons_Off"); // 엘리베이터 버튼 이미지 불러오기
        elv_BTN_Ons = Resources.LoadAll<Sprite>("Sprites/Elevator_Buttons_On");
        numberPannels = Resources.LoadAll<Sprite>("Sprites/Number_Pannels"); // 숫자 패널 이미지 불러오기
        textUIs[(int)textUI.stageTime].text = $"{11 + GameManager.Instance.stageNum} : 30";
        textUIs[(int)textUI.Attend].text = $"   달성 : {GameManager.Instance.attend}";
    }
}