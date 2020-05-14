using UnityEngine;
using UnityEditor;
using System;

namespace SA.Foundation.Editor
{
	public class ISN_GuiBeginVertical : IDisposable
	{
		public ISN_GuiBeginVertical(params GUILayoutOption[] layoutOptions) {
			EditorGUILayout.BeginVertical(layoutOptions);
		}

        public ISN_GuiBeginVertical(GUIStyle style,  params GUILayoutOption[] layoutOptions) {
            EditorGUILayout.BeginVertical(style, layoutOptions);
        }

        public void Dispose() {
			EditorGUILayout.EndVertical();
		}
	}
}