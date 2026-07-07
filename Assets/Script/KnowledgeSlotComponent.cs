using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KnowledgeSlotComponent : MonoBehaviour
{
    public Image slotImage;
    public TMP_Text slotText;
    public Button slotButton;

    public void SetUnlocked(Sprite newSprite, string newTitle, UnityEngine.Events.UnityAction onClickAction)
    {
        // 1. 이미지 변경
        slotImage.sprite = newSprite;

        // 2. 텍스트 설정
        if (slotText != null)
        {
            slotText.text = newTitle;

            // ★ [색상 변경] Hex Code(#437A53) 사용하기
            Color customColor;
            if (ColorUtility.TryParseHtmlString("#437A53", out customColor))
            {
                slotText.color = customColor;
            }

            // ★ [크기 변경] (숫자를 원하는 만큼 적어!)
            slotText.fontSize = 40f;
            slotText.enableAutoSizing = false;
        }

        // 3. 버튼 기능 연결
        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(onClickAction);

        // 4. 슬롯 크기 조정
        GetComponent<RectTransform>().sizeDelta = new Vector2(300, 300);
    }
}