using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIBatterPourEffect_LevelFill : MonoBehaviour
{
    [Header("Cài đặt Tốc độ")]
    [Tooltip("Tốc độ dâng lên của bột (0 -> 1)")]
    [SerializeField] private float fillSpeed = 0.5f;

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

    // Mảng lưu "Thời điểm được lấp đầy" của từng pixel
    // Giá trị từ 0.0 (lấp ngay lập tức) đến 1.0 (lấp sau cùng)
    private float[] fillThresholdMap;

    private int texWidth, texHeight;
    private int centerX, centerY;
    private bool isPouring = false;
    private float maxDist; // Đường chéo dài nhất

    void Start()
    {
        SetupTexture();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isPouring)
        {
            isPouring = true;
            StartCoroutine(PouringProcess());
        }
    }

    void SetupTexture()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetImage.sprite == null || moldSprite == null)
        {
            Debug.LogError("Thiếu ảnh Target hoặc Mold!");
            return;
        }

        Sprite batterSprite = targetImage.sprite;
        if (!batterSprite.texture.isReadable || !moldSprite.texture.isReadable)
        {
            Debug.LogError("Chưa bật Read/Write Enabled!");
            return;
        }

        Rect rect = batterSprite.rect;
        texWidth = (int)rect.width;
        texHeight = (int)rect.height;
        centerX = texWidth / 2;
        centerY = texHeight / 2;

        // Tính đường chéo dài nhất từ tâm ra góc
        maxDist = Mathf.Sqrt(centerX * centerX + centerY * centerY);

        paintTex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
        batterPixels = GetPixelsFromSprite(batterSprite);
        moldPixels = GetPixelsFromSprite(moldSprite); // Giả định cùng size

        currentPixels = new Color32[batterPixels.Length];
        fillThresholdMap = new float[batterPixels.Length];

        float rndX = Random.Range(0f, 100f);
        float rndY = Random.Range(0f, 100f);

        // --- TÍNH TOÁN BẢN ĐỒ LẤP ĐẦY (PRE-CALCULATION) ---
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int i = y * texWidth + x;

                // 1. Ẩn pixel ban đầu
                currentPixels[i] = batterPixels[i];
                currentPixels[i].a = 0;

                // 2. Tính toán các yếu tố
                // A. Khoảng cách chuẩn hóa (0 ở tâm -> 1 ở rìa)
                float dx = x - centerX;
                float dy = y - centerY;
                float distNorm = Mathf.Sqrt(dx * dx + dy * dy) / maxDist;

                // B. Độ cao từ khuôn (Lấy độ sáng)
                // Pixel sáng (1.0) -> Là hố sâu -> Cần lấp SỚM (Height = 0)
                // Pixel tối (0.0) -> Là chỗ cao -> Cần lấp MUỘN (Height = 1)
                Color32 p = (i < moldPixels.Length) ? moldPixels[i] : new Color32(0, 0, 0, 0);
                float brightness = (0.299f * p.r + 0.587f * p.g + 0.114f * p.b) / 255f;
                float heightNorm = 1.0f - brightness;

                // C. Noise (để mép không bị đều tăm tắp)
                float noise = Mathf.PerlinNoise((x * noiseScale) + rndX, (y * noiseScale) + rndY);
                float noiseNorm = (noise - 0.5f) * noiseAmount;

                // 3. TỔNG HỢP: QUYẾT ĐỊNH THỜI ĐIỂM FILL
                // Công thức: (Độ cao * Trọng số) + (Khoảng cách * Trọng số)
                // Nếu MoldWeight cao -> Height quyết định tất cả.
                float threshold = (heightNorm * moldWeight) + (distNorm * distanceWeight) + noiseNorm;

                // Clamp giá trị
                if (threshold < 0) threshold = 0;

                fillThresholdMap[i] = threshold;
            }
        }

        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();

        targetImage.sprite = Sprite.Create(paintTex, new Rect(0, 0, texWidth, texHeight), new Vector2(0.5f, 0.5f), batterSprite.pixelsPerUnit);
    }

    IEnumerator PouringProcess()
    {
        // currentLevel đi từ 0 (đáy thấp nhất) lên đến > 1 (ngập toàn bộ)
        float currentLevel = 0f;

        // Vòng lặp dừng khi mức nước vượt quá tổng trọng số tối đa có thể (khoảng 1.2 cho an toàn)
        while (currentLevel < 1.2f)
        {
            currentLevel += fillSpeed * Time.deltaTime;

            ApplyWaterLevel(currentLevel);

            paintTex.SetPixels32(currentPixels);
            paintTex.Apply();
            yield return null;
        }

        // Finish
        System.Array.Copy(batterPixels, currentPixels, batterPixels.Length);
        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();
        isPouring = false;
    }

    void ApplyWaterLevel(float level)
    {
        for (int i = 0; i < batterPixels.Length; i++)
        {
            // Bỏ qua pixel trong suốt
            if (batterPixels[i].a == 0) continue;

            // Lấy ngưỡng cần thiết để lấp pixel này
            float threshold = fillThresholdMap[i];

            // Nếu mực nước (level) đã dâng qua ngưỡng (threshold) của pixel
            if (level > threshold)
            {
                // Tính độ sâu ngập (Delta)
                float delta = level - threshold;

                // Tính Alpha dựa trên độ sâu (tạo độ dày/mờ biên)
                float alphaPercent = delta / edgeSoftness;

                if (alphaPercent > 1f) alphaPercent = 1f;

                // Cập nhật Alpha
                byte targetAlpha = (byte)(batterPixels[i].a * alphaPercent);
                if (targetAlpha > currentPixels[i].a)
                {
                    currentPixels[i] = batterPixels[i];
                    currentPixels[i].a = targetAlpha;
                }
            }
        }
    }

    // Hàm cắt pixel (Copy từ đoạn code trước của bạn)
    Color32[] GetPixelsFromSprite(Sprite sprite)
    {
        var rect = sprite.rect;
        int width = (int)rect.width;
        int height = (int)rect.height;

        if (width == sprite.texture.width && height == sprite.texture.height)
            return sprite.texture.GetPixels32();

        Color32[] fullPixels = sprite.texture.GetPixels32();
        Color32[] croppedPixels = new Color32[width * height];
        int startX = (int)rect.x;
        int startY = (int)rect.y;
        int atlasWidth = sprite.texture.width;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int atlasIndex = (startY + y) * atlasWidth + (startX + x);
                int localIndex = y * width + x;
                if (atlasIndex < fullPixels.Length) croppedPixels[localIndex] = fullPixels[atlasIndex];
            }
        }
        return croppedPixels;
    }
}