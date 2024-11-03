using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    public class CardGameManager : MonoBehaviour, IObserver
    {
        private Card firstCard;
        private Card secondCard;
        private List<Card> flippedCards = new List<Card>();
        private int matchesFound = 0;

        public void CardClicked(Card clickedCard)
        {
            if (firstCard == null)
            {
                firstCard = clickedCard;
                flippedCards.Add(firstCard);
            }
            else if (secondCard == null && clickedCard != firstCard)
            {
                secondCard = clickedCard;
                flippedCards.Add(secondCard);
                CheckForMatch();
            }
        }

        private void CheckForMatch()
        {
            if (firstCard.id == secondCard.id)
            {
                // Cards match
                matchesFound++;
                firstCard.Notify(firstCard, CardEvent.Matched);
                secondCard.Notify(secondCard, CardEvent.Matched);
                firstCard = null;
                secondCard = null; // Reset cards for the next turn
            }
            else
            {
                // Cards do not match, wait before flipping back
                Invoke("ResetCards", 1f); // Delay before flipping back
            }
        }

        private void ResetCards()
        {
            firstCard.Flip(); // Flip back
            secondCard.Flip(); // Flip back
            firstCard = null;
            secondCard = null; // Reset for the next turn
        }

        public void OnNotify(Card card, CardEvent cardEvent)
        {
            // Handle notifications if needed
        }
    }
}
