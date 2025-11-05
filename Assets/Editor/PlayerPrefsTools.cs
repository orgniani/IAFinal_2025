using UnityEditor;
using UnityEngine;

public static class PlayerPrefsTools
{
    [MenuItem("Tools/PlayerPrefs/Wipe All PlayerPrefs")]
    public static void WipeAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog(
            "Wipe All PlayerPrefs",
            "Are you sure you want to permanently delete all PlayerPrefs?",
            "Yes, wipe them",
            "Cancel"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("[PlayerPrefsTools] All PlayerPrefs have been wiped.");
        }
    }
}
