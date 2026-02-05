using System.Collections.Generic;
using UnityEngine;
public enum CakeMoldType
{
    None,
    Fish_Cake,
    Moon_Cake,
    Round_Cake,
    Slipper_Cake,
}
public enum IngredientPhaseType//Nguyen lieu lam nhan banh
{
    None,
}
public enum AdditionTimingType//Nguyen lieu trang tri
{
    None,
}
public class CakeMoldBase
{
    public CakeMoldType moldType;//Loại khuôn bánh quyết định chiếc bánh

    public Sprite icon;

    public PoolType prefabType;

    public List<CakeProcessStage> steps = new List<CakeProcessStage>
    {
    CakeProcessStage.AddingFilling,
    CakeProcessStage.PouringTopLayer,
    CakeProcessStage.Baking,
    CakeProcessStage.Decorating,
    CakeProcessStage.Completed
    };//quy trinh lam ban

    public List<IngredientPhaseType> ingredientTypes;//Các loại nhân bánh có thể bỏ vào khuôn
}
