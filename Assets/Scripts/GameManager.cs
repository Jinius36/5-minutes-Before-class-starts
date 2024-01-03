using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    #region 싱글톤
    static GameManager _Instance;
    GameManager() { }
    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
    }
    public static GameManager Instance
    {
        get
        {
            return _Instance;
        }
    }
    #endregion

    public int stageNum; // 플레이어가 현재 도전해야 하는 스테이지
    public int stageTime; // 경과 시간
    public int limitTime; // 제한 시간
    public int goal = 0; // 스테이지 목표 인원
    public int attend = 0; // 해당 스테이지에서 목표층에 도달한 인원
    public int floor = 0; // 현재 층
    bool isSave; // 저장된 진행 상황이 있는지 확인
    [SerializeField] GameObject stage; // 현재 스테이지

    public enum placing // 위치 좌표 설정을 위한것들
    {
        left, mid, right, leftUp, midUp, rightUp, leftDown, midDown, rightDown, MaxCount
    }
    public bool[] check_Place; // 엘리베이터 내부에 쓰임
    public bool[,] check_Out; // 외부의 층마다 쓰임
    public Vector3[] place; // 좌표

    #region 사운드
    public enum soundList
    {
        StudentMove, ElevatorMove, Arrive, Button, ElvButton, Clear, Fail, MaxCount
    }
    public AudioClip[] sounds;
    AudioSource effectSound;
    AudioSource buttonSound;
    public void PlaySound(AudioClip clip)
    {
        effectSound.clip = clip;
        effectSound.Play();
    }
    public void PlaySound2(AudioClip clip)
    {
        buttonSound.clip = clip;
        buttonSound.Play();
    }
    #endregion

    #region 성공,실패
    public Button retry_BTN;
    public Button next_BTN;
    public Button exit_BTN_Clear;
    public Button exit_BTN_Failed;
    public GameObject clearScreen;
    public GameObject failedScreen;
    public TextMeshProUGUI clearText;
    public TextMeshProUGUI failedText;
    public bool checkGoal() // 목표 인원에 도달했으면 true
    {
        return goal == attend;
    }
    public void CallClear()
    {
        StageClear();
    }
    void StageClear() // checkGoal 수행 후 true이면 반환, stageNum을 +1, 다음 스테이지, 게임 종료 버튼 출현
    {
        PlaySound(sounds[(int)soundList.Clear]);
        if (stageNum == 9)
            stageNum = 1;
        else
            stageNum++;
        PlayerPrefs.SetInt("savedStage", stageNum);
        PlayerPrefs.Save();
        UIManager.Instance.disableElv();
        foreach (Tuple<GameObject, Student> student in this.students)
            Destroy(student.Item1);
        clearScreen.SetActive(true);
        next_BTN.gameObject.SetActive(true);
        exit_BTN_Clear.gameObject.SetActive(true);
        clearText.text = $"목표: {goal}\n달성: {attend}";
    }
    public void CallFailed()
    {
        StageFailed();
    }
    void StageFailed() // 제한시간 경과 시 실패, 스테이지 재시도 및 게임 종료 버튼 출현
    {
        PlaySound(sounds[(int)soundList.Fail]);
        UIManager.Instance.disableElv();
        foreach (Tuple<GameObject, Student> student in this.students)
            Destroy(student.Item1);
        failedScreen.SetActive(true);
        retry_BTN.gameObject.SetActive(true);
        exit_BTN_Failed.gameObject.SetActive(true);
        failedText.text = $"목표: {goal}\n달성: {attend}";
    }
    #endregion

    #region 엘리베이터
    GameObject door_Left; // 엘리베이터 문
    GameObject door_Right;
    SpriteRenderer background; // 배경
    Sprite[] backgrounds;
    Image arrow;
    Sprite[] arrowMotion = new Sprite[8];
    int arrowNum = 1;
    int eSpeed = 1;
    public void DoorClose()
    {
        door_Left.transform.DOMoveX(-0.458f, 0.5f);
        door_Right.transform.DOMoveX(1.297f, 0.5f);
    }
    public void DoorOpen()
    {
        door_Left.transform.DOMoveX(-2.278f, 0.5f);
        door_Right.transform.DOMoveX(3.13f, 0.5f);
    }
    IEnumerator ArrowUping(int f)
    {
        arrowNum = 0;
        arrow.gameObject.SetActive(true);
        arrow.sprite = arrowMotion[0];
        while (floor != f)
        {
            arrowNum++;
            if (arrowNum > 3)
                arrowNum = 0;
            yield return StartCoroutine(ArrowUp());
        }
        arrowNum = 0;
        arrow.gameObject.SetActive(false);
    }
    IEnumerator ArrowUp()
    {
        yield return new WaitForSeconds(0.3f);
        arrow.sprite = arrowMotion[arrowNum];
    }
    IEnumerator ArrowDowning(int f)
    {
        arrowNum = 0;
        arrow.gameObject.SetActive(true);
        arrow.sprite = arrowMotion[4];
        while (floor != f)
        {
            arrowNum++;
            if (arrowNum > 3)
                arrowNum = 0;
            yield return StartCoroutine(ArrowDown());
        }
        arrowNum = 0;
        arrow.gameObject.SetActive(false);
    }
    IEnumerator ArrowDown()
    {
        yield return new WaitForSeconds(0.3f);
        arrow.sprite = arrowMotion[arrowNum + 4];
    }
    IEnumerator UpFloor()
    {
        yield return new WaitForSeconds(0.5f);
        floor++;
        UIManager.Instance.changeFloor();
        UIManager.Instance.addStageTime(eSpeed);
    }
    IEnumerator DownFloor()
    {
        yield return new WaitForSeconds(0.5f);
        floor--;
        UIManager.Instance.changeFloor();
        UIManager.Instance.addStageTime(eSpeed);
    }
    public IEnumerator UpElevator(int f)
    {
        Student.isElvMoving = true;
        DoorClose();
        yield return new WaitForSeconds(eSpeed);
        //PlaySound(sounds[(int)soundList.ElevatorMove]);
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor != -1)
                student.Item1.SetActive(false);
        }
        StartCoroutine(ArrowUping(f));
        for (int i = floor; i < f; i++)
        {
            yield return StartCoroutine(UpFloor());
        }
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor == floor)
                student.Item1.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        PlaySound(sounds[(int)soundList.Arrive]);
        ActiveStudents(f);
        ChangeBack(f);
        DoorOpen();
        UIManager.Instance.enableElv(f);
        Student.isElvMoving = false;
        if (stageTime >= limitTime)
            CallFailed();
    }
    public IEnumerator DownElevator(int f)
    {
        Student.isElvMoving = true;
        DoorClose();
        yield return new WaitForSeconds(eSpeed);
        //PlaySound(sounds[(int)soundList.ElevatorMove]);
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor != -1)
                student.Item1.SetActive(false);
        }
        StartCoroutine(ArrowDowning(f));
        for (int i = floor; i > f; i--)
        { 
            yield return StartCoroutine(DownFloor());
        }
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor == floor)
                student.Item1.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        PlaySound(sounds[(int)soundList.Arrive]);
        ActiveStudents(f);
        ChangeBack(f);
        DoorOpen();
        UIManager.Instance.enableElv(f);
        Student.isElvMoving = false;
        if (stageTime >= limitTime)
            CallFailed();
    }
    void ChangeBack(int f)
    {
        background.sprite = backgrounds[f];
    }
    #endregion

    #region 학생 관리
    public List<Tuple<GameObject, Student>> students = new List<Tuple<GameObject, Student>>(); // 개별 학생 스폰, 관리용
    public Tuple<GameObject, Student> Spawn(int sex, int spawnFloor, int goalFloor, int orderPlace) // 학생 소환
    {
        GameObject studentObject = Instantiate(Resources.Load("Student")) as GameObject;
        Student studentComponent = studentObject.AddComponent<Student>();
        studentComponent.sex = sex;
        studentComponent.nowFloor = spawnFloor;
        studentComponent.goalFloor = goalFloor;
        studentComponent.orderPlace = orderPlace;
        studentObject.transform.position = place[orderPlace];
        studentObject.SetActive(false);
        return Tuple.Create(studentObject, studentComponent);
    }
    void ActiveStudents(int f) // 학생 활성화, 엘리베이터가 층 이동을 완료하면 실행
    {
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor == f)
                student.Item1.SetActive(true);
        }
    }
    #endregion

    #region 느낌표 관리
    GameObject[] existenceMarks;
    public void checkExistenceAll() // 전체 층 대상으로 학생 유무 판단하여 느낌표 활성화
    {
        for (int i = 0; i < 11; i++)
        {
            if (check_Out[i, 0] || check_Out[i, 1] || check_Out[i, 2])
                existenceMarks[i].SetActive(true);
        }
    }
    public void checkExistence(int f) // 단일 층 대상 버전
    {
        if (!check_Out[f, 0] && !check_Out[f, 1] && !check_Out[f, 2])
            existenceMarks[f].SetActive(false);
    }
    #endregion

    void Start()
    {
        isSave = PlayerPrefs.HasKey("savedStage"); // 스테이지 프리팹
        if (!isSave)
        {
            stageNum = 1;
            Debug.Log($"현재 스테이지: {stageNum}");
        }
        else
        {
            stageNum = PlayerPrefs.GetInt("savedStage");
            Debug.Log($"현재 스테이지: {stageNum}");
        }

        arrow = GameObject.Find("Arrow").GetComponent<Image>();
        arrow.gameObject.SetActive(false);
        for( int i = 0; i < 4; i++)
        {
            arrowMotion[i] = Resources.Load<Sprite>($"Sprites/Pannel_Images/Pannel_Up_{i}");
        }
        for (int i = 4; i < 8; i++)
        {
            arrowMotion[i] = Resources.Load<Sprite>($"Sprites/Pannel_Images/Pannel_Down_{i-4}");
        }

        effectSound = gameObject.AddComponent<AudioSource>();
        buttonSound = gameObject.AddComponent<AudioSource>();
        sounds = new AudioClip[(int)soundList.MaxCount];
        for (int i = 0; i < (int)soundList.MaxCount; i++)
        {
            soundList soundName = (soundList)i;
            sounds[i] = Resources.Load<AudioClip>($"Sound/{soundName.ToString()}");
        }

        check_Place = new bool[(int)placing.MaxCount]; // 학생의 처음 생성 위치 및 엘리베이터 위치에 학생이 있는지 여부
        check_Out = new bool[11,3];                    // 엘리베이터 외부 위치에 학생이 있는지 여부
        place = new Vector3[(int)placing.MaxCount];    // 좌표 설정

        door_Left = GameObject.Find("Door_Left"); // 엘리베이터 문 설정
        door_Right = GameObject.Find("Door_Right");

        for (int i = 0; i < (int)placing.MaxCount; i++) // false로 초기화
        {
            check_Place[i] = false;
        }
        for(int i = 0; i < 11; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                check_Out[i, j] = false;
            }
        }

        for (int i = 0; i < (int)placing.MaxCount; i++) // 좌표 설정
        {
            int index = i;
            float x = (index % 3) * 1.15f;
            float y = index / 3 * 2.5f;
            place[i] = new Vector3(-0.75f + x, 1.5f - y, 10);
        }

        existenceMarks = new GameObject[11]; // 층의 학생 존재 유무 느낌표, 비활성으로 초기화
        for (int i = 0; i < 11; i++)
        {
            existenceMarks[i] = GameObject.Find($"Mark_{i}");
            existenceMarks[i].SetActive(false);
        }

        background=GameObject.Find("Background").GetComponent<SpriteRenderer>();
        backgrounds=new Sprite[11];
        for (int i = 0;i < 11; i++)
        {
            backgrounds[i] = Resources.Load<Sprite>($"Sprites/Background_Images/Back{i}");
        }

        stage = Resources.Load<GameObject>($"Stage"); // 스테이지 불러오기
        Instantiate(stage);
    }
}