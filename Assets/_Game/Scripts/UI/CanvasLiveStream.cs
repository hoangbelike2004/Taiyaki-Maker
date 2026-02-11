using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLiveStream : UICanvas
{
    [SerializeField] Button btnNext;
    [SerializeField] RectTransform rectComment; // Container chứa comment

    [Header("Config")]
    [SerializeField] float spawnInterval = 1.5f; // Thời gian sinh comment
    [SerializeField] float itemHeight = 100f;    // Khoảng cách giữa các comment (chiều cao item + spacing)
    [SerializeField] int maxItems = 3;           // Số lượng hiển thị tối đa

    // Mảng lưu độ mờ theo thứ tự: [0] mới nhất, [1] thứ hai, [2] thứ ba
    private float[] alphaLevels = new float[] { 1f, 0.5f, 0.25f };

    private List<ItemComent> activeComments = new List<ItemComent>();
    private ComentSO comentSO;
    private Coroutine spawnCoroutine;

    private RectTransform overlaybtn;

    private bool isNext = false;
    void Awake()
    {
        comentSO = Resources.Load<ComentSO>(GameConstants.KEY_DATA_GAME_COMMENT);
        overlaybtn = btnNext.transform.GetChild(0).GetComponent<RectTransform>();
    }

    void Start()
    {
        btnNext.onClick.AddListener(() =>
        {
            if (!isNext) return;
            GameController.Instance.StartBaking();
        });
    }

    void OnEnable()
    {
        // Xóa sạch comment cũ khi bật lên
        DespawnAllItems();
        // Bắt đầu coroutine sinh comment
        spawnCoroutine = StartCoroutine(AwaitComent());
        overlaybtn.gameObject.SetActive(true);
        isNext = false;
        StartCoroutine(OnCanNext());
    }

    void OnDisable()
    {
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        DespawnAllItems();
    }

    IEnumerator AwaitComent()
    {
        yield return new WaitForSeconds(spawnInterval);
        while (true)
        {
            SpawnNewComment();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnNewComment()
    {
        // 1. Lấy item từ Pool
        ItemComent newItem = SimplePool.Spawn<ItemComent>(PoolType.Item_Cmt, Vector3.zero, Quaternion.identity);
        CommentCardItem data = comentSO.GetCommentCardItem();

        // 2. Setup dữ liệu
        newItem.SetData(data, rectComment);

        // 3. Thêm vào ĐẦU danh sách (Index 0 luôn là cái mới nhất)
        activeComments.Insert(0, newItem);

        // 4. Cập nhật vị trí và độ mờ cho TOÀN BỘ danh sách
        UpdateAllCommentsVisual();
    }

    private void UpdateAllCommentsVisual()
    {
        // Duyệt qua danh sách để update vị trí
        for (int i = 0; i < activeComments.Count; i++)
        {
            // Nếu nằm trong giới hạn hiển thị (0, 1, 2)
            if (i < maxItems)
            {
                // Tính toán vị trí Y: Item mới nhất (0) ở dưới cùng (Y=0), cái tiếp theo ở trên (Y = i * height)
                float targetY = i * itemHeight;

                // Lấy độ mờ theo cấu hình (nếu i vượt quá mảng alpha thì lấy cái cuối cùng)
                float targetAlpha = (i < alphaLevels.Length) ? alphaLevels[i] : 0.25f;

                // Gọi lệnh update ở bên ItemComent
                activeComments[i].UpdateState(targetY, targetAlpha, 0.5f);
            }
            else
            {
                // Item thứ 4 trở đi (vượt quá giới hạn) -> Despawn
                ItemComent oldItem = activeComments[i];
                activeComments.RemoveAt(i); // Xóa khỏi list quản lý
                oldItem.DespawnAnim(0.5f);  // Gọi hiệu ứng biến mất và trả về pool
                i--; // Giảm i vì list vừa bị thụt lại 1 phần tử
            }
        }
    }

    public void DespawnAllItems()
    {
        for (int i = 0; i < activeComments.Count; i++)
        {
            if (activeComments[i] != null)
                SimplePool.Despawn(activeComments[i]);
        }
        activeComments.Clear();
    }

    IEnumerator OnCanNext()
    {
        yield return new WaitForSeconds(spawnInterval * 3);
        isNext = true;
        overlaybtn.gameObject.SetActive(false);
    }
}