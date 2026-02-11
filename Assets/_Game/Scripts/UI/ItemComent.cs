using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemComent : GameUnit
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI txtCmt;

    private CanvasGroup canvasGroup;
    private RectTransform rect;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    public void SetData(CommentCardItem commentCardItem, Transform parent)
    {
        transform.SetParent(parent);

        // Reset scale và vị trí
        transform.localScale = Vector3.one;
        rect.anchoredPosition = Vector3.zero; // Vị trí xuất phát (ở dưới cùng)

        // Set nội dung
        if (commentCardItem != null)
        {
            icon.sprite = commentCardItem.icon;
            int rnd = Random.Range(0, commentCardItem.cmts.Count);
            txtCmt.text = commentCardItem.cmts[rnd];
        }

        // Hiệu ứng xuất hiện (Scale to lên)
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        // Mới sinh ra thì rõ nhất (100%)
        canvasGroup.alpha = 1f;
    }

    // Hàm quan trọng: Cập nhật vị trí Y và độ mờ Alpha
    public void UpdateState(float targetY, float targetAlpha, float duration)
    {
        rect.DOKill(); // Hủy tween cũ nếu có
        canvasGroup.DOKill();

        // Di chuyển đến vị trí mới
        rect.DOAnchorPosY(targetY, duration).SetEase(Ease.OutQuad);

        // Fade sang độ mờ mới
        canvasGroup.DOFade(targetAlpha, duration);
    }

    public void DespawnAnim(float duration)
    {
        // Bay lên thêm 1 chút và mờ hẳn đi
        rect.DOAnchorPosY(rect.anchoredPosition.y + 60f, duration);
        canvasGroup.DOFade(0, duration).OnComplete(() =>
        {
            SimplePool.Despawn(this);
        });
    }
}