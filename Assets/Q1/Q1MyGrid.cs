using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

[System.Serializable]
public class GridPoint
{
    public int x;
    public int y;
    public Color color;
    public int colorIndex;

    /// <summary>
    /// 根据左侧和上方格子的颜色下标，随机生成当前格子的颜色
    /// </summary>
    public void GenerateRandomColor(int LIndex, int TIndex)
    {
        int colorCount = Q1.COLORS.Length;
        float[] probs = new float[colorCount];
        float baseProb = 1f / colorCount; // 默认基准概率？是这个意思吗，五个颜色的平均值

        float xProb = Q1MyGrid.vectorData.x / 100f;
        float yProb = Q1MyGrid.vectorData.y / 100f;
        float zProb = Q1MyGrid.vectorData.z / 100f;

        if (LIndex == -1 && TIndex == -1)
        {
            for (int i = 0; i < colorCount; i++) probs[i] = baseProb;
        }
        else if (LIndex == TIndex && LIndex != -1)
        {
            float p = Mathf.Clamp01(baseProb + xProb + yProb + zProb);
            float remain = 1f - p;
            float otherProb = remain / (colorCount - 1);

            for (int i = 0; i < colorCount; i++)
            {
                if (i == LIndex) probs[i] = p;
                else probs[i] = otherProb;
            }
        }
        else
        {
            int validCount = 0;
            float usedProb = 0f;

            if (LIndex >= 0) 
            { 
                validCount++; 
                usedProb += baseProb + xProb; 
            }
            if (TIndex >= 0) 
            { 
                validCount++; 
                usedProb += baseProb + yProb; 
            }

            usedProb = Mathf.Clamp01(usedProb);
            float remain = 1f - usedProb;
            
            float otherProb = remain / (colorCount - validCount);

            for (int i = 0; i < colorCount; i++)
            {
                if (i == LIndex) probs[i] = baseProb + xProb;
                else if (i == TIndex) probs[i] = baseProb + yProb;
                else probs[i] = otherProb;
            }
        }

        float randomVal = Random.value;
        float cumulative = 0f;
        int selectedIndex = 0;
        for (int i = 0; i < colorCount; i++)
        {
            cumulative += probs[i];
            if (randomVal <= cumulative)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex == 0 && randomVal > cumulative) 
        {
            selectedIndex = colorCount - 1;
        }

        colorIndex = selectedIndex;
        color = Q1.COLORS[selectedIndex];
    }
}

public class Q1MyGrid : MonoBehaviour
{
    public static Vector3 vectorData;
    public GridPoint[,] pointsArray;

    public GameObject gridItemPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetData(Vector3 vector3)
    {
        vectorData = vector3;
    }

    /// <summary>
    /// 随机生成格子
    /// </summary>
    public void GenerateGrid()
    {
        int size = 10;
        pointsArray = new GridPoint[size, size];

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        RectTransform panelRect = GetComponent<RectTransform>();
        if (panelRect == null)
        {
            Debug.LogError("Q1MyGrid needs a RectTransform component to calculate size.");
            return;
        }

        float panelWidth = panelRect.rect.width;
        float panelHeight = panelRect.rect.height;

        float calculatedCellWidth = panelWidth / size;
        float calculatedCellHeight = panelHeight / size;

        float startX = -panelWidth / 2f + (calculatedCellWidth / 2f);
        float startY = panelHeight / 2f - (calculatedCellHeight / 2f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                GridPoint point = new GridPoint
                {
                    x = x,
                    y = y
                };
                
                int lIndex = (x > 0) ? pointsArray[x - 1, y].colorIndex : -1;
                int tIndex = (y > 0) ? pointsArray[x, y - 1].colorIndex : -1;

                point.GenerateRandomColor(lIndex, tIndex);
                
                pointsArray[x, y] = point;

                GameObject cellObj = Instantiate(gridItemPrefab);
                cellObj.name = $"Cell_{x}_{y}";
                cellObj.transform.SetParent(this.transform, false);

                RectTransform rect = cellObj.GetComponent<RectTransform>();
                if (rect == null)
                {
                    rect = cellObj.AddComponent<RectTransform>();
                }
                
                rect.sizeDelta = new Vector2(calculatedCellWidth, calculatedCellHeight);
                
                float posX = startX + x * calculatedCellWidth;
                float posY = startY - y * calculatedCellHeight;
                rect.localPosition = new Vector3(posX, posY, 0);

                Image img = cellObj.GetComponent<Image>();
                if (img == null)
                {
                    img = cellObj.AddComponent<Image>();
                }
                img.color = point.color;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
