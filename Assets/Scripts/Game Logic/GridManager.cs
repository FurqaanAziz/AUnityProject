using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    public class GridManager : MonoBehaviour
    {
        public GameObject cardPrefab; // Reference to the card prefab
        public int rows = 3;          // Number of rows
        public int columns = 3;       // Number of columns
        public float spacing = 10f;    // Space between cards
        public float aspectRatio = 1.0f; // Aspect ratio (width/height)

        private void Start()
        {
            // Ensure rows and columns are within the allowed range
            rows = Mathf.Clamp(rows, 2, 6);
            columns = Mathf.Clamp(columns, 2, 6);

            // Ensure the total number of cards is even
            if ((rows * columns) % 2 != 0)
            {
                columns = (columns % 2 == 0) ? columns : columns - 1; // Make columns even
                if (columns < 2)
                {
                    columns = 2; // Ensure at least 2 columns
                }
                // Re-calculate rows to keep a minimum size
                rows = Mathf.Clamp(rows, 2, 10);
            }

            SetupGridLayout();
            CreateGrid(rows, columns);
        }

        private void SetupGridLayout()
        {
            GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                Debug.LogError("GridLayoutGroup component not found on the GridManager.");
                return;
            }

            RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRect.sizeDelta;

            int padding = 10;
            gridLayoutGroup.padding = new RectOffset(padding, padding, padding, padding);

            float availableWidth = canvasSize.x - (padding * 2);
            float availableHeight = canvasSize.y - (padding * 2);

            float cellWidth = (availableWidth - (spacing * (columns - 1))) / columns;
            float cellHeight = (availableHeight - (spacing * (rows - 1))) / rows;

            if (cellWidth / cellHeight > aspectRatio)
            {
                cellWidth = cellHeight * aspectRatio;
            }
            else
            {
                cellHeight = cellWidth / aspectRatio;
            }

            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            gridLayoutGroup.spacing = new Vector2(spacing, spacing);
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns;
        }

        private void CreateGrid(int rows, int columns)
        {
            // Clear existing children (if any) before creating a new grid
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Create a list of IDs for cards, ensuring each ID appears twice
            List<int> cardIDs = new List<int>();
            for (int i = 0; i < (rows * columns) / 2; i++)
            {
                cardIDs.Add(i); // Add each ID once
                cardIDs.Add(i); // Add each ID again for matching
            }

            // Shuffle the list of IDs
            Shuffle(cardIDs);

            // Loop to create the grid of cards
            for (int i = 0; i < rows * columns; i++)
            {
                GameObject cardInstance = Instantiate(cardPrefab, transform);
                Card card = cardInstance.GetComponent<Card>();
                card.id = cardIDs[i]; // Assign a shuffled ID to each card
                card.Attach(FindObjectOfType<CardGameManager>());
            }

            // Update the layout after adding all cards
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private void Shuffle(List<int> list)
        {
            System.Random rand = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = rand.Next(n--);
                int temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }
    }
}
