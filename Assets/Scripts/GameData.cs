using System.Collections.Generic;

namespace CardGame
{
    [System.Serializable]
    public class GameData
    {
        public int matchesFound;
        public List<CardData> cardStates = new List<CardData>();
        public List<int[]> matchedPairs = new List<int[]>(); // List of matched pairs

        [System.Serializable]
        public class CardData
        {
            public int id;
            public bool isFaceUp;
        }
    }
}
