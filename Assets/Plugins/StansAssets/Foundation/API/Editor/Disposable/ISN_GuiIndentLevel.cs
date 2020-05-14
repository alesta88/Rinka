using UnityEngine;
using UnityEditor;
using System;


namespace SA.Foundation.Editor
{
    public class ISN_GuiIndentLevel : IDisposable
    {

        private int m_indent = 1;
        public ISN_GuiIndentLevel(int indent) {
            m_indent = indent;
            EditorGUI.indentLevel += m_indent;

        }

        public void Dispose() {
            EditorGUI.indentLevel -= m_indent;
        }
    }


}