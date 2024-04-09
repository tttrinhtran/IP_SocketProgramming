using TMPro;
using UnityEngine;

public class ButtonGenerator : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonParent;

    void Start()
    {
        Debug.Log("Generating buttons...");
        GenerateButtons();
    }



    void GenerateButtons()
    {
        int buttonsPerRow = 13;
        int rowCount = 2;

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < buttonsPerRow; col++)
            {
                char letter = (char)('A' + (row * buttonsPerRow) + col);
                // Debug.Log("Generating button for letter: " + letter);

                GameObject buttonGO = Instantiate(buttonPrefab, buttonParent.transform); // Set the parent transform
                RectTransform buttonTransform = buttonGO.GetComponent<RectTransform>();

                TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText == null)
                {
                    Debug.LogError("TextMeshPro component not found in prefab's children.");
                    return;
                }

                buttonText.text = letter.ToString();
                // You can add functionality to the button here if needed
            }
        }
    }

}
