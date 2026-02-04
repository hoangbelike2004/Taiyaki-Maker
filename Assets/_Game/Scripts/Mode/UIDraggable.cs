using UnityEngine;
using UnityEngine.EventSystems;

// Thêm IPointerDownHandler và IPointerUpHandler để bắt sự kiện nhấn/thả ngay lập tức
public class UIDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [Tooltip("Tỉ lệ thu nhỏ khi nhấn (Ví dụ: 0.9, 0.9, 1)")]
    public Vector3 draggingScale = new Vector3(0.9f, 0.9f, 1f);

    [Tooltip("Khoảng cách lệch so với ngón tay ngay khi nhấn.")]
    public Vector2 dragOffset = new Vector2(0f, 50f);

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;

    // Biến để kiểm tra xem người dùng có thực sự kéo hay chỉ click
    private bool isDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = transform.localScale;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // 1. Xử lý ngay khi vừa chạm tay vào (Chưa cần kéo)
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false; // Reset trạng thái

        // Lưu scale gốc (đề phòng)
        originalScale = rectTransform.localScale;

        // Thu nhỏ ngay lập tức
        rectTransform.localScale = draggingScale;

        // Tắt raycast để chuẩn bị kéo
        canvasGroup.blocksRaycasts = false;

        // --- QUAN TRỌNG: Áp dụng Offset NGAY LẬP TỨC ---
        if (canvas != null)
        {
            rectTransform.anchoredPosition += dragOffset / canvas.scaleFactor;
        }
    }

    // 2. Xác nhận là đang kéo
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    // 3. Di chuyển object
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    // 4. Khi thả tay ra (Dù là đã kéo hay chưa kéo đều chạy hàm này)
    public void OnPointerUp(PointerEventData eventData)
    {
        // Khôi phục kích thước
        rectTransform.localScale = originalScale;
        canvasGroup.blocksRaycasts = true;

        // --- XỬ LÝ LOGIC TRẢ VỊ TRÍ ---
        // Nếu người chơi chỉ NHẤN mà KHÔNG KÉO (click nhầm), 
        // ta cần trừ đi cái offset đã cộng lúc đầu để nó về chỗ cũ.
        if (!isDragging && canvas != null)
        {
            rectTransform.anchoredPosition -= dragOffset / canvas.scaleFactor;
        }

        // Reset biến
        isDragging = false;
    }

    // Hàm này để đảm bảo an toàn cho logic của Unity (thường đi kèm OnBeginDrag)
    public void OnEndDrag(PointerEventData eventData)
    {
        // Logic phục hồi đã xử lý ở OnPointerUp rồi nên ở đây để trống hoặc gọi lại cho chắc cũng được
        isDragging = false;
    }
}