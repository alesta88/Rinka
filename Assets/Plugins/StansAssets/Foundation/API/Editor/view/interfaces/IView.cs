using UnityEngine;
using UnityEditor;

namespace Moon.EditorTools.View
{
	public interface IView
	{
		void OnGui(Rect rect, InputEvent e);
	}
}