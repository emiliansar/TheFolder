using UnityEngine;

public class Paper : MonoBehaviour
{
    public ColorType color;

    [Header("Links")]
    public GameManager gameManager;

    [Header("Sprites")]
    public SpriteRenderer sr;
    public Sprite redSprite;
    public Sprite blueSprite;
    public Sprite greenSprite;
    public Sprite purpleSprite;
    public int baseOrder;

    private Vector3 offset;
    private Vector3 startPosition;

    private Folder currentHoveredFolder;

    public SpawnPoint currentSpawnPoint;

    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        sr = GetComponent<SpriteRenderer>();
        baseOrder = sr.sortingOrder;

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    void Update()
    {
        CheckHover();
    }

    void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        offset = transform.position - new Vector3(mousePos.x, mousePos.y, 0);

        sr.sortingOrder = 1000;

        gameManager.PlayTakeSound();
    }

    void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0) + offset;
    }

    void OnMouseUp()
    {
        CheckDrop();

        sr.sortingOrder = baseOrder;
    }

    void CheckHover()
    {
        float radius = 1.5f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        Folder bestFolder = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            Folder folder = hit.GetComponent<Folder>();

            if (folder != null)
            {
                float dist = Vector2.Distance(transform.position, folder.transform.position);

                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestFolder = folder;
                }
            }
        }

        if (bestFolder != currentHoveredFolder)
        {
            if (currentHoveredFolder != null)
                currentHoveredFolder.SetHover(false);

            currentHoveredFolder = bestFolder;

            if (currentHoveredFolder != null)
            {
                currentHoveredFolder.SetHover(true);

                sr.sortingOrder = currentHoveredFolder.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
            else
            {
                sr.sortingOrder = baseOrder;
            }
        }
    }

    void CheckDrop()
    {
        Debug.Log("CheckDrop works!");

        if (gameManager.state != GameState.Playing)
        {
            return;
        }

        if (currentHoveredFolder != null)
        {
            if (currentHoveredFolder.color == color)
            {
                Debug.Log("Correct drop");
                gameManager.PlayPutSound();
                gameManager.AddProgress();

                currentSpawnPoint.isOccupied = false;

                gameManager.paperCounter--;

                gameManager.UpdateUI();

                currentHoveredFolder.SetHover(false);
                currentHoveredFolder = null;

                Destroy(gameObject);
            }
            else
            {
                transform.position = startPosition;
                transform.rotation = startRotation;
            }
        }
        else
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }

        if (currentHoveredFolder != null)
        {
            //currentHoveredFolder.SetHover(false);
            currentHoveredFolder = null;
        }

        sr.sortingOrder = baseOrder;
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;

        switch (color)
        {
            case ColorType.Red:
                sr.sprite = redSprite;
                break;

            case ColorType.Blue:
                sr.sprite = blueSprite;
                break;

            case ColorType.Green:
                sr.sprite = greenSprite;
                break;

            case ColorType.Purple:
                sr.sprite = purpleSprite;
                break;
        }
    }
}