using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Spine.Unity; // Bắt buộc để dùng Coroutine

public class UIDragHoverZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Tag của UI được phép tương tác (để trống nếu nhận tất cả)")]
    public string targetTag = "DraggableUI";

    private DragEffects dragEffects;
    private Coroutine currentCoroutine;

    private SkeletonGraphic skeletonGraphic;
    private bool isEat = false;
    private void Awake()
    {
        skeletonGraphic = transform.parent.GetComponent<SkeletonGraphic>();
    }

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
        skeletonGraphic.AnimationState.SetAnimation(0, GameConstants.ANIMATION_SKELETON_EAT_1_1, false);
        yield return new WaitForSeconds(0.25f);

        skeletonGraphic.AnimationState.SetAnimation(0, GameConstants.ANIMATION_SKELETON_EAT_1_2, false);
        if (dragEffects != null)
        {
            // Truyền vật đang kéo sang để bên kia tính toán vị trí Trái/Phải
            dragEffects.ProcessDropZone(gameObject);
        }
        yield return new WaitForSeconds(skeletonGraphic.SkeletonData.FindAnimation(GameConstants.ANIMATION_SKELETON_EAT_1_2).Duration);
        while (true)
        {
            // 1. Đợi một khoảng thời gian trước khi xử lý
            skeletonGraphic.AnimationState.SetAnimation(0, GameConstants.ANIMATION_SKELETON_EAT_1_1, false);
            yield return new WaitForSeconds(0.25f);

            skeletonGraphic.AnimationState.SetAnimation(0, GameConstants.ANIMATION_SKELETON_EAT_1_2, false);
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
            yield return new WaitForSeconds(skeletonGraphic.SkeletonData.FindAnimation(GameConstants.ANIMATION_SKELETON_EAT_1_2).Duration);
        }
    }

    // Hàm tiện ích để dừng Coroutine an toàn
    private void StopMyCoroutine()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            StartCoroutine(CheckAnimationEat());
        }
    }
    IEnumerator CheckAnimationEat()
    {
        while (true)
        {
            if (IsAnimationPlaying(GameConstants.ANIMATION_SKELETON_EAT_1_2))
            {
                skeletonGraphic.AnimationState.SetAnimation(0, GameConstants.ANIMATION_SKELETON_IDLE_1, true);
                yield break;
            }
            yield return null;
        }
    }
    private void OnDraggableCake(bool isDrag)
    {
        if (skeletonGraphic == null || currentCoroutine != null) return;
        if (isDrag)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, GameConstants.ANIMATION_SKELETON_CRAVE_1, true);
        }
        else
        {
            skeletonGraphic.AnimationState.SetAnimation(0, GameConstants.ANIMATION_SKELETON_IDLE_1, true);
        }
    }
    public bool IsAnimationPlaying(string nameAnimation)
    {
        var track = skeletonGraphic.AnimationState.GetCurrent(0);

        if (track != null && track.Animation.Name == nameAnimation)
        {
            if (track.IsComplete)
            {
                return true;
            }
        }
        return false;
    }
    void OnEnable()
    {
        Observer.OnDraggableCake += OnDraggableCake;
        StopAllCoroutines();
        currentCoroutine = null;
        dragEffects = null;
        skeletonGraphic.AnimationState.SetAnimation(0, GameConstants.ANIMATION_SKELETON_IDLE_1, true);
    }

    void OnDisable()
    {
        Observer.OnDraggableCake -= OnDraggableCake;
    }
}