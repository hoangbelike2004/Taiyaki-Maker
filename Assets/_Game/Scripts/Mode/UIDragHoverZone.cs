using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections; // Bắt buộc để dùng Coroutine

public class UIDragHoverZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    [Tooltip("Thời gian lặp lại hành động (giây).")]
    public float repeatInterval = 0.5f;

    [Tooltip("Tag của UI được phép tương tác (để trống nếu nhận tất cả)")]
    public string targetTag = "DraggableUI";

    private DragEffects dragEffects;
    private Coroutine currentCoroutine;

    // --- Khi chuột mang vật đi VÀO vùng này ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Kiểm tra xem chuột có đang kéo vật gì không
        if (eventData.dragging)
        {
            GameObject draggedObj = eventData.pointerDrag;

            if (draggedObj != null)
            {
                // Kiểm tra Tag hợp lệ
                if (string.IsNullOrEmpty(targetTag) || draggedObj.CompareTag(targetTag))
                {
                    // Bắt đầu vòng lặp (Xóa cũ trước nếu có)
                    StopMyCoroutine();
                    currentCoroutine = StartCoroutine(RepeatProcessRoutine(draggedObj));
                }
            }
        }
    }

    // --- Khi chuột mang vật đi RA KHỎI vùng này ---
    public void OnPointerExit(PointerEventData eventData)
    {
        // Dừng vòng lặp ngay lập tức
        StopMyCoroutine();
    }

    // --- Vòng lặp xử lý ---
    IEnumerator RepeatProcessRoutine(GameObject draggedObj)
    {
        dragEffects = draggedObj.GetComponent<DragEffects>();
        if (dragEffects != null)
        {
            // Truyền vật đang kéo sang để bên kia tính toán vị trí Trái/Phải
            dragEffects.ProcessDropZone(gameObject);
        }
        while (true)
        {
            // 1. Đợi một khoảng thời gian trước khi xử lý
            yield return new WaitForSeconds(repeatInterval);

            // 2. Kiểm tra an toàn: nếu vật đang kéo bị hủy (đã bị xóa hết) hoặc null thì dừng
            if (draggedObj == null)
            {
                StopMyCoroutine();
                yield break;
            }
            // 3. Gọi trực tiếp hàm logic bên script DragEffects
            if (dragEffects != null)
            {
                // Truyền vật đang kéo sang để bên kia tính toán vị trí Trái/Phải
                dragEffects.ProcessDropZone(gameObject);
            }
        }
    }

    // Hàm tiện ích để dừng Coroutine an toàn
    private void StopMyCoroutine()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }
}