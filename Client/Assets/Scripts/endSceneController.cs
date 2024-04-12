using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endSceneController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI endText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateUI(string endText)
    {
        this.endText.text = endText;
    }
}
