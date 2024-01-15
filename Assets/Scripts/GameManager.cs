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
    #region �̱���
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

    public int stageNum; // �÷��̾ ���� �����ؾ� �ϴ� ��������
    public int stageTime; // ��� �ð�
    public int limitTime; // ���� �ð�
    public int goal = 0; // �������� ��ǥ �ο�
    public int attend = 0; // �ش� ������������ ��ǥ���� ������ �ο�
    public int floor = 0; // ���� ��
    public bool[] check_Place; // ���������� ������ �� ���� �ڸ����� �л��� ��ġ�ϴ��� Ȯ��
    public bool[,] check_Out; // �� ������ ���������� �ܺ� ��ġ�� �л��� �ִ��� ����
    public Vector3[] place; // �� 9�� �ڸ��� ��ǥ��

    #region ����,����
    public GameObject clearScreen; // ���� â
    public GameObject failedScreen; // ���� â
    public TextMeshProUGUI clearText; // ���� â �ؽ�Ʈ
    public TextMeshProUGUI failedText; // ���� â �ؽ�Ʈ
    public void StageClear() // ���ѽð� �ȿ� ��� �л��� �����ϸ� ����, stageNum++
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
        clearText.text = $"��ǥ: {goal}\n�޼�: {attend}";
    }
    public void StageFailed() // ��������Ʈ�� ������ ���� �������� �� ���ѽð� ����ߴٸ� ����
    {
        Setting.Instance.PlaySFX(Setting.Instance.sounds[(int)Setting.soundList.Fail]);
        UIManager.Instance.disableElv();
        foreach (Tuple<GameObject, Student> student in this.students)
        { 
            Destroy(student.Item1); 
        }
        failedScreen.SetActive(true);
        failedText.text = $"��ǥ: {goal}\n�޼�: {attend}";
    }
    #endregion

    #region ����������
    public GameObject door_Left; // ���������� ��
    public GameObject door_Right;
    public GameObject door_Left_Empty;
    public GameObject door_Right_Empty;
    float left_Close;
    float right_Close;
    float left_Open;
    float right_Open;
    public Image background; // �ܺι��
    Sprite[] backgrounds; // �ܺι�� �̹�����
    public Image arrow; // ���������� �̵� ȭ��ǥ
    Sprite[] arrowMotion; // ���������� �̵� ȭ��ǥ �̹�����
    int arrowNum; // ���������� �̵� ȭ��ǥ �̹��� �ε���
    int eSpeed = 1; // ���������Ͱ� �� �� �̵��� �� ���� ����ϴ� �ð�
    public void DoorClose() // �� ����
    {
        door_Left.transform.DOMoveX(left_Close, 0.5f);
        door_Right.transform.DOMoveX(right_Close, 0.5f);
    }
    public void DoorOpen() // �� �ݱ�
    {
        door_Left.transform.DOMoveX(left_Open, 0.5f);
        door_Right.transform.DOMoveX(right_Open, 0.5f);
    }
    IEnumerator ArrowUping(int f) // ���� ȭ��ǥ ���
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
    IEnumerator ArrowDowning(int f) // �Ʒ��� ȭ��ǥ ���
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
    IEnumerator ArrowUpDown() // ȭ��ǥ ��� �� �̹��� �ٲٱ�
    {
        yield return new WaitForSeconds(0.3f);
        arrow.sprite = arrowMotion[arrowNum];
    }
    IEnumerator ChangeFloor(int i) // �� �̵�, i�� 1�̸� ����, -1�̸� �Ʒ���
    {
        yield return new WaitForSeconds(0.5f);
        floor += i;
        UIManager.Instance.changeFloor();
        UIManager.Instance.addStageTime(eSpeed);
    }
    public IEnumerator UpElevator(int f) // ������ ������ ���� �̵�
    {
        Student.isElvMoving = true;
        DoorClose();
        yield return new WaitForSeconds(1);
        //PlaySound(sounds[(int)soundList.ElevatorMove]);
        foreach (Tuple<GameObject, Student> student in students) // ���������Ϳ� ž���� �л��� �����ϰ� ��Ȱ��ȭ
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
        foreach (Tuple<GameObject, Student> student in students) // ���� �� ���� �� �ش� ���� �ִ� �л��鸸 Ȱ��ȭ
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
    public IEnumerator DownElevator(int f) // ������ ������ �Ʒ��� �̵�
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

    #region �л� ����
    public List<Tuple<GameObject, Student>> students = new List<Tuple<GameObject, Student>>(); // ���� �л� ����, ������
    Sprite[] hairSprites; // �Ӹ� �̹�����
    Sprite[] backHairSprites; // ���� �Ӹ� 0~2�� �޸Ӹ� �̹�����
    Sprite[] faceSprites; // �� �̹�����
    Sprite[] topSprites; // ���� �̹�����
    Sprite[] pantsSprites; // ���� �̹�����
    List<Tuple<int, int, int, int, int>> avoidDuplication = new List<Tuple<int, int, int, int, int>>(); // �ߺ� ���� ���� ����Ʈ
    public Tuple<GameObject, Student> Spawn(int sex, int spawnFloor, int goalFloor, int orderPlace) // �л� ��ȯ
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

    #region ����ǥ ����
    public GameObject[] existenceMarks; // �л��� �����ϴ� ���� ��Ÿ���� ����ǥ
    public void checkExistenceAll() // ��ü �� ������� �л� ���� �Ǵ��Ͽ� ����ǥ Ȱ��ȭ
    {
        for (int i = 0; i < 11; i++)
        {
            if (check_Out[i, 0] || check_Out[i, 1] || check_Out[i, 2]) // �Ѹ��̶� ���� �� Ȱ��ȭ
            {
                existenceMarks[i].SetActive(true);
            }
        }
    }
    public void checkExistence(int f) // � ������ �л��� ���������ͷ� ��ġ���� ��, �ش� ���� �л��� �� �����ִ��� Ȯ��
    {
        if (!check_Out[f, 0] && !check_Out[f, 1] && !check_Out[f, 2]) // �����ִ� �л��� ���� �� ��Ȱ��ȭ
            existenceMarks[f].SetActive(false);
    }
    #endregion

    void Start()
    {
        if (!PlayerPrefs.HasKey("savedStage")) // �������� ���� ��Ȳ ������
        {
            stageNum = 1;
            Debug.Log($"���� ��������: {stageNum}");
        }
        else
        {
            stageNum = PlayerPrefs.GetInt("savedStage");
            Debug.Log($"���� ��������: {stageNum}");
        }

        left_Close = door_Left.transform.position.x;
        right_Close = door_Right.transform.position.x;
        left_Open = door_Left_Empty.transform.position.x;
        right_Open = door_Right_Empty.transform.position.x;

        check_Place = new bool[9];
        check_Out = new bool[11,3];
        place = new Vector3[9];
        for (int i = 0; i < 9; i++) // false�� �ʱ�ȭ
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
        for (int i = 0; i < 9; i++) // ��ǥ ����
        {
            int index = i;
            float x = (index % 3) * 1.15f;
            float y = index / 3 * 2.5f;
            place[i] = new Vector3(-0.705f + x, 1.5f - y, 10);
        }

        arrowMotion = Resources.LoadAll<Sprite>("Sprites/Arrow_Pannel"); // ���������� �̵� ȭ��ǥ �̹��� �ҷ�����
        backgrounds = Resources.LoadAll<Sprite>("Sprites/Background_Images"); // �ܺι�� �̹��� �ҷ�����
        hairSprites = Resources.LoadAll<Sprite>("Sprites/Human/Hair"); // �л� ���� �̹��� �ҷ�����
        backHairSprites = Resources.LoadAll<Sprite>("Sprites/Human/Female_Hair_Back");
        faceSprites = Resources.LoadAll<Sprite>("Sprites/Human/Face");
        topSprites = Resources.LoadAll<Sprite>("Sprites/Human/Top");
        pantsSprites = Resources.LoadAll<Sprite>("Sprites/Human/Pants");

        Student.isOnSetting = false;
        Student.isElvMoving = false;

        Instantiate(Resources.Load<GameObject>($"Stage")); // �������� �ҷ�����
    }
}