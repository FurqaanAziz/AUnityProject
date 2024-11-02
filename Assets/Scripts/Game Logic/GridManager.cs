using UnityEngine;

namespace CardGame
{
    public class GridManager : MonoBehaviour
    {
        public GameObject cardPrefab; // Reference to the card prefab
        public int rows = 3;          // Number of rows
        public int columns = 3;       // Number of columns

        void Start()
        {
            CreateGrid(rows, columns);
        }

        void CreateGrid(int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Calculate position for each card
                    Vector3 position = new Vector3(i, 0, j);
                    // Instantiate a card at the calculated position
                    Card card = Instantiate(cardPrefab, position, Quaternion.identity).GetComponent<Card>();
                    // Optionally, you can set the card ID or any other properties here
                    card.id = (i * columns) + j; // Example of setting an ID
                    // Attach the observer (e.g., CardGameManager) if needed
                }
            }
        }
    }
}