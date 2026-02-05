using UnityEngine;
using UnityEditor; // Bắt buộc có để can thiệp Inspector
using System.Collections.Generic;

[CustomEditor(typeof(CakeMoldSO))] // Định nghĩa script này dành cho CakeMoldSO
public class CakeMoldSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. Vẽ giao diện mặc định (để hiện các List như bình thường)
        base.OnInspectorGUI();

        // Lấy tham chiếu đến file SO đang chọn
        CakeMoldSO scriptableObject = (CakeMoldSO)target;

        GUILayout.Space(20); // Tạo khoảng trống cho đẹp

        // 2. Tạo nút bấm thêm khuôn Normal
        if (GUILayout.Button("Thêm Khuôn Normal (Có sẵn quy trình)", GUILayout.Height(40)))
        {
            // Tạo phần tử mới và điền sẵn dữ liệu
            CakeMoldNormal newMold = new CakeMoldNormal();

            // --- ĐIỀN DỮ LIỆU MẶC ĐỊNH TẠI ĐÂY ---
            newMold.moldType = CakeMoldType.Round_Cake; // Ví dụ mặc định
            newMold.steps = new List<CakeProcessStage>
            {
                CakeProcessStage.PouringBottomLayer,
                CakeProcessStage.AddingFilling,
                CakeProcessStage.PouringTopLayer,
                CakeProcessStage.Baking,
                CakeProcessStage.Completed
            };
            newMold.ingredientTypes = new List<IngredientPhaseType>();
            // -------------------------------------

            // Thêm vào list trong SO
            scriptableObject.cakeMoldNormals.Add(newMold);

            // Báo cho Unity biết dữ liệu đã thay đổi để nó lưu lại (hiện dấu sao *)
            EditorUtility.SetDirty(scriptableObject);
        }

        // 3. Tạo nút bấm thêm khuôn ADS (nếu cần)
        if (GUILayout.Button("Thêm Khuôn ADS (Có sẵn quy trình)"))
        {
            CakeMoldADS newAdsMold = new CakeMoldADS();
            newAdsMold.steps = new List<CakeProcessStage>
            {
                CakeProcessStage.PouringBottomLayer,
                CakeProcessStage.AddingFilling,
                CakeProcessStage.PouringTopLayer,
                CakeProcessStage.Baking,
                CakeProcessStage.Decorating,
                CakeProcessStage.Completed
            };

            scriptableObject.cakeMoldADSs.Add(newAdsMold);
            EditorUtility.SetDirty(scriptableObject);
        }
    }
}