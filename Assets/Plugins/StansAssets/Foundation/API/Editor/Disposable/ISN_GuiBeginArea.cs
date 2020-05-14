using System;
using UnityEngine;

namespace SA.Foundation.Editor
{
	public class ISN_GuiBeginArea : IDisposable
	{
		public ISN_GuiBeginArea(Rect area) {
			GUILayout.BeginArea(area);
		}

		public ISN_GuiBeginArea(Rect area, string content) {
			GUILayout.BeginArea(area, content);
		}

		public ISN_GuiBeginArea(Rect area, string content, string style) {
			GUILayout.BeginArea(area, content, style);
		}

		public void Dispose() {
			GUILayout.EndArea();
		}
	}
}