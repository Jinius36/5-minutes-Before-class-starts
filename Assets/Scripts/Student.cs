using TMPro;
using UnityEngine;

public class Student : MonoBehaviour
{
    public int sex; // ���� ��: 0, ��: 1
    public int nowFloor; // ���� ��ġ�� ��, -1�� ���������� ž��
    public int goalFloor; // ��ǥ ��
    public int orderPlace; // ���ִ� ��ġ
    public SpriteRenderer hair;
    public SpriteRenderer hairBack;
    public SpriteRenderer face;
    public SpriteRenderer top;
    public SpriteRenderer pants;
    public Canvas bubble;
    public TextMeshProUGUI goalText;

    #region �巡�� �� ���
    Vector3 mousepoz;
    public SpriteRenderer[] draglayer; // �巡�� ���� �л��� ���� ���̰� �ϴ� ���̾� ������
    bool isDragging = false;
    bool isAtOutside = true;
    public static bool isOnSetting = false; // ����â�� �����ִ��� ����
    public static bool isElvMoving = false; // ���������Ͱ� �̵� ������ ����
    private void ResetPlace() // ���ǿ� �ȸ��� ��� �л��� ����ġ ��Ű�� �Լ�
    {
        transform.position = GameManager.Instance.place[orderPlace];
    }
    private void OnMouseDown()
    {
        if (!isOnSetting && !isElvMoving)
        {
            isDragging = true;
            bubble.gameObject.SetActive(true);
        }
    }
    private void OnMouseDrag()
    {
        for(int i=0; i<5; i++) // �巡�� ���� �л��� �ٸ� �л����� ���� ���̰� �� ���̾� ����
        {
            switch(i)
            {
                case 0:
                    draglayer[i].sortingLayerName = "DragHairBack";
                    break;
                case 1:
                    draglayer[i].sortingLayerName = "DragFace";
                    break;
                case 2:
                    draglayer[i].sortingLayerName = "DragHair";
                    break;
                case 3:
                    draglayer[i].sortingLayerName = "DragPants";
                    break;
                case 4:
                    draglayer[i].sortingLayerName = "DragTop";
                    break;
            }
        }
        bubble.sortingOrder = -1;
        if (isDragging)
        {
            mousepoz = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousepoz);
        }
    }
    private void OnMouseUp()
    {
        for (int i = 0; i < 5; i++) // �� ���̾� ������� ����
        {
            switch (i)
            {
                case 0:
                    draglayer[i].sortingLayerName = "HairBack";
                    break;
                case 1:
                    draglayer[i].sortingLayerName = "Face";
                    break;
                case 2:
                    draglayer[i].sortingLayerName = "Hair";
                    break;
                case 3:
                    draglayer[i].sortingLayerName = "Pants";
                    break;
                case 4:
                    draglayer[i].sortingLayerName = "Top";
                    break;
            }
        }
        bubble.sortingOrder = -2;
        isDragging = false;
        int p = 0; // �ڸ� �迭 �ε���
        if (transform.position.x <= -1.0f || transform.position.x >= 1.8f 
            || transform.position.y >= 3.0f || transform.position.y <= -5.0f) // �ʹ� ����ٸ� ����ġ
        {
            if (orderPlace > 2) // ���������Ϳ� �ִ� �л��� ��� ��� �� ��ǳ�� ��Ȱ��ȭ
            {
                bubble.gameObject.SetActive(false);
            }
            ResetPlace();
            return;
        }
        else // x ��ǥ ����
        {
            switch (transform.position.x)
            {
                case var n when n > -1.0f && n <= -0.175f:
                    p = 0;
                    break;

                case var n when n > -0.175f && n < 0.975f:
                    p = 1;
                    break;

                case var n when n >= 0.975f && n < 1.8f:
                    p = 2;
                    break;
            }
        }
        if (isAtOutside) // �ۿ��� ���ٸ�
        {
            if (goalFloor == GameManager.Instance.floor) // ���ϴ� ���� ������ ��Ȳ
            {
                GameManager.Instance.check_Place[orderPlace] = false;
                UIManager.Instance.addAttend();
                if (GameManager.Instance.goal == GameManager.Instance.attend)
                {
                    GameManager.Instance.StageClear();
                }
                Destroy(gameObject);
            }
            else // ���� ���� ���ϴ� ���� �ƴ�
            {
                ResetPlace();
                if (!isOnSetting && orderPlace > 2)
                {
                    bubble.gameObject.SetActive(false);
                }
                return;
            }
        }
        else // ���������Ϳ� ���ٸ�
        {
            if (transform.position.y > -2.65f) // �� ��
            {
                p += 3;
            }
            else // �Ʒ� ��
            {
                p += 6;
            }
            if (!GameManager.Instance.check_Place[p])
            {
                transform.position = GameManager.Instance.place[p];
                if (orderPlace > 2) // ���������Ϳ��� ���������ͷ� ��ġ�� ���
                {
                    GameManager.Instance.check_Place[orderPlace] = false; // ���� �ִ� �ڸ��� ����ٰ� ���
                }
                else // �ۿ��� ���������Ϳ� ��ġ�� ���
                {
                    GameManager.Instance.check_Out[nowFloor, orderPlace] = false; // ���� �ִ� �ڸ��� ����ٰ� ��� (�ܺ�)
                    GameManager.Instance.checkExistence(nowFloor); // �ش� ���� �л��� �����ִ� �� Ȯ�� �� ����ǥ ���� Ȥ�� ��Ȱ��ȭ
                }
                orderPlace = p; // �ű� �ڸ��� ����
                GameManager.Instance.check_Place[p] = true; // �ű� �ڸ��� �����Ͽ��ٰ� ���
                nowFloor = -1; // ���������Ϳ� ž���� ����
                if (!isOnSetting)
                {
                    bubble.gameObject.SetActive(false);
                }
            }
            else
            {
                ResetPlace();
                if (!isOnSetting)
                {
                    bubble.gameObject.SetActive(false);
                }
                return;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAtOutside = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isAtOutside = true;
    }
    #endregion

    void Start()
    {
        switch (goalFloor)
        {
            case 0:
                goalText.text = "B2";
                break;
            case 1:
                goalText.text = "L";
                break;
            default:
                goalText.text = $"{goalFloor - 1}";
                break;
        }
    }
}