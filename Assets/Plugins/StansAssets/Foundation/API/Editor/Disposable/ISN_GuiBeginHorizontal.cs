using UnityEngine;
using UnityEditor;
using System;

namespace SA.Foundation.Editor
{
	public class ISN_GuiBeginHorizontal : IDisposable
	{
		public ISN_GuiBeginHorizontal(params GUILayoutOption[] layoutOptions) {
			EditorGUILayout.BeginHorizontal(layoutOptions);
		}

		public void Dispose() {
			EditorGUILayout.EndHorizontal();
		}
	}
}