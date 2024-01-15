using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using Rand = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region 싱글톤
    private static GameManager _Instance;
    public static GameManager Instance
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
    GameManager() { }
    #endregion

    public int stageNum; // 플레이어가 현재 도전해야 하는 스테이지
    public int stageTime; // 경과 시간
    public int limitTime; // 제한 시간
    public int goal = 0; // 스테이지 목표 인원
    public int attend = 0; // 해당 스테이지에서 목표층에 도달한 인원
    public int floor = 0; // 현재 층
    public bool[] check_Place; // 엘리베이터 내부의 총 여섯 자리마다 학생이 위치하는지 확인
    public bool[,] check_Out; // 각 층마다 엘리베이터 외부 위치에 학생이 있는지 여부
    public Vector3[] place; // 총 9개 자리의 좌표들

    #region 성공,실패
    public GameObject clearScreen; // 성공 창
    public GameObject failedScreen; // 실패 창
    public TextMeshProUGUI clearText; // 성공 창 텍스트
    public TextMeshProUGUI failedText; // 실패 창 텍스트
    public void StageClear() // 제한시간 안에 모든 학생이 도착하면 성공, stageNum++
    {
        Setting.Instance.PlaySFX(Setting.Instance.sounds[(int)Setting.soundList.Clear]);
        if (stageNum != 9)
        {
            stageNum++;
        }
        PlayerPrefs.SetInt("savedStage", stageNum);
        PlayerPrefs.Save();
        UIManager.Instance.disableElv();
        foreach (Tuple<GameObject, Student> student in this.students)
        {
            Destroy(student.Item1);
        }
        clearScreen.SetActive(true);
        clearText.text = $"목표: {goal}\n달성: {attend}";
    }
    public void StageFailed() // 엘리베이트가 지정한 층에 도착했을 때 제한시간 경과했다면 실패
    {
        Setting.Instance.PlaySFX(Setting.Instance.sounds[(int)Setting.soundList.Fail]);
        UIManager.Instance.disableElv();
        foreach (Tuple<GameObject, Student> student in this.students)
        { 
            Destroy(student.Item1); 
        }
        failedScreen.SetActive(true);
        failedText.text = $"목표: {goal}\n달성: {attend}";
    }
    #endregion

    #region 엘리베이터
    public GameObject door_Left; // 엘리베이터 문
    public GameObject door_Right;
    public GameObject door_Left_Empty;
    public GameObject door_Right_Empty;
    float left_Close;
    float right_Close;
    float left_Open;
    float right_Open;
    public Image background; // 외부배경
    Sprite[] backgrounds; // 외부배경 이미지들
    public Image arrow; // 엘리베이터 이동 화살표
    Sprite[] arrowMotion; // 엘리베이터 이동 화살표 이미지들
    int arrowNum; // 엘리베이터 이동 화살표 이미지 인덱스
    int eSpeed = 1; // 엘리베이터가 한 층 이동할 때 마다 경과하는 시간
    public void DoorClose() // 문 열기
    {
        door_Left.transform.DOMoveX(left_Close, 0.5f);
        door_Right.transform.DOMoveX(right_Close, 0.5f);
    }
    public void DoorOpen() // 문 닫기
    {
        door_Left.transform.DOMoveX(left_Open, 0.5f);
        door_Right.transform.DOMoveX(right_Open, 0.5f);
    }
    IEnumerator ArrowUping(int f) // 위쪽 화살표 모션
    {
        arrowNum = 4;
        arrow.gameObject.SetActive(true);
        arrow.sprite = arrowMotion[4];
        while (floor != f)
        {
            arrowNum++;
            if (arrowNum > 7)
            {
                arrowNum = 4;
            }
            yield return StartCoroutine(ArrowUpDown());
        }
        arrow.gameObject.SetActive(false);
    }
    IEnumerator ArrowDowning(int f) // 아래쪽 화살표 모션
    {
        arrowNum = 0;
        arrow.gameObject.SetActive(true);
        arrow.sprite = arrowMotion[0];
        while (floor != f)
        {
            arrowNum++;
            if (arrowNum > 3)
            {
                arrowNum = 0;
            }
            yield return StartCoroutine(ArrowUpDown());
        }
        arrow.gameObject.SetActive(false);
    }
    IEnumerator ArrowUpDown() // 화살표 모션 중 이미지 바꾸기
    {
        yield return new WaitForSeconds(0.3f);
        arrow.sprite = arrowMotion[arrowNum];
    }
    IEnumerator ChangeFloor(int i) // 층 이동, i가 1이면 위로, -1이면 아래로
    {
        yield return new WaitForSeconds(0.5f);
        floor += i;
        UIManager.Instance.changeFloor();
        UIManager.Instance.addStageTime(eSpeed);
    }
    public IEnumerator UpElevator(int f) // 지정한 층까지 위로 이동
    {
        Student.isElvMoving = true;
        DoorClose();
        yield return new WaitForSeconds(1);
        //PlaySound(sounds[(int)soundList.ElevatorMove]);
        foreach (Tuple<GameObject, Student> student in students) // 엘리베이터에 탑승한 학생들 제외하고 비활성화
        {
            if (student.Item2.nowFloor != -1)
            {
                student.Item1.SetActive(false);
            }
        }
        StartCoroutine(ArrowUping(f));
        for (int i = floor; i < f; i++)
        {
            yield return StartCoroutine(ChangeFloor(1));
        }
        foreach (Tuple<GameObject, Student> student in students) // 지정 층 도착 시 해당 층에 있는 학생들만 활성화
        {
            if (student.Item2.nowFloor == floor)
            {
                student.Item1.SetActive(true);
            }
        }
        yield return new WaitForSeconds(0.5f);
        Setting.Instance.PlaySFX(Setting.Instance.sounds[(int)Setting.soundList.Arrive]);
        background.sprite = backgrounds[f];
        DoorOpen();
        UIManager.Instance.enableElv(f);
        Student.isElvMoving = false;
        if (stageTime >= limitTime)
        {
            StageFailed();
        }
    }
    public IEnumerator DownElevator(int f) // 지정한 층까지 아래로 이동
    {
        Student.isElvMoving = true;
        DoorClose();
        yield return new WaitForSeconds(1);
        //PlaySound(sounds[(int)soundList.ElevatorMove]);
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor != -1)
            {
                student.Item1.SetActive(false);
            }
        }
        StartCoroutine(ArrowDowning(f));
        for (int i = floor; i > f; i--)
        {
            yield return StartCoroutine(ChangeFloor(-1));
        }
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor == floor)
            {
                student.Item1.SetActive(true);
            }
        }
        yield return new WaitForSeconds(0.5f);
        Setting.Instance.PlaySFX(Setting.Instance.sounds[(int)Setting.soundList.Arrive]);
        background.sprite = backgrounds[f];
        DoorOpen();
        UIManager.Instance.enableElv(f);
        Student.isElvMoving = false;
        if (stageTime >= limitTime)
        {
            StageFailed();
        }
    }
    #endregion

    #region 학생 관리
    public List<Tuple<GameObject, Student>> students = new List<Tuple<GameObject, Student>>(); // 개별 학생 스폰, 관리용
    Sprite[] hairSprites; // 머리 이미지들
    Sprite[] backHairSprites; // 여자 머리 0~2번 뒷머리 이미지들
    Sprite[] faceSprites; // 얼굴 이미지들
    Sprite[] topSprites; // 상의 이미지들
    Sprite[] pantsSprites; // 하의 이미지들
    List<Tuple<int, int, int, int, int>> avoidDuplication = new List<Tuple<int, int, int, int, int>>(); // 중복 방지 조합 리스트
    public Tuple<GameObject, Student> Spawn(int sex, int spawnFloor, int goalFloor, int orderPlace) // 학생 소환
    {
        GameObject studentObject = Instantiate(Resources.Load<GameObject>("Student"));
        Student studentComponent = studentObject.GetComponent<Student>();
        studentComponent.sex = sex;
        studentComponent.nowFloor = spawnFloor;
        studentComponent.goalFloor = goalFloor;
        studentComponent.orderPlace = orderPlace;
        studentObject.transform.position = place[orderPlace];
        int hair, face, top, pants;
        do
        {
            if (sex == 0)
            {
                hair = Rand.Range(0, 6);
                face = Rand.Range(0, 5);
                top = Rand.Range(0, 8);
                pants = Rand.Range(0, 3);
            }
            else
            {
                hair = Rand.Range(0, 6);
                face = Rand.Range(0, 5);
                top = Rand.Range(0, 8);
                pants = Rand.Range(0, 6);
            }
        } 
        while (avoidDuplication.Contains(Tuple.Create(sex, hair, face, top, pants)));
        if (sex == 0)
        {
            studentComponent.hair.sprite = hairSprites[hair + 6];
            studentComponent.face.sprite = faceSprites[face];
            studentComponent.top.sprite = topSprites[top];
            studentComponent.pants.sprite = pantsSprites[pants];
        }
        else
        {
            studentComponent.hair.sprite = hairSprites[hair];
            if (hair < 3)
            {
                studentComponent.hairBack.sprite = backHairSprites[hair];
            }
            studentComponent.face.sprite = faceSprites[face];
            studentComponent.top.sprite = topSprites[top];
            studentComponent.pants.sprite = pantsSprites[pants];
        }
        avoidDuplication.Add(Tuple.Create(sex, hair, face, top, pants));
        studentObject.SetActive(false);
        return Tuple.Create(studentObject, studentComponent);
    }
    #endregion

    #region 느낌표 관리
    public GameObject[] existenceMarks; // 학생이 존재하는 층에 나타나는 느낌표
    public void checkExistenceAll() // 전체 층 대상으로 학생 유무 판단하여 느낌표 활성화
    {
        for (int i = 0; i < 11; i++)
        {
            if (check_Out[i, 0] || check_Out[i, 1] || check_Out[i, 2]) // 한명이라도 존재 시 활성화
            {
                existenceMarks[i].SetActive(true);
            }
        }
    }
    public void checkExistence(int f) // 어떤 층에서 학생을 엘리베이터로 배치했을 때, 해당 층에 학생이 더 남아있는지 확인
    {
        if (!check_Out[f, 0] && !check_Out[f, 1] && !check_Out[f, 2]) // 남아있는 학생이 없을 시 비활성화
            existenceMarks[f].SetActive(false);
    }
    #endregion

    void Start()
    {
        if (!PlayerPrefs.HasKey("savedStage")) // 스테이지 진행 상황 프리팹
        {
            stageNum = 1;
            Debug.Log($"현재 스테이지: {stageNum}");
        }
        else
        {
            stageNum = PlayerPrefs.GetInt("savedStage");
            Debug.Log($"현재 스테이지: {stageNum}");
        }

        left_Close = door_Left.transform.position.x;
        right_Close = door_Right.transform.position.x;
        left_Open = door_Left_Empty.transform.position.x;
        right_Open = door_Right_Empty.transform.position.x;

        check_Place = new bool[9];
        check_Out = new bool[11,3];
        place = new Vector3[9];
        for (int i = 0; i < 9; i++) // false로 초기화
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
        for (int i = 0; i < 9; i++) // 좌표 설정
        {
            int index = i;
            float x = (index % 3) * 1.15f;
            float y = index / 3 * 2.5f;
            place[i] = new Vector3(-0.705f + x, 1.5f - y, 10);
        }

        arrowMotion = Resources.LoadAll<Sprite>("Sprites/Arrow_Pannel"); // 엘리베이터 이동 화살표 이미지 불러오기
        backgrounds = Resources.LoadAll<Sprite>("Sprites/Background_Images"); // 외부배경 이미지 불러오기
        hairSprites = Resources.LoadAll<Sprite>("Sprites/Human/Hair"); // 학생 착장 이미지 불러오기
        backHairSprites = Resources.LoadAll<Sprite>("Sprites/Human/Female_Hair_Back");
        faceSprites = Resources.LoadAll<Sprite>("Sprites/Human/Face");
        topSprites = Resources.LoadAll<Sprite>("Sprites/Human/Top");
        pantsSprites = Resources.LoadAll<Sprite>("Sprites/Human/Pants");

        Student.isOnSetting = false;
        Student.isElvMoving = false;

        Instantiate(Resources.Load<GameObject>($"Stage")); // 스테이지 불러오기
    }
}