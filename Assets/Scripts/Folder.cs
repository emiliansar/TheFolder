using UnityEngine;

public class Folder : MonoBehaviour
{
    public ColorType color;

    private Vector3 originalScale;
    private SpriteRenderer sr;

    private int baseOrder;
    private bool isHovered;

    void Awake()
    {
        originalScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();

        baseOrder = sr.sortingOrder;
    }

    public void SetHover(bool state)
    {
        if (state)
        {
            isHovered = true;

            transform.localScale = originalScale * 1.2f;

            // ❗ поднимаем поверх всех
            sr.sortingOrder = 100;
        }
        else
        {
            isHovered = false;

            transform.localScale = originalScale;

            // ❗ возвращаем назад
            sr.sortingOrder = baseOrder;
        }
    }
}