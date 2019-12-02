using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Contains all view-related data
/// </summary>
public class EditorNotesViewData
{
    public static string NoteText;
    public static bool IsEditingNote;

    private static Texture2D s_cachedNoteIconTexture;
    public static Texture2D NoteIconTexture
    {
        get
        {
            if (s_cachedNoteIconTexture == null)
                s_cachedNoteIconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorNotesSettings.TEXTURE_FOLDER + "/editorNotes_noteIcon.png");

            return s_cachedNoteIconTexture;
        }
    }

    private static Texture2D s_cachedWindowBGTexture;
    public static Texture2D WindowBGTexture
    {
        get
        {
            if (s_cachedWindowBGTexture == null)
                s_cachedWindowBGTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorNotesSettings.TEXTURE_FOLDER + "/editorNotes_windowBG.png");

            return s_cachedWindowBGTexture;
        }
    }

    private static Texture2D s_cachedSelectionBorderTexture;
    public static Texture2D SelectionBorderTexture
    {
        get
        {
            if (s_cachedSelectionBorderTexture == null)
                s_cachedSelectionBorderTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorNotesSettings.TEXTURE_FOLDER + "/editorNotes_selectionBorder.png");

            return s_cachedSelectionBorderTexture;
        }
    }
}
