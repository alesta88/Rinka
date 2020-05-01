using UnityEngine;

public static class PlayerPrefsUtil {
    public static void SetBool( string key, bool val ) {
        int intVal = ( val == true ) ? 1 : 0;
        PlayerPrefs.SetInt( key, intVal );
    }

    public static bool GetBool( string key, bool defaultVal = false ) {
        int intVal = PlayerPrefs.GetInt( key, -1 );
        return (intVal == -1) ? defaultVal : intVal == 1;
    }
}
