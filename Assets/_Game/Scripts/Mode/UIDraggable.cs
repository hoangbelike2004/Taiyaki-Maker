using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // Nhớ import thư viện này

public class UIDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [Tooltip("Tỉ lệ thu nhỏ khi nhấn (Ví dụ: 0.9, 0.9, 1)")]
    public Vector3 draggingScale = new Vector3(0.9f, 0.9f, 1f);

    [Tooltip("Thời gian animation (giây)")]
    public float duration = 0.2f;

    [Tooltip("Kiểu chuyển động khi quay về (VD: OutBack để nảy nhẹ)")]
    public Ease returnEase = Ease.OutBack;

    [Tooltip("Khoảng cách lệch so với ngón tay (để ngón tay không che mất vật).")]
    public Vector2 dragOffset = new Vector2(0f, 50f);

    // Các biến nội bộ
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private Vector2 startPosition; // Lưu vị trí xuất phát
    private bool isDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Lưu scale gốc lúc đầu game
        originalScale = transform.localScale;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        // Tìm Canvas cha để đảm bảo tính toán tọa độ đúng
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    // Hàm setup khi sinh ra từ Pool hoặc khởi tạo lại
    public void Setparent(RectTransform parent)
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform.anchoredPosition = parent.anchoredPosition;

        // Random kích thước một chút cho tự nhiên (như code cũ của bạn)
        float rnd = Random.Range(0.8f, 1f);
        transform.localScale = Vector3.one * rnd;
        originalScale = transform.localScale; // Cập nhật lại originalScale theo random mới
    }

    // --- 1. KHI NHẤN CHUỘT XUỐNG ---
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;

        // Lưu vị trí hiện tại làm điểm xuất phát để quay về
        startPosition = rectTransform.anchoredPosition;

        // Bắn sự kiện cho Observer (Skeleton animation...)
        Observer.OnDraggableCake?.Invoke(true);

        // Hủy các tween cũ để tránh xung đột
        rectTransform.DOKill();

        // Hiệu ứng thu nhỏ
        rectTransform.DOScale(draggingScale, duration).SetEase(Ease.OutQuad);

        // Cho phép Raycast xuyên qua để tìm thấy Slot bên dưới
        canvasGroup.blocksRaycasts = false;

        // --- QUAN TRỌNG: Cập nhật vị trí ngay lập tức ---
        UpdatePositionToMouse(eventData);
    }

    // --- 2. KHI BẮT ĐẦU KÉO ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    // --- 3. TRONG KHI KÉO ---
    public void OnDrag(PointerEventData eventData)
    {
        UpdatePositionToMouse(eventData);
    }

    // --- 4. KHI THẢ CHUỘT RA ---
    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        Observer.OnDraggableCake?.Invoke(false);
        isDragging = false;

        // Hủy tween cũ
        rectTransform.DOKill();

        // A. Trả lại kích thước gốc (nảy nhẹ)
        rectTransform.DOScale(originalScale, duration).SetEase(Ease.OutBack);

        // B. Bay về vị trí cũ (startPosition)
        rectTransform.DOAnchorPos(startPosition, 0.3f).SetEase(returnEase);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    // --- HÀM TÍNH TOÁN VỊ TRÍ CHUẨN ---
    // Giúp tâm vật thể luôn bám theo chuột bất kể bạn click vào rìa hay giữa
    private void UpdatePositionToMouse(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Lấy RectTransform của cha (để tính tọa độ local so với cha)
        RectTransform parentRect = rectTransform.parent as RectTransform;
        Vector2 localPoint;

        // Chuyển đổi vị trí màn hình (Screen) sang vị trí Local của cha (UI)
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera, // Camera UI (null nếu là Overlay)
            out localPoint))
        {
            // Gán vị trí mới = Vị trí chuột (local) + Offset cài đặt
            rectTransform.anchoredPosition = localPoint + dragOffset;
        }
    }

    // --- HÀM GỌI KHI THẢ TRÚNG ĐÍCH ---
    // Gọi hàm này từ script DropZone để ngăn vật bay về chỗ cũ
    public void OnDropSuccess()
    {
        rectTransform.DOKill(); // Dừng bay về
        rectTransform.localScale = originalScale; // Reset scale
        // Bạn có thể thêm code snap vào tâm Slot ở đây nếu muốn
    }
}