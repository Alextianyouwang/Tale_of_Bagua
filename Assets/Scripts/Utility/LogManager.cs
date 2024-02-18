using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public GameObject LogText;
    static int maxLines = 11;

    static int lineCount = 0;

    private static List<string> logEntries = new List<string>();

    private void OnApplicationQuit()
    {
        logEntries = null;
    }
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
                        logEntries.RemoveAt(0);
                    }
                }

                // Add new log entry to the list
                logEntries.Add(content);

                // Clear log text component
                logTextComponent.text = "";

                // Append each log entry with appropriate color
                for (int i = 0; i < logEntries.Count; i++)
                {
                    string formattedContent = i == logEntries.Count - 1 ?
                        $"<color=#F8FF30>{logEntries[i]}</color>" : logEntries[i];
                    logTextComponent.text += (i == 0 ? "" : "\n") + formattedContent;
                }

                lineCount = logEntries.Count;
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
