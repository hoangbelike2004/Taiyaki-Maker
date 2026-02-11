using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // Nhớ import DOTween

public class UIHoldRadialReveal : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UIRadialReveal uIRadialReveal;

    private ParticleSystem particleSystem;
    private float rotateDuration = 0.5f; // Thời gian xoay

    // Biến để xác định vị trí tương đối: Nếu object này nằm bên PHẢI so với tâm thì isLeft = false (và ngược lại)
    // Logic của bạn: transform.position.x > target.x (Nghĩa là nó đang ở bên Phải) => isLeft = true? 
    // (Lưu ý: Tên biến isLeft của bạn hơi ngược, nhưng mình sẽ giữ nguyên logic của bạn để code chạy đúng ý bạn muốn)
    private bool isleft => transform.position.x > uIRadialReveal.transform.position.x;

    // Lưu góc xoay ban đầu để trả về
    private Quaternion originalRotation;

    void Awake()
    {
        // Lưu góc xoay mặc định lúc đầu game
        originalRotation = transform.rotation;
        particleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        Observer.OnEndPouringTopLayer += StopParticel;
        Observer.OnEndPouringBottomLayer += StopParticel;
        if (uIRadialReveal != null)
            uIRadialReveal.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        Observer.OnEndPouringTopLayer -= StopParticel;
        Observer.OnEndPouringBottomLayer -= StopParticel;
        // Khi tắt đi thì trả về góc cũ ngay lập tức để tránh lỗi hiển thị lần sau
        transform.rotation = originalRotation;

        if (uIRadialReveal != null)
            uIRadialReveal.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (uIRadialReveal != null)
        {
            // 1. Xác định góc xoay
            // Nếu isleft (bên phải) = true -> xoay 30
            // Nếu isleft (bên trái) = false -> xoay -30
            float targetAngle = isleft ? 30f : -30f;

            // 2. Dùng DOTween để xoay
            transform.DOKill(); // Hủy tween cũ
            transform.DOLocalRotate(new Vector3(0, 0, targetAngle), rotateDuration).OnComplete(() =>
            {
                particleSystem.Play();
            });

            // 3. Gọi hàm đổ nước cũ của bạn
            uIRadialReveal.StarPouring();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (uIRadialReveal != null)
        {
            // 1. Trả về góc xoay ban đầu (thường là 0)
            transform.DOKill();
            transform.DORotateQuaternion(originalRotation, rotateDuration);

            // 2. Dừng đổ nước
            uIRadialReveal.EndPouring();
            particleSystem.Stop();
        }
    }

    public void StopParticel()
    {
        particleSystem.Stop();
    }
}