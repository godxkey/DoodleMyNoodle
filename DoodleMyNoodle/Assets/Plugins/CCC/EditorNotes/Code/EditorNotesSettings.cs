using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNotesSettings
{
    public const EventModifiers MODIFIER_KEY = EventModifiers.Alt;

    public const string DEFAULT_NOTE_TEXT = "Add note here!";

    public const string FOLDER_ROOT = "Assets/Plugins/CCC/EditorNotes";
    public const string FOLDER_DATABASE = FOLDER_ROOT + "/Database";
    public const string FOLDER_CODE = FOLDER_ROOT + "/Code";
    public const string FOLDER_TEXTURE = FOLDER_ROOT + "/Textures";

    public const string ASSET_PATH_DATABASE = FOLDER_DATABASE + "/NotesDatabase.asset";
    public const string ASSET_PATH_NOTE_ICON = FOLDER_TEXTURE + "/editorNotes_noteIcon.png";
    public const string ASSET_PATH_WINDOW_BACKGROUND = FOLDER_TEXTURE + "/editorNotes_windowBG.png";
    public const string ASSET_PATH_SELECTION_BORDER = FOLDER_TEXTURE + "/editorNotes_selectionBorder.png";

    public const float NOTE_WINDOW_MIN_HEIGHT = 44;
    public const float NOTE_WINDOW_WIDTH = 250;

    public const float NOTE_ICON_WIDTH = 16;
    public const float NOTE_ICON_HEIGHT = 16;
}
