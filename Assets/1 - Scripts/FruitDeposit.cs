using UnityEngine;

public class FruitDeposit : MonoBehaviour
{
    [Header("Setup")]
    [Range(2, 3)]
    public int maxFruits = 3;
    public IntVariable currentFruits;

    [Header("References")]
    [SerializeField] private SpriteRenderer spRenderer;

    [Header("2 Fruits")]
    [SerializeField] private Sprite empty2;
    [SerializeField] private Sprite one2;
    [SerializeField] private Sprite two2;

    [Header("3 Fruits")]
    [SerializeField] private Sprite empty3;
    [SerializeField] private Sprite one3;
    [SerializeField] private Sprite two3;
    [SerializeField] private Sprite three3;

    private void Awake()
    {
        if (spRenderer == null)
            spRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        currentFruits.onValueChange += UpdateSprite;
    }

    private void OnDisable()
    {
        currentFruits.onValueChange -= UpdateSprite;
    }

    private void UpdateSprite()
    {
        switch (currentFruits.Value)
        {
            case 0:
                spRenderer.sprite = (maxFruits == 2 ? empty2 : empty3);
                break;

            case 1:
                spRenderer.sprite = (maxFruits == 2 ? one2 : one3);
                break;

            case 2:
                spRenderer.sprite = (maxFruits == 2 ? two2 : two3);
                break;

            case 3:
                spRenderer.sprite = (maxFruits == 2 ? two2 : three3);
                break;

            default:
                spRenderer.sprite = (maxFruits == 2 ? empty2 : empty3);
                break;
        }
    }
}
