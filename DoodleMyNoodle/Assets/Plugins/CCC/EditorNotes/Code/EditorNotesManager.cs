using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad] // causes the static constructor to get called
public class EditorNotesManager
{
    static EditorNotesManager()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        EditorApplication.update += Update;
    }

    static double s_lastDrawTime;
    static DirtyValue<string> s_noteId;

    private static void Update()
    {
        // This will make sure that the 'Note' display is closed if the mouse exits the window
        if (EditorApplication.timeSinceStartup - s_lastDrawTime > 0.2f)
        {
            if (CanEndNoteAndClose())
            {
                EndNoteAndClose();
            }
        }
    }

    static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
    {
        string noteId = guid;

        if ((Event.current.modifiers & EditorNotesSettings.MODIFIER_KEY) != EventModifiers.None) // user pressed the correct key
        {
            if (selectionRect.Contains(Event.current.mousePosition)) // we're drawing the item that the user is pointing on
            {
                // _________________________________________ Begin / End note _________________________________________ //
                if (!EditorNotesViewData.IsEditingNote)
                {
                    s_noteId.Value = noteId;

                    if (s_noteId.IsDirty)
                    {
                        if (!s_noteId.PreviousValue.IsNullOrEmpty())
                        {
                            OnEndNote(s_noteId.PreviousValue);
                        }

                        OnBeginNote(s_noteId.Value);

                        s_noteId.Reset();
                    }
                }

                // _________________________________________ Draw _________________________________________ //
                EditorNotesDrawer.DrawNote(selectionRect);
                if (!EditorNotesViewData.IsEditingNote)
                {
                    EditorNotesDrawer.DrawEditNoteButton(selectionRect);
                }


                EditorApplication.RepaintProjectWindow(); // needed for responsive UI

                s_lastDrawTime = EditorApplication.timeSinceStartup;
            }
        }
        else if (CanEndNoteAndClose())
        {
            EndNoteAndClose();
        }


        if(s_noteId.Value == noteId)
        {
            EditorNotesDrawer.DrawSelectionBorder(selectionRect);
        }

        if (EditorNotesDatabase.Instance.ContainsNote(noteId))
        {
            EditorNotesDrawer.DrawNoteIcon(selectionRect);
        }
    }

    static bool CanEndNoteAndClose()
    {
        if (s_noteId.Value == null)
        {
            return false; // the note is already null, there's nothing to close
        }

        if (EditorNotesViewData.IsEditingNote)
        {
            return false; // we're editng a note, wait !
        }

        return true;
    }

    static void EndNoteAndClose()
    {
        if (s_noteId.Value != null)
        {
            OnEndNote(s_noteId.Value);
        }
        EditorNotesDrawer.CloseDrawings();
        s_noteId.Value = null;
        s_noteId.Reset();
    }

    static void OnBeginNote(string id)
    {
        EditorNotesViewData.NoteText = EditorNotesDatabase.Instance.GetNote(id);
    }

    static void OnEndNote(string id)
    {
        EditorNotesDatabase database = EditorNotesDatabase.Instance;

        if (EditorNotesViewData.NoteText != database.GetNote(id))
        {
            if (EditorNotesViewData.NoteText.IsNullOrEmpty())
            {
                database.DeleteNote(id);
            }
            else
            {
                database.SetOrAddNote(id, EditorNotesViewData.NoteText);
            }

            database.Save();
            Debug.Log($"Updated note '{EditorNotesViewData.NoteText}' for asset {id}");
        }
    }

}
