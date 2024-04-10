using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class GameplayData : MonoBehaviour
{

    private List<GameplayQuestion> gameplayQuestions = new List<GameplayQuestion>();
   
    void Start()
    {
        LoadGameplayDataFromFile();
  
    }

    private void LoadGameplayDataFromFile()
    {
        try
        {
            string fileName = "database.txt";
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                int currentIndex = 0;
                int numQuestions = int.Parse(lines[currentIndex++]);
                for (int i = 0; i < numQuestions; i++)
                {
                    string keyword = lines[currentIndex++].Trim();
                    string description = lines[currentIndex++].Trim();
                    GameplayQuestion question = new GameplayQuestion(keyword, description);
                    gameplayQuestions.Add(question);
                }
                Debug.Log("Database loaded successfully.");
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading gameplay data: " + ex.Message);
        }
    }
   public GameplayQuestion GetRandomWord()
    {
        if (gameplayQuestions.Count > 0)
        {
            // Generate a random index within the range of the list
            int randomIndex = UnityEngine.Random.Range(0, gameplayQuestions.Count);

       
            GameplayQuestion randomQuestion = gameplayQuestions[randomIndex];

            string randomKeyword = randomQuestion.Keyword;
            Debug.Log("Random keyword: " + randomKeyword);
            return randomQuestion;

           
        }
        else
        {
            Debug.LogWarning("No gameplay questions loaded.");
            return null;
        }
      
      
    }


    public class GameplayQuestion
    {
        public string Keyword { get; }
        public string Description { get; }

        public GameplayQuestion(string keyword, string description)
        {
            Keyword = keyword;
            Description = description;
        }
    }
    
}
