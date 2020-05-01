using UnityEditor;
using UnityEngine;

namespace Moon.EditorTools.View
{
	public sealed class SimpleView : IView
	{
		private GUIContent m_content;
		private Rect windowRect;

		public SimpleView(GUIContent content) {
			m_content = content;
		}

		public void OnGui(Rect rect, InputEvent e) {
			GUI.BeginGroup(rect);

			Rect rc = new Rect(0.0f, 0.0f, rect.width, rect.height);
			GUI.Box(rc, m_content);
			GUI.EndGroup();
		}
	}
}
