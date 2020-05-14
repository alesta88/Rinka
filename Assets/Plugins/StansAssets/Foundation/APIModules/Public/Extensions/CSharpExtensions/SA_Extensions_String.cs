using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation;

public static class SA_Extensions_String  {

    public static string GetLast(this string source, int count) {
        if (count >= source.Length)
            return source;
        return source.Substring(source.Length - count);
    }

    public static string GetFirst(this string source, int count) {


        if (count >= source.Length)
            return source;
        return source.Substring(0, count);
    }

    public static void CopyToClipboard(this string source) {
        TextEditor te = new TextEditor();
        te.text = source;
        te.SelectAll();
        te.Copy();
    }
    public static System.Uri CovertToURI(this string source) {
        return new System.Uri(source);
    }

}
