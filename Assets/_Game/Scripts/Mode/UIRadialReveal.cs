using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIRadialReveal : MonoBehaviour
{
    // ... (Các biến Header cũ giữ nguyên không đổi) ...
    [Header("Cài đặt Tốc độ")]
    [Tooltip("Tốc độ dâng lên của bột (0 -> 1)")]
    [SerializeField] private float fillSpeed = 0.2f;

    [Tooltip("Độ mềm của mép bột (Càng cao càng mượt)")]
    [SerializeField] private float edgeSoftness = 0.1f;

    [Header("QUAN TRỌNG: Cân bằng Lực")]
    [Tooltip("Kéo ảnh KHUÔN vào đây")]
    [SerializeField] private Sprite moldSprite;

    [Tooltip("Độ ưu tiên cho vùng LÕM (Sáng). Đặt CAO (vd: 0.8) để lấp đầy lỗ trước.")]
    [Range(0f, 1f)]
    [SerializeField] private float moldWeight = 0.85f;

    [Tooltip("Độ ưu tiên cho khoảng cách từ tâm. Đặt THẤP (vd: 0.15) để giảm bớt hiệu ứng lan từ tâm.")]
    [Range(0f, 1f)]
    [SerializeField] private float distanceWeight = 0.15f;

    [Header("Cài đặt Noise")]
    [SerializeField] private float noiseAmount = 0f;
    [SerializeField] private float noiseScale = 0.05f;

    [Header("Tham chiếu")]
    [SerializeField] private Image targetImage;

    // --- BIẾN HỆ THỐNG ---
    private Texture2D paintTex;
    private Color32[] currentPixels;
    private Color32[] batterPixels;
    private Color32[] moldPixels;

    private int texWidth, texHeight;
    private int moldWidth, moldHeight;
    private float[] fillThresholdMap;

    private float maxDist;
    private Coroutine coroutinePouring;

    // --- CÁC BIẾN MỚI THÊM VÀO ĐỂ CHECK WIN ---
    private float currentLevel = 0f; // Đưa ra ngoài để lưu tiến độ đổ
    private bool isFinished = false; // Biến kiểm tra xem đã thắng chưa

    void Start()
    {
        SetupTexture();
    }

    public void StarPouring()
    {
        // Nếu đã thắng rồi thì không cho đổ nữa
        if (isFinished) return;

        // Nếu đang chạy thì dừng lại trước khi chạy cái mới (tránh bị chồng lấn)
        if (coroutinePouring != null) StopCoroutine(coroutinePouring);

        coroutinePouring = StartCoroutine(PouringProcess());
    }

    public void EndPouring()
    {
        if (coroutinePouring != null) StopCoroutine(coroutinePouring);
    }

    void SetupTexture()
    {
        // ... (Giữ nguyên toàn bộ logic trong hàm SetupTexture của bạn) ...
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetImage.sprite == null || moldSprite == null) return;

        Sprite batterSprite = targetImage.sprite;
        if (!batterSprite.texture.isReadable || !moldSprite.texture.isReadable) return;

        Rect batterRect = batterSprite.rect;
        texWidth = (int)batterRect.width;
        texHeight = (int)batterRect.height;
        batterPixels = GetPixelsFromSprite(batterSprite);

        Rect moldRect = moldSprite.rect;
        moldWidth = (int)moldRect.width;
        moldHeight = (int)moldRect.height;
        moldPixels = GetPixelsFromSprite(moldSprite);

        currentPixels = new Color32[batterPixels.Length];
        fillThresholdMap = new float[batterPixels.Length];

        int centerX = texWidth / 2;
        int centerY = texHeight / 2;
        maxDist = Mathf.Sqrt(centerX * centerX + centerY * centerY);

        float rndX = Random.Range(0f, 100f);
        float rndY = Random.Range(0f, 100f);

        paintTex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int i = y * texWidth + x;
                currentPixels[i] = batterPixels[i];
                currentPixels[i].a = 0;

                float u = x / (float)texWidth;
                float v = y / (float)texHeight;
                int mX = Mathf.FloorToInt(u * moldWidth);
                int mY = Mathf.FloorToInt(v * moldHeight);
                mX = Mathf.Clamp(mX, 0, moldWidth - 1);
                mY = Mathf.Clamp(mY, 0, moldHeight - 1);
                int moldIndex = mY * moldWidth + mX;

                Color32 p = moldPixels[moldIndex];
                float brightness = (0.299f * p.r + 0.587f * p.g + 0.114f * p.b) / 255f;
                float heightNorm = 1.0f - brightness;
                float dx = x - centerX;
                float dy = y - centerY;
                float distNorm = Mathf.Sqrt(dx * dx + dy * dy) / maxDist;
                float noise = Mathf.PerlinNoise((x * noiseScale) + rndX, (y * noiseScale) + rndY);
                float noiseNorm = (noise - 0.5f) * noiseAmount;

                float threshold = (heightNorm * moldWeight) + (distNorm * distanceWeight) + noiseNorm;
                if (threshold < 0) threshold = 0;
                fillThresholdMap[i] = threshold;
            }
        }
        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();

        Vector2 pivot = new Vector2(batterSprite.pivot.x / batterSprite.rect.width, batterSprite.pivot.y / batterSprite.rect.height);
        targetImage.sprite = Sprite.Create(paintTex, new Rect(0, 0, texWidth, texHeight), pivot, batterSprite.pixelsPerUnit);
    }

    IEnumerator PouringProcess()
    {
        // Bỏ dòng "float currentLevel = 0f;" ở đây đi nhé, dùng biến toàn cục

        // Vòng lặp
        while (currentLevel < 1.3f)
        {
            currentLevel += fillSpeed * Time.deltaTime;

            // --- KIỂM TRA CHIẾN THẮNG Ở ĐÂY ---
            // 1.2f là ngưỡng an toàn để đảm bảo hình đã đầy hẳn
            if (currentLevel >= 1.2f && !isFinished)
            {
                isFinished = true;
                OnLevelComplete(); // Gọi hàm xử lý thắng
            }
            // -----------------------------------

            ApplyWaterLevel(currentLevel);
            paintTex.SetPixels32(currentPixels);
            paintTex.Apply();
            yield return null;
        }

        // Đoạn code dưới này chỉ chạy khi currentLevel >= 1.3f (quá đầy)
        // Nếu đã win rồi thì thôi, còn chưa win thì force win nốt
        if (!isFinished)
        {
            isFinished = true;
            OnLevelComplete();
        }

        System.Array.Copy(batterPixels, currentPixels, batterPixels.Length);
        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();
    }

    // Hàm xử lý khi chiến thắng
    void OnLevelComplete()
    {
        Observer.OnChangeStage?.Invoke();
    }

    void ApplyWaterLevel(float level)
    {
        // ... (Giữ nguyên logic cũ) ...
        for (int i = 0; i < batterPixels.Length; i++)
        {
            if (batterPixels[i].a == 0) continue;
            float threshold = fillThresholdMap[i];
            if (level > threshold)
            {
                float delta = level - threshold;
                float alphaPercent = delta / edgeSoftness;
                if (alphaPercent > 1f) alphaPercent = 1f;
                byte targetAlpha = (byte)(batterPixels[i].a * alphaPercent);
                if (targetAlpha > currentPixels[i].a)
                {
                    currentPixels[i] = batterPixels[i];
                    currentPixels[i].a = targetAlpha;
                }
            }
        }
    }

    Color32[] GetPixelsFromSprite(Sprite sprite)
    {
        // ... (Giữ nguyên logic cũ) ...
        var rect = sprite.rect;
        int width = (int)rect.width;
        int height = (int)rect.height;
        Texture2D tex = sprite.texture;
        if (width == tex.width && height == tex.height) return tex.GetPixels32();
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
                    croppedPixels[y * width + x] = fullPixels[atlasIndex];
            }
        }
        return croppedPixels;
    }
}