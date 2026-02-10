using UnityEngine;

[System.Serializable]
public class CakeBase
{
    public CakeMoldType moldType;//Loại khuôn bánh quyết định chiếc bánh

    public AdditionTimingType additionTimingType;//trang tri banh

    public IngredientPhaseType ingredientPhaseType;//Nhan banh

    public Sprite iconCakeMold, iconCake, iconCakeWhenAddition;

    public UIDraggable prefab;
}
