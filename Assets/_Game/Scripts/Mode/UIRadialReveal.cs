using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIRadialReveal : MonoBehaviour
{
    [Header("Cài đặt Tốc độ & Giao diện")]
    [Tooltip("Tốc độ dâng lên của bột (0 -> 1)")]
    [SerializeField] private float fillSpeed = 0.3f; // Tăng nhẹ để test cho nhanh

    [Tooltip("Độ mềm của mép bột (Càng cao càng mượt)")]
    [SerializeField] private float edgeSoftness = 0.1f;

    [Header("QUAN TRỌNG: Cân bằng Lực (Logic)")]
    [Tooltip("Kéo ảnh KHUÔN (Grayscale) vào đây")]
    [SerializeField] private Sprite moldSprite;

    [Tooltip("Độ ưu tiên cho vùng LÕM (Sáng). 0.8 trở lên để lấp đầy hố trước.")]
    [Range(0f, 1f)]
    [SerializeField] private float moldWeight = 0.85f;

    [Tooltip("Độ ưu tiên cho khoảng cách từ tâm. 0.15 trở xuống để giảm hiệu ứng lan tròn.")]
    [Range(0f, 1f)]
    [SerializeField] private float distanceWeight = 0.15f;

    [Header("Cài đặt Noise (Ngẫu nhiên)")]
    [SerializeField] private float noiseAmount = 0.1f; // Thêm chút noise cho tự nhiên
    [SerializeField] private float noiseScale = 0.1f;

    [Header("Tham chiếu")]
    [SerializeField] private Image targetImage;

    // --- BIẾN HỆ THỐNG ---
    private Texture2D paintTex;
    private Color32[] currentPixels;
    private Color32[] batterPixels;
    private Color32[] moldPixels;

    private int texWidth, texHeight;
    private int moldWidth, moldHeight;
    private float[] fillThresholdMap; // Bản đồ độ sâu

    private float maxDist;
    private Coroutine coroutinePouring;

    // --- BIẾN TRẠNG THÁI GAME ---
    private float currentLevel = 0f; // Lưu trữ tiến độ đổ (không bị reset khi nhả tay)
    private bool isFinished = false; // Cờ kiểm tra chiến thắng

    void Start()
    {
        SetupTexture();
    }

    // --- CÁC HÀM GỌI TỪ EVENT TRIGGER (HOẶC UI BUTTON) ---
    public void StarPouring()
    {
        if (isFinished) return; // Thắng rồi thì không đổ nữa

        // Dừng coroutine cũ nếu đang chạy để tránh chồng chéo
        if (coroutinePouring != null) StopCoroutine(coroutinePouring);

        coroutinePouring = StartCoroutine(PouringProcess());
    }

    public void EndPouring()
    {
        if (coroutinePouring != null) StopCoroutine(coroutinePouring);
    }
    // -----------------------------------------------------

    void SetupTexture()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetImage.sprite == null || moldSprite == null)
        {
            Debug.LogError("Thiếu ảnh Target hoặc Mold!");
            return;
        }

        Sprite batterSprite = targetImage.sprite;

        // Kiểm tra Read/Write Enabled
        if (!batterSprite.texture.isReadable || !moldSprite.texture.isReadable)
        {
            Debug.LogError("LỖI: Hãy bật 'Read/Write Enabled' trong Import Settings của Texture!");
            return;
        }

        // 1. Lấy dữ liệu Pixel Target
        Rect batterRect = batterSprite.rect;
        texWidth = (int)batterRect.width;
        texHeight = (int)batterRect.height;
        batterPixels = GetPixelsFromSprite(batterSprite);

        // 2. Lấy dữ liệu Pixel Mold
        Rect moldRect = moldSprite.rect;
        moldWidth = (int)moldRect.width;
        moldHeight = (int)moldRect.height;
        moldPixels = GetPixelsFromSprite(moldSprite);

        // Khởi tạo mảng
        currentPixels = new Color32[batterPixels.Length];
        fillThresholdMap = new float[batterPixels.Length];

        int centerX = texWidth / 2;
        int centerY = texHeight / 2;
        maxDist = Mathf.Sqrt(centerX * centerX + centerY * centerY);

        // Random để mỗi lần chơi noise khác nhau chút
        float rndX = Random.Range(0f, 100f);
        float rndY = Random.Range(0f, 100f);

        paintTex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);

        // Biến để tìm giá trị nhỏ nhất (Fix lỗi delay)
        float minThresholdFound = float.MaxValue;

        // --- TÍNH TOÁN BẢN ĐỒ (HEAVY CALCULATION) ---
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int i = y * texWidth + x;

                // Mặc định ẩn pixel đi
                currentPixels[i] = batterPixels[i];
                currentPixels[i].a = 0;

                // A. Mapping UV để lấy màu từ khuôn (Hỗ trợ Atlas)
                float u = x / (float)texWidth;
                float v = y / (float)texHeight;

                int mX = Mathf.FloorToInt(u * moldWidth);
                int mY = Mathf.FloorToInt(v * moldHeight);
                mX = Mathf.Clamp(mX, 0, moldWidth - 1);
                mY = Mathf.Clamp(mY, 0, moldHeight - 1);
                int moldIndex = mY * moldWidth + mX;

                // B. Tính toán các chỉ số
                Color32 p = moldPixels[moldIndex];
                float brightness = (0.299f * p.r + 0.587f * p.g + 0.114f * p.b) / 255f;
                float heightNorm = 1.0f - brightness; // Đảo ngược: Sáng là sâu (ưu tiên), Tối là cao

                float dx = x - centerX;
                float dy = y - centerY;
                float distNorm = Mathf.Sqrt(dx * dx + dy * dy) / maxDist;

                float noise = Mathf.PerlinNoise((x * noiseScale) + rndX, (y * noiseScale) + rndY);
                float noiseNorm = (noise - 0.5f) * noiseAmount;

                // C. Công thức tổng hợp
                float threshold = (heightNorm * moldWeight) + (distNorm * distanceWeight) + noiseNorm;

                // Lưu tạm
                fillThresholdMap[i] = threshold;

                // Cập nhật min
                if (threshold < minThresholdFound) minThresholdFound = threshold;
            }
        }

        // --- BƯỚC CHUẨN HÓA (NORMALIZE) ---
        // Trừ tất cả cho minThresholdFound để điểm sâu nhất bắt đầu từ 0
        for (int i = 0; i < fillThresholdMap.Length; i++)
        {
            fillThresholdMap[i] -= minThresholdFound;
            if (fillThresholdMap[i] < 0) fillThresholdMap[i] = 0;
        }

        // Apply lần đầu (ẩn hết)
        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();

        // Tạo Sprite mới đè lên
        Vector2 pivot = new Vector2(batterSprite.pivot.x / batterSprite.rect.width, batterSprite.pivot.y / batterSprite.rect.height);
        targetImage.sprite = Sprite.Create(paintTex, new Rect(0, 0, texWidth, texHeight), pivot, batterSprite.pixelsPerUnit);

        // Reset trạng thái
        currentLevel = 0f;
        isFinished = false;
    }

    IEnumerator PouringProcess()
    {
        // Chạy cho đến khi đầy hẳn (1.3f cho an toàn)
        while (currentLevel < 1.3f)
        {
            currentLevel += fillSpeed * Time.deltaTime;

            // --- KIỂM TRA CHIẾN THẮNG ---
            // Ngưỡng 1.15f hoặc 1.2f là đủ để nhìn thấy đầy
            if (currentLevel >= 1.15f && !isFinished)
            {
                isFinished = true;
                OnLevelComplete();
            }

            ApplyWaterLevel(currentLevel);

            // Cập nhật texture
            paintTex.SetPixels32(currentPixels);
            paintTex.Apply();

            yield return null;
        }

        // Force hoàn thành nếu chưa kịp trigger
        if (!isFinished)
        {
            isFinished = true;
            OnLevelComplete();
        }

        // Đảm bảo hiển thị sắc nét pixel gốc khi xong
        System.Array.Copy(batterPixels, currentPixels, batterPixels.Length);
        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();

        isFinished = true; // Chốt chặn cuối cùng
    }

    void OnLevelComplete()
    {
        Debug.Log("<color=green>WIN: Bánh đã đổ đầy khuôn!</color>");

        // Gọi code của bạn ở đây. Tôi dùng dấu ? để tránh lỗi null nếu bạn chưa gán
        // Observer.OnChangeStage?.Invoke(); 
    }

    void ApplyWaterLevel(float level)
    {
        for (int i = 0; i < batterPixels.Length; i++)
        {
            // Bỏ qua pixel trong suốt của ảnh gốc
            if (batterPixels[i].a == 0) continue;

            float threshold = fillThresholdMap[i];

            if (level > threshold)
            {
                float delta = level - threshold;

                // Tính toán độ mờ viền (Anti-aliasing giả)
                float alphaPercent = delta / edgeSoftness;
                if (alphaPercent > 1f) alphaPercent = 1f;

                byte targetAlpha = (byte)(batterPixels[i].a * alphaPercent);

                // Chỉ cập nhật nếu alpha mới lớn hơn alpha hiện tại (để không bị mất hình khi noise dao động)
                if (targetAlpha > currentPixels[i].a)
                {
                    currentPixels[i] = batterPixels[i];
                    currentPixels[i].a = targetAlpha;
                }
            }
        }
    }

    // Hàm hỗ trợ cắt Pixel từ Atlas
    Color32[] GetPixelsFromSprite(Sprite sprite)
    {
        var rect = sprite.rect;
        int width = (int)rect.width;
        int height = (int)rect.height;
        Texture2D tex = sprite.texture;

        // Trường hợp ảnh đơn (Single)
        if (width == tex.width && height == tex.height)
            return tex.GetPixels32();

        // Trường hợp Atlas (Multiple)
        Color32[] fullPixels = tex.GetPixels32();
        Color32[] croppedPixels = new Color32[width * height];

        int startX = Mathf.FloorToInt(rect.x);
        int startY = Mathf.FloorToInt(rect.y);
        int atlasWidth = tex.width;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int atlasIndex = (startY + y) * atlasWidth + (startX + x);
                if (atlasIndex >= 0 && atlasIndex < fullPixels.Length)
                {
                    croppedPixels[y * width + x] = fullPixels[atlasIndex];
                }
            }
        }
        return croppedPixels;
    }
}