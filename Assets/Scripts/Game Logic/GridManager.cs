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
            SetupGridLayout();
            CreateGrid(rows, columns);
        }

        private void SetupGridLayout()
        {
            // Get the GridLayoutGroup component attached to this GameObject
            GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                Debug.LogError("GridLayoutGroup component not found on the GridManager.");
                return;
            }

            // Get the Canvas size
            RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRect.sizeDelta;

            // Set padding
            int padding = 10;
            gridLayoutGroup.padding = new RectOffset(padding, padding, padding, padding);

            // Calculate available width and height for the grid
            float availableWidth = canvasSize.x - (padding * 2);
            float availableHeight = canvasSize.y - (padding * 2);

            // Calculate cell size based on available space
            float cellWidth = (availableWidth - (spacing * (columns - 1))) / columns;
            float cellHeight = cellWidth / aspectRatio; // Maintain the aspect ratio

            // Check if the calculated height fits the available space
            if (cellHeight > (availableHeight - (spacing * (rows - 1))) / rows)
            {
                cellHeight = (availableHeight - (spacing * (rows - 1))) / rows;
                cellWidth = cellHeight * aspectRatio; // Adjust width according to the aspect ratio
            }

            // Set the cell size and spacing
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            gridLayoutGroup.spacing = new Vector2(spacing, spacing); // Set the spacing between cards
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns; // Set how many columns to have
        }

        private void CreateGrid(int rows, int columns)
        {
            // Clear existing children (if any) before creating a new grid
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Loop to create the grid of cards
            for (int i = 0; i < rows * columns; i++)
            {
                // Instantiate the card prefab as a child of this GridManager
                GameObject cardInstance = Instantiate(cardPrefab, transform);
                Card card = cardInstance.GetComponent<Card>();
                card.id = i; // Example of setting an ID
                // Attach the observer (e.g., CardGameManager) if needed
                card.Attach(FindObjectOfType<CardGameManager>());
            }

            // Update the layout after adding all cards
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}
