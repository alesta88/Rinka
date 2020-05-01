using UnityEngine;
using UnityEditor;
using System;

namespace SA.Foundation.Editor
{
	public class ISN_GuiChangeColor : IDisposable
	{
        private Color m_previousColor { get; set; }

		public ISN_GuiChangeColor(Color newColor) {
			m_previousColor = GUI.color;
            GUI.color = newColor;
		}

		public void Dispose() {
            GUI.color = m_previousColor;
		}
	}
}