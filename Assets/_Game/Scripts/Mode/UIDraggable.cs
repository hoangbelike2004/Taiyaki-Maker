using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // Bắt buộc phải có

public class UIDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [Tooltip("Tỉ lệ thu nhỏ khi nhấn")]
    public Vector3 draggingScale = new Vector3(0.9f, 0.9f, 1f);

    [Tooltip("Thời gian animation (giây)")]
    public float duration = 0.2f;

    [Tooltip("Kiểu chuyển động khi quay về (VD: OutBack để nảy nhẹ)")]
    public Ease returnEase = Ease.OutBack;

    [Tooltip("Khoảng cách lệch so với ngón tay ngay khi nhấn.")]
    public Vector2 dragOffset = new Vector2(0f, 50f);

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private Vector2 startPosition; // Lưu vị trí ban đầu để quay về

    private bool isDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Lưu scale mặc định ban đầu (thường là 1,1,1)
        originalScale = transform.localScale;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setparent(RectTransform parent)
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform.anchoredPosition = parent.anchoredPosition;
        float rnd = Random.Range(0.8f, 1);
        transform.localScale = Vector3.one * rnd;
    }

    // 1. NHẤN VÀO: Thu nhỏ mượt mà
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;

        // Lưu vị trí xuất phát để tí nữa còn biết đường quay về
        startPosition = rectTransform.anchoredPosition;

        Observer.OnDraggableCake?.Invoke(true);

        // Hủy các tween cũ đang chạy dở (quan trọng để không bị giật)
        rectTransform.DOKill();

        // Animation thu nhỏ (Scale)
        rectTransform.DOScale(draggingScale, duration).SetEase(Ease.OutQuad);

        canvasGroup.blocksRaycasts = false;

        // Áp dụng Offset (Vẫn giữ nguyên để ngón tay không che mất vật)
        if (canvas != null)
        {
            rectTransform.anchoredPosition += dragOffset / canvas.scaleFactor;
        }
    }

    // 2. BẮT ĐẦU KÉO
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    // 3. ĐANG KÉO (Giữ nguyên logic để bám sát ngón tay)
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    // 4. THẢ TAY RA: Phóng to lại & Bay về chỗ cũ
    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        Observer.OnDraggableCake?.Invoke(false);
        isDragging = false;

        // Hủy tween cũ
        rectTransform.DOKill();

        // A. Animation phục hồi kích thước (nảy nhẹ nhờ Ease.OutBack)
        rectTransform.DOScale(originalScale, duration).SetEase(Ease.OutBack);

        // B. Animation bay về vị trí cũ (startPosition)
        // Dùng DOAnchorPos để di chuyển mượt mà về chỗ ban đầu
        rectTransform.DOAnchorPos(startPosition, 0.3f).SetEase(returnEase);
    }

    // 5. KẾT THÚC KÉO
    public void OnEndDrag(PointerEventData eventData)
    {
        // Logic đã xử lý ở OnPointerUp
        isDragging = false;
    }

    // Hàm bổ trợ: Nếu bạn thả trúng đích và KHÔNG muốn nó bay về chỗ cũ
    // Hãy gọi hàm này từ script nhận (DropZone)
    public void OnDropSuccess()
    {
        rectTransform.DOKill(); // Dừng việc bay về
        rectTransform.localScale = originalScale; // Trả lại scale
        // Tại đây bạn có thể snap nó vào vị trí mới nếu muốn
    }
}