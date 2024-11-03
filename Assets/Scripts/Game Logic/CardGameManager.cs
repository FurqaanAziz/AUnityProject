using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    public class CardGameManager : MonoBehaviour, IObserver
    {
        private Card firstCard;
        private Card secondCard;
        private int matchesFound = 0;
        private string saveFilePath;

        public Text scoreText; // Assign this in the Inspector
        public GameObject gameCompletedPanel; // Assign this in the Inspector
        public AudioClip cardFlippedAudio;
        public AudioClip cardMatchedAudio;
        public AudioClip cardMismatchedAudio;
        public AudioClip gameCompletedAudio;
        private AudioSource audioSource;

        private void Start()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, "gameSave.json");
            UpdateScoreText();
            audioSource = GetComponent<AudioSource>();
        }

        public void CardClicked(Card clickedCard)
        {
            if (firstCard == null)
            {
                firstCard = clickedCard;
                firstCard.Flip();
                PlayAudio(cardFlippedAudio);
            }
            else if (secondCard == null && clickedCard != firstCard)
            {
                secondCard = clickedCard;
                secondCard.Flip();
                PlayAudio(cardFlippedAudio);
                CheckForMatch();
            }
        }

        private void CheckForMatch()
        {
            if (firstCard.id == secondCard.id)
            {
                matchesFound++;
                PlayAudio(cardMatchedAudio);
                firstCard.Notify(firstCard, CardEvent.Matched);
                secondCard.Notify(secondCard, CardEvent.Matched);
                UpdateScoreText();
                CheckForGameCompletion();
                ResetSelectedCards();
            }
            else
            {
                PlayAudio(cardMismatchedAudio);
                Invoke("ResetCards", 1f); // Delay before flipping back
            }
        }

        private void ResetCards()
        {
            firstCard.Flip();
            secondCard.Flip();
            ResetSelectedCards();
        }

        private void ResetSelectedCards()
        {
            firstCard = null;
            secondCard = null; // Reset for the next turn
        }

        private void UpdateScoreText()
        {
            if (scoreText != null)
            {
                scoreText.text = "Matches Found: " + matchesFound;
            }
        }

        private void CheckForGameCompletion()
        {
            if (matchesFound == (FindObjectsOfType<Card>().Length / 2))
            {
                ShowGameCompletedPanel();
            }
        }

        private void ShowGameCompletedPanel()
        {
            if (gameCompletedPanel != null)
            {
                gameCompletedPanel.SetActive(true);
                PlayAudio(gameCompletedAudio);
            }
        }

        public void SaveGame()
        {
            GameData data = new GameData
            {
                matchesFound = matchesFound,
                cardStates = new List<GameData.CardData>()
            };

            // Loop through all cards to save their state
            foreach (var card in FindObjectsOfType<Card>())
            {
                data.cardStates.Add(new GameData.CardData
                {
                    id = card.id,
                    isFaceUp = card.isFaceUp
                });
            }

            // Convert data to JSON and save
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Game saved!");
        }

        public void LoadGame()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                GameData data = JsonUtility.FromJson<GameData>(json);

                matchesFound = data.matchesFound;
                UpdateScoreText();

                foreach (var cardData in data.cardStates)
                {
                    Card card = FindCardById(cardData.id);
                    if (card != null)
                    {
                        card.isFaceUp = cardData.isFaceUp;
                        if (card.isFaceUp)
                        {
                            card.Flip(); // Flip the card to show it's face up
                        }
                    }
                }

                Debug.Log("Game loaded!");
            }
            else
            {
                Debug.LogError("Save file not found!");
            }
        }

        private Card FindCardById(int id)
        {
            foreach (var card in FindObjectsOfType<Card>())
            {
                if (card.id == id)
                    return card;
            }
            return null;
        }

        public void OnNotify(Card card, CardEvent cardEvent)
        {
            // Handle notifications if needed
        }

        private void PlayAudio(AudioClip clip)
        {
            if (audioSource && clip)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        // Public methods for UI buttons
        public void OnSaveButtonClicked()
        {
            SaveGame();
        }

        public void OnLoadButtonClicked()
        {
            LoadGame();
        }
    }
}
