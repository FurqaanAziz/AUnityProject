using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    public class GridManager : MonoBehaviour
    {
        public List<GameObject> cardPrefabs; // List of card prefabs
        public int rows = 2;                  // Number of rows
        public int columns = 2;               // Number of columns
        public float spacing = 10f;            // Space between cards
        public float aspectRatio = 1.0f;       // Aspect ratio (width/height)

        private Dictionary<string, int> prefabIDs = new Dictionary<string, int>();
        private List<int> cardIDs = new List<int>();

        private void Start()
        {
            SetupGridLayout();
            CreateGrid(rows, columns);
        }

        public void CreateGrid(int rows, int columns)
        {
            // Ensure the total number of cards is even
            if ((rows * columns) % 2 != 0)
            {
                columns = (columns % 2 == 0) ? columns : columns - 1; // Make columns even
                if (columns < 2)
                {
                    columns = 2; // Ensure at least 2 columns
                }
                rows = Mathf.Clamp(rows, 2, 6);
            }

            // Clear existing children (if any) before creating a new grid
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            cardIDs.Clear(); // Clear previous IDs
            prefabIDs.Clear(); // Clear previous prefab IDs

            int totalCards = rows * columns; // Total cards needed
            int totalPairs = totalCards / 2; // Total pairs needed

            // Create IDs based on card prefab names
            foreach (var prefab in cardPrefabs)
            {
                if (!prefabIDs.ContainsKey(prefab.name))
                {
                    prefabIDs[prefab.name] = prefabIDs.Count; // Assign a new ID based on current count
                }

                int id = prefabIDs[prefab.name];

                // Add the ID twice for matching pairs
                cardIDs.Add(id);
                cardIDs.Add(id);
            }

            // If we don't have enough pairs, repeat the available pairs
            while (cardIDs.Count < totalCards)
            {
                foreach (var prefab in cardPrefabs)
                {
                    int id = prefabIDs[prefab.name];
                    cardIDs.Add(id);
                    cardIDs.Add(id);
                    if (cardIDs.Count >= totalCards) break; // Stop if we reached the desired count
                }
            }

            // Shuffle the list of IDs
            Shuffle(cardIDs);

            // Loop to create the grid of cards
            for (int i = 0; i < totalCards; i++)
            {
                int cardId = cardIDs[i]; // Get the ID from the shuffled list

                // Find prefab name by ID
                string prefabName = prefabIDs.FirstOrDefault(x => x.Value == cardId).Key;
                if (prefabName != null)
                {
                    GameObject cardInstance = Instantiate(cardPrefabs.Find(p => p.name == prefabName), transform);
                    Card card = cardInstance.GetComponent<Card>();
                    card.id = cardId; // Assign the ID to the card
                    card.Attach(FindObjectOfType<CardGameManager>());
                }
            }

            // Update the layout after adding all cards
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
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
