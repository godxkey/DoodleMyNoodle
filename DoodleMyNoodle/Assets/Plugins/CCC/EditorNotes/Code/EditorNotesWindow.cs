using UnityEditor;
using UnityEngine;

/// <summary>
/// Window used to display/edit the note
/// </summary>
public class EditorNotesWindow : DraggablePopupWindow
{
    bool _forceFocusOnTextArea;
    public Vector2 TargetPosition;

    protected override void OnGUI()
    {
        base.OnGUI();

        AdjustWindowRect();

        // background
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height), EditorNotesViewData.WindowBGTexture, ScaleMode.StretchToFill);

        if (EditorNotesViewData.IsEditingNote)
        {
            // close the window if the user pressed certain keys
            if (Event.current.type == EventType.KeyDown)
            {
                KeyCode keycode = Event.current.keyCode;

                if ((keycode == KeyCode.Return && !Event.current.shift)
                    || (keycode == KeyCode.KeypadEnter && !Event.current.shift)
                    || keycode == KeyCode.Escape)
                {
                    OnLostFocus();
                    Close();
                    Event.current.Use();
                    return;
                }
            }

            // draw text area
            GUI.SetNextControlName("TextArea");
            EditorNotesViewData.NoteText = GUILayout.TextArea(EditorNotesViewData.NoteText);

            // force focus on text area if needed
            if (_forceFocusOnTextArea)
            {
                GUI.FocusControl("TextArea");
                _forceFocusOnTextArea = false;
            }

            GUILayout.Label("Shift+Enter for new line", EditorStyles.miniLabel);

        }
        else
        {
            bool oldWrap = EditorStyles.label.wordWrap;
            bool oldStrechHeight = EditorStyles.label.stretchHeight;

            if (EditorNotesViewData.NoteText == null)
            {
                GUILayout.Label(EditorNotesSettings.DEFAULT_NOTE_TEXT, EditorStyles.helpBox);
            }
            else
            {
                EditorStyles.label.wordWrap = true;
                EditorStyles.label.stretchHeight = true;
                GUILayout.Label(EditorNotesViewData.NoteText, EditorStyles.label);
            }

            EditorStyles.label.wordWrap = oldWrap;
            EditorStyles.label.stretchHeight = oldStrechHeight;
        }

    }

    void AdjustWindowRect()
    {
        // calculate text height
        float textHeight = EditorStyles.textArea.CalcHeight(new GUIContent(EditorNotesViewData.NoteText), EditorNotesSettings.NOTE_WINDOW_WIDTH - 8);

        float height = Mathf.Max(textHeight + 5, EditorNotesSettings.NOTE_WINDOW_MIN_HEIGHT);
        float width = EditorNotesSettings.NOTE_WINDOW_WIDTH;

        Rect noteBox = new Rect()
        {
            width = width,
            height = height,
            x = TargetPosition.x,
            y = TargetPosition.y - height - 3
        };

        position = noteBox;
    }

    private void OnFocus()
    {
        EditorNotesViewData.IsEditingNote = true;
        _forceFocusOnTextArea = true;
    }

    private void OnLostFocus()
    {
        EditorNotesViewData.IsEditingNote = false;
    }
}