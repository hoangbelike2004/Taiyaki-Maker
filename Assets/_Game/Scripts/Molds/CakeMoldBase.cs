using System.Collections.Generic;
using UnityEngine;
public enum CakeMoldType
{
    None,
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

    public List<IngredientPhaseType> ingredientType;//Các loại nhân bánh có thể bỏ vào khuôn
}
