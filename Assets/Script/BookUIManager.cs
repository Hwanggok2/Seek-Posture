using UnityEngine;

public class BookUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject bookCanvas;  // Book 창 전체 (지식 도감)
    public GameObject book;
    public GameObject detail_canvas;

    void Start()
    {
        if (bookCanvas != null)
        {
            bookCanvas.SetActive(false); // 시작 시 꺼둠
            detail_canvas.SetActive(false);
        }
    }
    // “지식 보기” 버튼에 연결
    public void OpenBook()
    {
        if (bookCanvas != null)
        {
            bookCanvas.SetActive(true);
            book.SetActive(false);
        }
            
    }

    // “닫기” 버튼에 연결
    public void CloseBook()
    {
        if (bookCanvas != null)
        {
            bookCanvas.SetActive(false);
            book.SetActive(true);
        }
    }

    public void Closedetail()
    {
        if (detail_canvas != null)
        {
            detail_canvas.SetActive(false);
        }
    }
}
