using System.Collections.Generic;

namespace CardGame
{
    [System.Serializable]
    public class GameData
    {
        public int matchesFound;
        public List<CardData> cardStates = new List<CardData>();
        public int rows; // Store the number of rows
        public int columns; // Store the number of columns

        [System.Serializable]
        public class CardData
        {
            public int id;
            public bool isFaceUp;
        }
    }
}
