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
    static bool s_requestRepaint;

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

        if (s_requestRepaint)
        {
            EditorApplication.RepaintProjectWindow(); // needed for responsive UI
            s_requestRepaint = false;
        }
    }

    static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
    {
        string noteId = guid;

        bool isHoldingRequiredKeys = (Event.current.modifiers & EditorNotesSettings.MODIFIER_KEY) != EventModifiers.None;

        if (isHoldingRequiredKeys) // user pressed the correct key
        {
            if (selectionRect.Contains(Event.current.mousePosition)) // we're drawing the item that the user is pointing on
            {
                // _________________________________________ Begin / End note _________________________________________ //
                if (!EditorNotesViewData.IsEditingNote)
                {
                    s_noteId.Set(noteId);

                    if (s_noteId.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(s_noteId.GetPrevious()))
                        {
                            OnEndNote(s_noteId.GetPrevious());
                        }

                        OnBeginNote(s_noteId.Get());

                        s_noteId.Reset();
                    }
                }


                s_lastDrawTime = EditorApplication.timeSinceStartup;
            }
        }
        else if (CanEndNoteAndClose())
        {
            EndNoteAndClose();
        }


        // _________________________________________ Draw _________________________________________ //
        if (s_noteId.Get() == noteId)
        {
            EditorNotesDrawer.DrawNote(selectionRect);
            EditorNotesDrawer.DrawSelectionBorder(selectionRect);

            if (!EditorNotesViewData.IsEditingNote)
            {
                EditorNotesDrawer.DrawEditNoteButton(selectionRect);
            }
        }

        if (EditorNotesDatabase.Instance.ContainsNote(noteId))
        {
            EditorNotesDrawer.DrawNoteIcon(selectionRect);
        }



        s_requestRepaint = isHoldingRequiredKeys || EditorNotesViewData.IsEditingNote; // needed for responsive UI
    }

    static bool CanEndNoteAndClose()
    {
        if (s_noteId.Get() == null)
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
        if (s_noteId.Get() != null)
        {
            OnEndNote(s_noteId.Get());
        }
        EditorNotesDrawer.CloseDrawings();
        s_noteId.Set(null);
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
            if (string.IsNullOrEmpty(EditorNotesViewData.NoteText))
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
