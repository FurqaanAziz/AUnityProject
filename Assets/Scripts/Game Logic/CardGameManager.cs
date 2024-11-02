using UnityEngine;
using System.Collections.Generic;

namespace CardGame
{
    public class CardGameManager : MonoBehaviour, IObserver
    {
        private List<Card> flippedCards = new List<Card>();
        public int score = 0;

        public void OnNotify(Card card, CardEvent cardEvent)
        {
            switch (cardEvent)
            {
                case CardEvent.Flipped:
                    HandleCardFlipped(card);
                    break;
                case CardEvent.Matched:
                    score += 10; // Increment score for a match
                    break;
                case CardEvent.Mismatched:
                    // Handle mismatch logic (e.g., flip back after delay)
                    break;
            }
        }

        private void HandleCardFlipped(Card card)
        {
            flippedCards.Add(card);
            if (flippedCards.Count == 2)
            {
                CheckForMatch();
            }
        }

        private void CheckForMatch()
        {
            if (flippedCards[0].id == flippedCards[1].id)
            {
                // Cards match
                flippedCards[0].Notify(flippedCards[0], CardEvent.Matched);
                flippedCards[1].Notify(flippedCards[1], CardEvent.Matched);
            }
            else
            {
                // Cards do not match
                // Optionally, add logic to flip them back
                foreach (var card in flippedCards)
                {
                    card.Notify(card, CardEvent.Mismatched);
                }
            }
            flippedCards.Clear(); // Clear flipped cards after checking
        }
    }
}