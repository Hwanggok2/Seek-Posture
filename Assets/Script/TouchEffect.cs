using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TouchEffect : MonoBehaviour
{
    public Canvas canvas;
    public GameObject checkPrefab;
    public GameObject xPrefab;
    public Animator characterAnimator;

    public GameObject collider1;
    public GameObject collider2;
    public GameObject collider3;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;
            HandleTouch(pos);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 pos = Input.GetTouch(0).position;
            HandleTouch(pos);
        }
#endif
    }

    void HandleTouch(Vector2 screenPos)
    {
        // 터치 위치 → 월드 좌표로 변환
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        bool isCorrect = false;
        GameObject touchedObj = null;

        if (hit.collider != null)
        {
            touchedObj = hit.collider.gameObject;
            Debug.Log("[TouchEffect] Hit: " + touchedObj.name);

            if (hit.collider.CompareTag("Correct"))
                isCorrect = true;
        }

        // UI 표시
        ShowMark(screenPos, isCorrect);

        if (touchedObj == null) return;

        // 정답일 경우 캐릭터 애니메이션 진행
        if (isCorrect && characterAnimator != null)
        {
            Debug.Log("[TouchEffect] Trigger 'Next'");
            characterAnimator.SetTrigger("Next");

            // collider1 -> collider2,3 활성화
            if (isCorrect)
            {
                characterAnimator.SetTrigger("Next");

                // 예: collider 이름에 맞게 지식 ID 매핑
                string knowledgeId = "";
                if (touchedObj == collider1) knowledgeId = "leg_crossing";
                if (touchedObj == collider2) knowledgeId = "leaning_back";
                if (touchedObj == collider3) knowledgeId = "cross_arms";

                if (!string.IsNullOrEmpty(knowledgeId))
                {
                    KnowledgeManager.Instance.UnlockKnowledge(knowledgeId);
                }

                touchedObj.SetActive(false);
            }
        }
        else
        {
            // 오답 터치 시, X 표시만 나옴
        }
    }

    void ShowMark(Vector2 screenPos, bool correct)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPos
        );

        GameObject prefab = correct ? checkPrefab : xPrefab;
        GameObject mark = Instantiate(prefab, canvas.transform);
        mark.SetActive(true);
        mark.GetComponent<RectTransform>().anchoredPosition = localPos;

        StartCoroutine(RemoveAfterDelay(mark, 1f));
    }

    IEnumerator RemoveAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
