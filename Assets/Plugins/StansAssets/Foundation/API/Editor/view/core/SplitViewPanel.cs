using UnityEngine;

namespace Moon.EditorTools.View
{
	/// <summary>
	/// Creates a panel that is associated with a SplitView.
	/// </summary>
	public sealed class SplitViewPanel : IView
	{
		private int m_size = 0;

		private int m_minSize = 0;
		private int m_maxSize = int.MaxValue;
		private int m_startSize = 100;

		public void OnGui(Rect rect, InputEvent e) {
			m_size = (int)rect.width;

			if (View == null) {
				//Nothing to render, just return
				return;
			}

			GUI.BeginGroup(rect);
			Rect rc = new Rect(0.0f, 0.0f, rect.width, rect.height);
			View.OnGui(rc, e);
			GUI.EndGroup();
		}

		/// <summary>
		/// Sets the view contained within the SplitViewPanel
		/// </summary>
		/// <param name="view">The view contained within the SplitViewPanel</param>
		public void SetView(IView view) {
			View = view;
		}

		/// <summary>
		/// Gets or sets the minimum width or height of the panel in pixels depending on the SplitView Orientation.
		/// The default value is 0 pixels, regardless of SplitView Orientation.
		/// </summary>
		public int MinSize {
			get {
				return m_minSize;
			}
			set {
				m_minSize = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum width or height of the panel in pixels depending on the SplitView Orientation.
		/// The default value is int.MaxValue pixels, regardless of SplitView Orientation.
		/// </summary>
		public int MaxSize {
			get {
				return m_maxSize;
			}
			set {
				m_maxSize = value;
			}
		}

		/// <summary>
		/// Gets the view contained within the SplitViewPanel
		/// </summary>
		public IView View { get; private set; }

		/// <summary>
		/// Gets current width or height of a panel depending on the SplitView Orientation.
		/// </summary>
		public int Size {
			get {
				return m_size;
			}
		}

		/// <summary>
		/// Gets or sets the start width or height of the panel depending on the SplitView Orientation.
		/// <para>Important: Make sure that this value is between MinSize and MaxSize of the panel.</para>
		/// </summary>
		public int StartSize {
			get {
				return m_startSize;
			}
			set {
				m_startSize = value;
			}
		}
	}
}
