using UnityEditor;
using UnityEngine;

namespace Moon.EditorTools.View
{
	public abstract class View : IView
	{
		public abstract void OnGui(Rect rect, InputEvent e);
	}
}
