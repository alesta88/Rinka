using UnityEngine;
using System;

namespace SA.Foundation.Editor
{
	public class ISN_GuiChangeContentColor : IDisposable
	{
		private Color PreviousColor { get; set; }

		public ISN_GuiChangeContentColor(Color newColor) {
			PreviousColor = GUI.contentColor;
			GUI.contentColor = newColor;
		}

		public void Dispose() {
			GUI.contentColor = PreviousColor;
		}
	}
}