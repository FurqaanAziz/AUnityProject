using System.Collections;
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
        private Coroutine flipCoroutine; // Reference to the flip coroutine

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
            if (flipCoroutine != null)
            {
                StopCoroutine(flipCoroutine); // Stop any ongoing flip
            }
            flipCoroutine = StartCoroutine(FlipCard());
        }

        private IEnumerator FlipCard()
        {
            // Animate the flip
            float duration = 0.1f;
            float elapsedTime = 0f;
            Quaternion originalRotation = transform.localRotation;

            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / (duration / 2);
                transform.localRotation = Quaternion.Slerp(originalRotation, originalRotation * Quaternion.Euler(0, 90, 0), t);
                yield return null;
            }

            // Change the sprite after the first half
            isFaceUp = !isFaceUp;
            cardImage.sprite = isFaceUp ? faceSprite : backSprite;

            // Complete the first half of the flip
            transform.localRotation = originalRotation * Quaternion.Euler(0, 90, 0);

            // Second half: Flip back to original position
            elapsedTime = 0f;
            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / (duration / 2);
                transform.localRotation = Quaternion.Slerp(originalRotation * Quaternion.Euler(0, 90, 0), originalRotation, t);
                yield return null;
            }

            // Ensure the rotation is exactly back to original
            transform.localRotation = originalRotation;
            Notify(this, CardEvent.Flipped);
        }

        // Handle card click
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isFaceUp) // Only flip if it's face down
            {
                Flip();
                FindObjectOfType<CardGameManager>().CardClicked(this); // Notify the CardGameManager
            }
        }
    }
}
