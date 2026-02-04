using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UITopDownReveal : MonoBehaviour
{
    [Header("Cài đặt Tốc độ")]
    [Tooltip("Tốc độ hiện hình")]
    [SerializeField] private float revealSpeed = 0.5f;

    [Header("Thông tin Tiến độ")]
    [Range(0f, 1f)]
    public float CurrentProgress = 0f; // Biến này sẽ chạy từ 0 đến 1

    [Header("Cài đặt Hiệu ứng")]
    [Range(0.01f, 0.5f)]
    [SerializeField] private float edgeSoftness = 0.1f;

    [Tooltip("Độ gợn sóng của đường cắt")]
    [SerializeField] private float noiseAmount = 0.2f;
    [SerializeField] private float noiseScale = 10f;

    [Header("Tham chiếu")]
    [SerializeField] private Image targetImage;

    // --- BIẾN HỆ THỐNG ---
    private Texture2D paintTex;
    private Color32[] currentPixels;
    private Color32[] originalPixels;
    private float[] noiseMap;

    private int texWidth, texHeight;
    private bool isRevealing = false;

    // Định nghĩa giới hạn quét: Từ trên (1.2) xuống dưới (-0.2)
    private const float START_Y = 1.2f;
    private const float END_Y = -0.2f;

    void Start()
    {
        SetupTexture();
    }

    void Update()
    {
        // Ấn Space để test
        if (Input.GetKeyDown(KeyCode.Space) && !isRevealing)
        {
            isRevealing = true;
            StartCoroutine(RevealProcess());
        }
    }

    void SetupTexture()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetImage.sprite == null) return;

        Sprite sourceSprite = targetImage.sprite;
        if (!sourceSprite.texture.isReadable)
        {
            Debug.LogError("Vui lòng bật Read/Write Enabled trong Import Settings của ảnh!");
            return;
        }

        Rect rect = sourceSprite.rect;
        texWidth = (int)rect.width;
        texHeight = (int)rect.height;

        paintTex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
        originalPixels = GetPixelsFromSprite(sourceSprite);
        currentPixels = new Color32[originalPixels.Length];
        noiseMap = new float[originalPixels.Length];

        float rndX = Random.Range(0f, 100f);
        float rndY = Random.Range(0f, 100f);

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int i = y * texWidth + x;

                // Ban đầu ẩn hoàn toàn
                currentPixels[i] = originalPixels[i];
                currentPixels[i].a = 0;

                // Tạo noise map
                float u = (float)x / texWidth;
                float v = (float)y / texHeight;
                float noise = Mathf.PerlinNoise((u * noiseScale) + rndX, (v * noiseScale) + rndY);
                noiseMap[i] = (noise - 0.5f) * noiseAmount;
            }
        }

        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();

        targetImage.sprite = Sprite.Create(paintTex, new Rect(0, 0, texWidth, texHeight), new Vector2(0.5f, 0.5f), sourceSprite.pixelsPerUnit);
    }

    IEnumerator RevealProcess()
    {
        float currentY = START_Y;

        while (currentY > END_Y)
        {
            currentY -= revealSpeed * Time.deltaTime;

            // Tính toán tiến độ (0 -> 1)
            CurrentProgress = Mathf.InverseLerp(START_Y, END_Y, currentY);

            ApplyScanline(currentY);

            paintTex.SetPixels32(currentPixels);
            paintTex.Apply();

            yield return null;
        }

        // Kết thúc: Đảm bảo 100% và hiện rõ nét
        CurrentProgress = 1.0f;

        System.Array.Copy(originalPixels, currentPixels, originalPixels.Length);
        paintTex.SetPixels32(currentPixels);
        paintTex.Apply();

        isRevealing = false;
    }

    void ApplyScanline(float scanLineLevel)
    {
        for (int i = 0; i < originalPixels.Length; i++)
        {
            if (originalPixels[i].a == 0) continue;

            int y = i / texWidth;

            float normalizedY = (float)y / texHeight;
            float effectiveY = normalizedY + noiseMap[i];

            if (effectiveY > scanLineLevel)
            {
                float dist = effectiveY - scanLineLevel;
                float alphaPercent = dist / edgeSoftness;

                if (alphaPercent > 1f) alphaPercent = 1f;
                else if (alphaPercent < 0f) alphaPercent = 0f;

                byte targetAlpha = (byte)(originalPixels[i].a * alphaPercent);

                if (targetAlpha > currentPixels[i].a)
                {
                    currentPixels[i] = originalPixels[i];
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