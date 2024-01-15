using TMPro;
using UnityEngine;

public class Student : MonoBehaviour
{
    public int sex; // 성별 남: 0, 여: 1
    public int nowFloor; // 현재 위치한 층, -1은 엘리베이터 탑승
    public int goalFloor; // 목표 층
    public int orderPlace; // 서있는 위치
    public SpriteRenderer hair;
    public SpriteRenderer hairBack;
    public SpriteRenderer face;
    public SpriteRenderer top;
    public SpriteRenderer pants;
    public Canvas bubble;
    public TextMeshProUGUI goalText;

    #region 드래그 앤 드롭
    Vector3 mousepoz;
    public SpriteRenderer[] draglayer; // 드래그 중인 학생이 먼저 보이게 하는 레이어 설정용
    bool isDragging = false;
    bool isAtOutside = true;
    public static bool isOnSetting = false; // 설정창이 켜져있는지 여부
    public static bool isElvMoving = false; // 엘리베이터가 이동 중인지 여부
    private void ResetPlace() // 조건에 안맞을 경우 학생을 원위치 시키는 함수
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
        for(int i=0; i<5; i++) // 드래그 중인 학생이 다른 학생보다 먼저 보이게 각 레이어 조정
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
        for (int i = 0; i < 5; i++) // 각 레이어 원래대로 조정
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
        int p = 0; // 자리 배열 인덱스
        if (transform.position.x <= -1.0f || transform.position.x >= 1.8f 
            || transform.position.y >= 3.0f || transform.position.y <= -5.0f) // 너무 벗어났다면 원위치
        {
            if (orderPlace > 2) // 엘리베이터에 있던 학생인 경우 드롭 시 말풍선 비활성화
            {
                bubble.gameObject.SetActive(false);
            }
            ResetPlace();
            return;
        }
        else // x 좌표 설정
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
        if (isAtOutside) // 밖에다 놨다면
        {
            if (goalFloor == GameManager.Instance.floor) // 원하는 층에 도착한 상황
            {
                GameManager.Instance.check_Place[orderPlace] = false;
                UIManager.Instance.addAttend();
                if (GameManager.Instance.goal == GameManager.Instance.attend)
                {
                    GameManager.Instance.StageClear();
                }
                Destroy(gameObject);
            }
            else // 내린 층이 원하는 층이 아님
            {
                ResetPlace();
                if (!isOnSetting && orderPlace > 2)
                {
                    bubble.gameObject.SetActive(false);
                }
                return;
            }
        }
        else // 엘리베이터에 놨다면
        {
            if (transform.position.y > -2.65f) // 윗 줄
            {
                p += 3;
            }
            else // 아랫 줄
            {
                p += 6;
            }
            if (!GameManager.Instance.check_Place[p])
            {
                transform.position = GameManager.Instance.place[p];
                if (orderPlace > 2) // 엘리베이터에서 엘리베이터로 배치한 경우
                {
                    GameManager.Instance.check_Place[orderPlace] = false; // 원래 있던 자리를 비웠다고 명시
                }
                else // 밖에서 엘리베이터에 배치한 경우
                {
                    GameManager.Instance.check_Out[nowFloor, orderPlace] = false; // 원래 있던 자리를 비웠다고 명시 (외부)
                    GameManager.Instance.checkExistence(nowFloor); // 해당 층에 학생이 남아있는 지 확인 후 느낌표 유지 혹은 비활성화
                }
                orderPlace = p; // 옮긴 자리로 조정
                GameManager.Instance.check_Place[p] = true; // 옮긴 자리를 차지하였다고 명시
                nowFloor = -1; // 엘리베이터에 탑승한 상태
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