using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIRadialReveal : MonoBehaviour
{
    // ... (Giữ nguyên các biến Header như cũ) ...
    [Header("Cài đặt Tốc độ & Giao diện")]
    [Tooltip("Tốc độ dâng lên của bột (0 -> 1)")]
    [SerializeField] private float fillSpeed = 0.3f;

    [Tooltip("Độ mềm của mép bột (Càng cao càng mượt)")]
    [SerializeField] private float edgeSoftness = 0.1f;

    [Header("QUAN TRỌNG: Cân bằng Lực (Logic)")]
    [SerializeField] private Sprite moldSprite;
    [Range(0f, 1f)][SerializeField] private float moldWeight = 0.85f;
    [Range(0f, 1f)][SerializeField] private float distanceWeight = 0.15f;

    [Header("Cài đặt Noise (Ngẫu nhiên)")]
    [SerializeField] private float noiseAmount = 0.1f;
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
    private float[] fillThresholdMap;

    private float maxDist;
    private Coroutine coroutinePouring;

    // --- BIẾN TRẠNG THÁI GAME ---
    private float currentLevel = 0f;
    private bool isFinished = false;

    // THÊM BIẾN NÀY: Mốc bắt đầu thực tế
    private float startLevel = 0f;

    void Start()
    {
        SetupTexture();
    }

    // --- THÊM HÀM NÀY ĐỂ RESET KHI DISABLE ---
    void OnDisable()
    {
        // 1. Dừng việc đổ bột ngay lập tức
        if (coroutinePouring != null) StopCoroutine(coroutinePouring);

        // 2. Reset trạng thái game
        isFinished = false;

        // Reset level về mốc bắt đầu (để lần sau bật lại không bị delay)
        currentLevel = startLevel;

        // 3. Xóa hình ảnh (Làm trong suốt texture)
        // Kiểm tra null để tránh lỗi nếu tắt object trước khi Start kịp chạy
        if (paintTex != null && currentPixels != null && batterPixels != null)
        {
            for (int i = 0; i < currentPixels.Length; i++)
            {
                // Reset về pixel gốc nhưng Alpha = 0
                currentPixels[i] = batterPixels[i];
                currentPixels[i].a = 0;
            }

            // Cập nhật lại Texture
            paintTex.SetPixels32(currentPixels);
            paintTex.Apply();
        }
    }
    // -----------------------------------------

    public void StarPouring()
    {
        if (isFinished) return;

        // LOGIC FIX TRỄ:
        if (currentLevel < startLevel)
        {
            currentLevel = startLevel;
        }

        if (coroutinePouring != null) StopCoroutine(coroutinePouring);
        coroutinePouring = StartCoroutine(PouringProcess());
    }

    public void EndPouring()
    {
        if (coroutinePouring != null) StopCoroutine(coroutinePouring);
    }

    void SetupTexture()
    {
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

        float minVal = float.MaxValue;

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int i = y * texWidth + x;

                currentPixels[i] = batterPixels[i];
                currentPixels[i].a = 0;

                float u = x / (float)texWidth;
                float v = y / (float)texHeight;
                int mX = Mathf.Clamp(Mathf.FloorToInt(u * moldWidth), 0, moldWidth - 1);
                int mY = Mathf.Clamp(Mathf.FloorToInt(v * moldHeight), 0, moldHeight - 1);

                Color32 p = moldPixels[mY * moldWidth + mX];
                float heightNorm = 1.0f - ((0.299f * p.r + 0.587f * p.g + 0.114f * p.b) / 255f);

                float dx = x - centerX;
                float dy = y - centerY;
                float distNorm = Mathf.Sqrt(dx * dx + dy * dy) / maxDist;

                float noise = Mathf.PerlinNoise((x * noiseScale) + rndX, (y * noiseScale) + rndY);
                float noiseNorm = (noise - 0.5f) * noiseAmount;

                float threshold = (heightNorm * moldWeight) + (distNorm * distanceWeight) + noiseNorm;

                fillThresholdMap[i] = threshold;

                if (threshold < minVal) minVal = threshold;
            }
        }

        startLevel = minVal + (edgeSoftness * 0.1f);
        currentLevel = startLevel;
        isFinished = false;

        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();

        Vector2 pivot = new Vector2(batterSprite.pivot.x / batterSprite.rect.width, batterSprite.pivot.y / batterSprite.rect.height);
        targetImage.sprite = Sprite.Create(paintTex, new Rect(0, 0, texWidth, texHeight), pivot, batterSprite.pixelsPerUnit);
    }

    IEnumerator PouringProcess()
    {
        float targetMax = startLevel + 1.3f;

        while (currentLevel < targetMax)
        {
            currentLevel += fillSpeed * Time.deltaTime;

            if (currentLevel >= (startLevel + 1.15f) && !isFinished)
            {
                isFinished = true;
                OnLevelComplete();
            }

            ApplyWaterLevel(currentLevel);
            paintTex.SetPixels32(currentPixels);
            paintTex.Apply();
            yield return null;
        }

        if (!isFinished)
        {
            isFinished = true;
            OnLevelComplete();
        }

        System.Array.Copy(batterPixels, currentPixels, batterPixels.Length);
        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();
    }

    void OnLevelComplete() { Observer.OnChangeStage?.Invoke(); }

    void ApplyWaterLevel(float level)
    {
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
                if (atlasIndex >= 0 && atlasIndex < fullPixels.Length) croppedPixels[y * width + x] = fullPixels[atlasIndex];
            }
        }
        return croppedPixels;
    }
}