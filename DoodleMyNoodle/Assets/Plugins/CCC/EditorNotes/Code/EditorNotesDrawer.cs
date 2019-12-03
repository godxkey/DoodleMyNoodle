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
        // Show window if it wasn't already displayed
        if (s_noteWindow == null)
        {
            EditorWindow previousFocus = EditorWindow.focusedWindow;

            s_noteWindow = DraggablePopupWindow.GetDraggableWindow<EditorNotesWindow>();
            s_noteWindow.titleContent = new GUIContent("Note");
            s_noteWindow.ShowAt(selectionRect, false);

            if (previousFocus != s_noteWindow) // focuse previous window (unity always focuses the new...)
            {
                previousFocus.Focus();
            }
        }

        s_noteWindow.TargetPosition = GUIUtility.GUIToScreenRect(selectionRect).position;
        s_noteWindow.Repaint();
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

        Rect iconRect = new Rect()
        {
            width = EditorNotesSettings.NOTE_ICON_WIDTH,
            height = EditorNotesSettings.NOTE_ICON_HEIGHT,
            x = selectionRect.xMax - EditorNotesSettings.NOTE_ICON_WIDTH - 5,
            y = selectionRect.y
        };

        GUI.DrawTexture(iconRect, EditorNotesViewData.NoteIconTexture);
    }
}