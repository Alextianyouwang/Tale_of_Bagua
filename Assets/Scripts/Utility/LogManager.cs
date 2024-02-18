using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public GameObject LogText;
    static int maxLines = 11;

    static int lineCount = 0;
    public static void Log(string content) 
    {
        LogManager instance = FindObjectOfType<LogManager>();
        TextMeshProUGUI logTextComponent = instance.GetComponent<TextMeshProUGUI>();

        if (instance != null)
        {
            if (logTextComponent != null)
            {
                if (lineCount >= maxLines)
                {
                    // Remove the first line
                    int firstLineEndIndex = logTextComponent.text.IndexOf('\n');
                    if (firstLineEndIndex != -1)
                    {
                        logTextComponent.text = logTextComponent.text.Substring(firstLineEndIndex + 1);
                    }
                }

                logTextComponent.text += "\n" + content;
                lineCount++;
            }
            else
            {
                Debug.LogWarning("Log Text component is not set.");
            }
        }
        else 
        {
            Debug.LogWarning("There is no LogManager found in the Scene.");
        }
 
    }
}
