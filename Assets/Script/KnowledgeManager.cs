using System.Collections.Generic;
using TMPro; // TextMeshPro 사용을 위해 필수!
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class KnowledgeData
{
    public string knowledgeId;       // 지식 ID (예: "leg_crossing")
    public string title;             // ★ [화면에 표시할 제목] (예: "다리 꼬기")
    public Sprite unlockedSprite;    // 해금된 이미지
    [TextArea(2, 5)]
    public string description;       // 세부 설명
}

public class KnowledgeManager : MonoBehaviour
{
    public static KnowledgeManager Instance;

    [Header("UI References")]
    // 슬롯 리스트 (KnowledgeSlotComponent 스크립트가 붙은 프리팹들)
    public List<KnowledgeSlotComponent> knowledgeSlots;

    public GameObject detailPanel;       // 상세창 패널 (부모)
    public Image detailImage;            // 상세창 그림
    public TMP_Text detailText;          // 상세창 설명글 (작은 글씨)

    // ★ [여기가 추가된 부분!] 제목 텍스트를 넣을 빈칸을 만드는 거야.
    public TMP_Text detailTitleText;     // 상세창 제목 (초록색 큰 글씨)

    public TMP_Text unlockedCountText;   // 해금 개수 표시

    [Header("Knowledge Database")]
    public List<KnowledgeData> allKnowledges; // 데이터베이스 리스트

    private HashSet<string> unlockedIds = new HashSet<string>();
    private int _unlockedCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 지식 해금 함수
    public void UnlockKnowledge(string id)
    {
        if (unlockedIds.Contains(id)) return;
        unlockedIds.Add(id);

        // 데이터베이스에서 ID로 정보 찾기
        KnowledgeData data = allKnowledges.Find(k => k.knowledgeId == id);
        if (data == null)
        {
            Debug.LogWarning($"[KnowledgeManager] Unknown knowledge ID: {id}");
            return;
        }

        // 잠겨있는 슬롯을 찾아서 해금시키기
        foreach (var slot in knowledgeSlots)
        {
            // (참고: 슬롯의 초기 상태를 확인하는 조건문. 상황에 맞게 수정 가능)
            if (slot.slotImage.sprite.name.Contains("lock"))
            {
                // 슬롯에게 "너 이제 해금됐다!"라고 알림 (이미지, 제목 교체)
                slot.SetUnlocked(data.unlockedSprite, data.title, () => ShowDetail(data));

                // 해금 개수 증가 및 UI 업데이트
                _unlockedCount += 1;
                UpdateUnlockedCountUI();

                Debug.Log($"[KnowledgeManager] Unlocked: {id}");
                break;
            }
        }
    }

    // ★ [상세창 보여주는 함수] 여기가 핵심!
    public void ShowDetail(KnowledgeData data)
    {
        detailPanel.SetActive(true);          // 1. 창 켜고
        detailImage.sprite = data.unlockedSprite; // 2. 그림 바꾸고
        detailText.text = data.description;   // 3. 설명글 바꾸고

        if (detailTitleText != null)
        {
            detailTitleText.text = data.title;
        }
    }

    // 상세창 닫기
    public void CloseDetail()
    {
        detailPanel.SetActive(false);
    }

    // 해금 개수 UI 업데이트
    private void UpdateUnlockedCountUI()
    {
        if (unlockedCountText != null)
        {
            unlockedCountText.text = $"unlock : {_unlockedCount}";
        }
    }
}