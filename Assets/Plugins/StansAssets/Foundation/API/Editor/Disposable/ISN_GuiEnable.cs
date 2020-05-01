using UnityEngine;
using System;

namespace SA.Foundation.Editor
{
	public class ISN_GuiEnable : IDisposable
	{
		private bool PreviousState { get; set; }

		public ISN_GuiEnable(bool newState) {
			PreviousState = GUI.enabled;
			GUI.enabled = newState;
		}

		public void Dispose() {
			GUI.enabled = PreviousState;
		}
	}
}