using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // TextMeshPro 필수

public class Stage1Manager : MonoBehaviour
{
    [Header("기본 연결")]
    public Canvas canvas;
    public Animator characterAnimator; // 캐릭터 움직이게 하려고 필요함!

    [Header("피드백 프리팹 (Project 창에서 드래그)")]
    public GameObject checkPrefab;      // O 표시 프리팹
    public GameObject xPrefab;          // X 표시 프리팹

    [Header("정답 콜라이더 (Hierarchy 창에서 드래그)")]
    public GameObject collider1;
    public GameObject collider2;
    public GameObject collider3;

    [Header("UI - 진행도 & 메시지 (Hierarchy 창에서 드래그)")]
    public Image[] progressCircles;      // 하단 빈 동그라미 3개
    public Sprite filledCircleSprite;    // 빨간색 꽉 찬 동그라미 그림 파일
    public GameObject newKnowledgeBadge; // 책 옆 "새로운 지식!"
    public TMP_Text bottomMessageText;   // 하단 설명 텍스트

    [Header("UI - 클리어 화면")]
    public GameObject clearPanel;

    public GameObject book;

    // ★ [추가] 사운드 설정
    [Header("사운드 설정")]
    public AudioSource sfxAudioSource; // 효과음 전용 스피커 (Inspector에서 연결!)
    public AudioClip correctSound;     // 정답 소리 (띠링!)
    public AudioClip wrongSound;       // 오답 소리 (삐익!)
    public AudioClip clearSound;       // 클리어 소리 (빠밤!)

    private int currentProgress = 0;
    private Coroutine messageCoroutine;

    void Start()
    {
        if (newKnowledgeBadge != null && newKnowledgeBadge.activeSelf)
        {
            newKnowledgeBadge.SetActive(false);
        }

        if (bottomMessageText != null && bottomMessageText.gameObject.activeSelf)
        {
            bottomMessageText.gameObject.SetActive(false);
        }

        // 시작 시 클리어 패널 끄기
        if (clearPanel != null) clearPanel.SetActive(false);
    }

    void Update()
    {
        if (book.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;
            HandleTouch(pos);
        }
    }

    void HandleTouch(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        bool isCorrect = false;
        GameObject touchedObj = null;

        if (hit.collider != null)
        {
            if (hit.collider.gameObject == collider1 ||
                hit.collider.gameObject == collider2 ||
                hit.collider.gameObject == collider3)
            {
                isCorrect = true;
                touchedObj = hit.collider.gameObject;
            }
        }

        // 1. O / X 마크 표시
        ShowMark(screenPos, isCorrect);

        // ★ [추가] 정답/오답 소리 재생 로직
        if (sfxAudioSource != null)
        {
            if (isCorrect)
            {
                if (correctSound != null) sfxAudioSource.PlayOneShot(correctSound);
            }
            else
            {
                if (wrongSound != null) sfxAudioSource.PlayOneShot(wrongSound);
            }
        }

        // 정답일 때만 실행
        if (isCorrect && touchedObj != null)
        {
            // 2. 캐릭터 애니메이션
            if (characterAnimator != null) characterAnimator.SetTrigger("Next");

            // 3. 지식 해금 및 메시지 설정
            string kId = "";
            string msg = "";

            if (touchedObj == collider1)
            {
                kId = "leg_crossing";
                msg = "지식 획득! 다리 꼬기\n-척추 불균형 유발";
                if (collider2) collider2.SetActive(true);
                if (collider3) collider3.SetActive(true);
            }
            else if (touchedObj == collider2)
            {
                if (collider3.activeSelf == false)
                {
                    kId = "slouching";
                    msg = "지식 획득! 눕듯이 앉기\n-허리 디스크 위험";
                }
            }
            else if (touchedObj == collider3)
            {
                if (collider2.activeSelf == false)
                {
                    kId = "slouching";
                    msg = "지식 획득! 눕듯이 앉기\n-허리 디스크 위험";
                }
            }

            // ... collider3 처리 ...

            // 지식 매니저에게 해금 요청
            if (KnowledgeManager.Instance != null && kId != "")
            {
                KnowledgeManager.Instance.UnlockKnowledge(kId);
            }

            // 4. 진행도 원 채우기 & 메시지 띄우기
            UpdateProgressUI();
            if (messageCoroutine != null) StopCoroutine(messageCoroutine);
            messageCoroutine = StartCoroutine(ShowMessageRoutine(msg));

            // 5. 맞춘 콜라이더는 끄기
            touchedObj.SetActive(false);
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
        if (prefab != null)
        {
            GameObject mark = Instantiate(prefab, canvas.transform);
            mark.SetActive(true);
            mark.GetComponent<RectTransform>().anchoredPosition = localPos;
            StartCoroutine(RemoveAfterDelay(mark, 1f));
        }
    }

    // 진행도 원 빨갛게 바꾸는 함수
    void UpdateProgressUI()
    {
        if (currentProgress < progressCircles.Length)
        {
            progressCircles[currentProgress].sprite = filledCircleSprite;
            currentProgress++;

            if (currentProgress == 3)
            {
                StartCoroutine(ShowClearPanelRoutine());
            }
        }
    }

    // ★ [추가] 클리어 화면과 소리를 처리하는 코루틴
    IEnumerator ShowClearPanelRoutine()
    {
        // 클리어 소리 재생
        if (sfxAudioSource != null && clearSound != null)
        {
            sfxAudioSource.PlayOneShot(clearSound);
        }

        // 메시지를 읽을 수 있게 2초 정도 대기
        yield return new WaitForSeconds(0.7f);

        // 클리어 패널 활성화
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }
    }

    IEnumerator ShowMessageRoutine(string message)
    {
        if (bottomMessageText != null)
        {
            bottomMessageText.text = message;
            bottomMessageText.gameObject.SetActive(true);
        }
        if (newKnowledgeBadge != null)
        {
            if (currentProgress == 1 || currentProgress == 3)
            {
                newKnowledgeBadge.SetActive(true);
            }   
        }
        yield return new WaitForSeconds(3.5f);

        if (bottomMessageText != null) bottomMessageText.gameObject.SetActive(false);
        if (newKnowledgeBadge != null) newKnowledgeBadge.SetActive(false);
    }

    IEnumerator RemoveAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}