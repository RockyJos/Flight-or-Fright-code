#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

public class TextInputDialog : EditorWindow
{
    private string input = "";
    private string message = "";
    private bool accepted = false;
    private System.Action<string> onAccept;
    private Texture2D leftImage;
    private string errorMessage = "";

    public static void Show(string title, string message, System.Action<string> onAccept)
    {
        Texture2D image = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Core/Editor/GaW_Pad_Circle.png");
        TextInputDialog window = CreateInstance<TextInputDialog>();
        window.titleContent = new GUIContent(title);
        window.minSize = new Vector2(400, 140);
        window.maxSize = new Vector2(400, 140);
        window.message = message;
        window.leftImage = image;
        window.onAccept = onAccept;

        Rect main = EditorGUIUtility.GetMainWindowPosition();
        float centerX = main.x + (main.width - window.minSize.x) / 2;
        float centerY = main.y + (main.height - window.minSize.y) / 2;
        window.position = new Rect(centerX, centerY, window.minSize.x, window.minSize.y);

        window.ShowModalUtility();
        window.Focus();
    }

    private void OnGUI()
    {
        bool invokeAccept = false;
        GUILayout.BeginHorizontal();

        if (leftImage != null)
        {
            GUILayout.Label(leftImage, GUILayout.Width(100), GUILayout.Height(100));
        }

        GUILayout.BeginVertical();

        GUILayout.Label(message, EditorStyles.wordWrappedLabel);
        if (errorMessage != null && errorMessage.Length > 0)
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
        }

        GUI.SetNextControlName("InputField");
        input = EditorGUILayout.TextField(input);

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            if (Regex.IsMatch(input, @"^\d{8}$"))
            {
                invokeAccept = true;
            }
            else
            {
                errorMessage = "Invalid student ID. Please enter a valid student ID number.";
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        if (Event.current.type == EventType.Layout)
        {
            EditorGUI.FocusTextInControl("InputField");
        }

        if (invokeAccept)
        {
            accepted = true;
            onAccept?.Invoke(input);
            Close();
        }
    }

    private void OnDisable()
    {
        if (!accepted)
        {
            Show(titleContent.text, message, onAccept);
        }
    }
}
#endif