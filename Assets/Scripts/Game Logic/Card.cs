using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardGame
{
    public enum CardEvent
    {
        Flipped,
        Matched,
        Mismatched
    }

    public class Card : MonoBehaviour, ISubject, IPointerClickHandler
    {
        public int id; // Unique identifier for matching
        public bool isFaceUp = false; // Card state
        public Sprite faceSprite; // Sprite for the face of the card
        public Sprite backSprite; // Sprite for the back of the card
        private Image cardImage; // Reference to the Image component

        private List<IObserver> observers = new List<IObserver>();

        void Start()
        {
            cardImage = GetComponent<Image>();
            cardImage.sprite = backSprite; // Set the initial sprite to the back
        }

        public void Attach(IObserver observer) => observers.Add(observer);
        public void Detach(IObserver observer) => observers.Remove(observer);

        public void Notify(Card card, CardEvent cardEvent)
        {
            foreach (var observer in observers)
            {
                observer.OnNotify(card, cardEvent);
            }
        }

        public void Flip()
        {
            isFaceUp = !isFaceUp;

            // Change the sprite based on the card state
            cardImage.sprite = isFaceUp ? faceSprite : backSprite;

            Notify(this, CardEvent.Flipped); // Notify observers about the flip
        }

        // Handle card click
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isFaceUp) // Only flip if it's face down
            {
                Flip();
            }
        }
    }
}
