using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Cần thư viện này để dùng List

public class DragEffects : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Ngưỡng sai số (nếu cần), mặc định là 0")]
    public float centerThreshold = 0f;
    [Tooltip("Kéo danh sách các Image con cần quản lý vào đây")]
    private List<Image> imageList = new List<Image>();

    void Awake()
    {
        OnInit();
    }

    public void OnInit()
    {
        imageList.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Image img = transform.GetChild(i).GetComponent<Image>();
            if (img != null)
            {
                img.gameObject.SetActive(true);
                imageList.Add(img);
            }
        }
    }
    // ---------------------------------------------------------
    // PHẦN 1: Logic Xử lý Trái/Phải và Xóa
    // ---------------------------------------------------------

    // Kéo hàm này vào sự kiện OnObjectDropped của UIDeadZone
    public void ProcessDropZone(GameObject draggedObj)
    {
        if (draggedObj == null) return;

        // 1. Xác định vị trí
        // Lấy vị trí X của vật đang kéo
        float objectX = draggedObj.transform.position.x;

        // Lấy vị trí X của chính vùng chết (Script này phải gắn trên vùng chết)
        float zoneX = transform.position.x;

        // 2. So sánh để biết bên Trái hay Phải
        if (objectX < zoneX - centerThreshold)
        {
            Debug.Log("Đối tượng nằm bên TRÁI vùng chết -> Xóa phần tử ĐẦU TIÊN");
            RemoveFirstItem();
        }
        else
        {
            Debug.Log("Đối tượng nằm bên PHẢI vùng chết -> Xóa phần tử CUỐI CÙNG");
            RemoveLastItem();
        }
    }

    private void RemoveFirstItem()
    {
        if (imageList.Count > 0)
        {
            // Lấy ra phần tử đầu tiên (Index 0)
            Image itemToRemove = imageList[0];

            // Xóa khỏi danh sách quản lý
            imageList.RemoveAt(0);

            // Xóa GameObject khỏi màn hình
            if (itemToRemove != null)
                Destroy(itemToRemove.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void RemoveLastItem()
    {
        if (imageList.Count > 0)
        {
            // Lấy ra phần tử cuối cùng (Index = Count - 1)
            int lastIndex = imageList.Count - 1;
            Image itemToRemove = imageList[lastIndex];

            // Xóa khỏi danh sách quản lý
            imageList.RemoveAt(lastIndex);

            // Xóa GameObject khỏi màn hình
            if (itemToRemove != null)
                Destroy(itemToRemove.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}