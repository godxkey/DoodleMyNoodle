using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorNotesDrawer
{
    static EditorNotesWindow s_noteWindow;

    public static void DrawNote(Rect selectionRect)
    {
        Rect screenSpaceSelectionRect = GUIUtility.GUIToScreenRect(selectionRect);

        // TODO: this should resize to the size of the text
        const float WINDOW_HEIGHT = 46;
        Rect noteBox = new Rect()
        {
            width = 250,
            height = WINDOW_HEIGHT,
            x = screenSpaceSelectionRect.x,
            y = screenSpaceSelectionRect.y - WINDOW_HEIGHT - 3
        };


        // Show window if it wasn't already displayed
        if (s_noteWindow == null)
        {
            EditorWindow previousFocus = EditorWindow.focusedWindow;

            s_noteWindow = DraggablePopupWindow.GetDraggableWindow<EditorNotesWindow>();
            s_noteWindow.titleContent = new GUIContent("Note");
            s_noteWindow.ShowAt(noteBox, false);

            if (previousFocus != s_noteWindow) // focuse previous window (unity always focuses the new...)
            {
                previousFocus.Focus();
            }
        }


        // set position if not editing
        if (!EditorNotesViewData.IsEditingNote)
        {
            s_noteWindow.position = noteBox;
        }
    }


    public static void CloseDrawings()
    {
        if (s_noteWindow != null)
        {
            s_noteWindow.Close();
            s_noteWindow = null;
        }
    }

    public static bool HasOpenedDrawings()
    {
        return s_noteWindow != null;
    }

    public static void DrawSelectionBorder(Rect selectionRect)
    {
        Rect selectionBorderBox = new Rect(selectionRect);
        selectionBorderBox.x -= 4;

        GUI.DrawTexture(selectionBorderBox, EditorNotesViewData.SelectionBorderTexture, ScaleMode.StretchToFill);
    }

    public static void DrawEditNoteButton(Rect selectionRect)
    {
        // invisible button drawn above the hovered asset
        if (GUI.Button(selectionRect, GUIContent.none, GUIStyle.none))
        {
            s_noteWindow.Focus();
        }
    }

    public static void DrawNoteIcon(Rect selectionRect)
    {
        // icon drawn to indicate that an asset has a note

        const int NOTE_ICON_SIZE = 16;
        const int SELECTION_RECT_PADDING = 5;

        Rect buttonBox = new Rect()
        {
            width = NOTE_ICON_SIZE,
            height = NOTE_ICON_SIZE,
            x = selectionRect.xMax - NOTE_ICON_SIZE - SELECTION_RECT_PADDING,
            y = selectionRect.y
        };

        GUI.DrawTexture(buttonBox, EditorNotesViewData.NoteIconTexture);
    }
}