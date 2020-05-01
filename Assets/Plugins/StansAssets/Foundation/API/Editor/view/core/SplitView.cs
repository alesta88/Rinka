using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Moon.EditorTools.View
{
	/// <summary>
	/// Specifies the orientation of controls or elements of controls.
	/// </summary>
	public enum Orientation
	{
		/// <summary>
		/// The control or element is oriented horizontally.
		/// </summary>
		Horizontal,

		/// <summary>
		/// The control or element is oriented vertically.
		/// </summary>
		Vertical
	}

	/// <summary>
	/// Represents a control consisting of a movable bar (splitter) that divides a container's display area into two resizable panels.
	/// </summary>
	public class SplitView : View
	{
		private Orientation m_orientation = Orientation.Horizontal;

		protected SplitViewPanel m_panel1 = new SplitViewPanel();
		protected SplitViewPanel m_panel2 = new SplitViewPanel();

		protected Rect m_panel1Rect = new Rect();
		protected Rect m_panel2Rect = new Rect();

		protected float m_splitterSize = 2.0f;
		protected Rect m_splitterRect = Rect.zero;

		protected bool b_dragStarted = false;
		protected bool b_isFixed = false;
		protected bool b_isInitialized = false;

		protected Rect m_oldRect = Rect.zero;

		public SplitView(Orientation orientation = Orientation.Horizontal) {
			m_orientation = orientation;
		}

		public override void OnGui(Rect rect, InputEvent e) {
			if (!b_isInitialized) {
				if (m_orientation == Orientation.Horizontal) {
					m_splitterRect.xMin = ClampHorizontalSplitter(m_panel1.StartSize, rect);
					m_splitterRect.yMin = 0;
				} else {
					m_splitterRect.xMin = 0;
					m_splitterRect.yMin = ClampVerticalSplitter(m_panel1.StartSize, rect);
				}

				m_oldRect = rect;
				b_isInitialized = true;
			}

			if (m_oldRect.width != rect.width || m_oldRect.height != rect.height) {
				if (m_orientation == Orientation.Horizontal) {
					m_splitterRect.xMin = ClampHorizontalSplitter(m_splitterRect.xMin, rect);
				} else {
					m_splitterRect.yMin = ClampVerticalSplitter(m_splitterRect.yMin, rect);
				}

				m_oldRect = rect;
			}

			//Set mouse cursor for splitter rect according to Split View orientation
			if (!b_isFixed && m_splitterRect.Contains(e.mousePosition)) {
				EditorGUIUtility.AddCursorRect(m_splitterRect,
					m_orientation == Orientation.Horizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);
			}

			if (!b_isFixed && e.type == EventType.MouseDown && e.button == 0 && m_splitterRect.Contains(e.mousePosition) && EditorGUIUtility.hotControl == 0) {
				EditorGUIUtility.hotControl = GetHashCode();
				b_dragStarted = true;
			}

			if (!b_isFixed && e.type == EventType.MouseUp && e.button == 0 && EditorGUIUtility.hotControl == GetHashCode()) {
				EditorGUIUtility.hotControl = 0;
				b_dragStarted = false;
			}

			if (m_orientation == Orientation.Horizontal) {
				if (b_dragStarted && e.type == EventType.MouseDrag && EditorGUIUtility.hotControl == GetHashCode()) {
					m_splitterRect.xMin = ClampHorizontalSplitter(e.mousePosition.x, rect);
				}

				m_splitterRect.xMax = m_splitterRect.xMin + m_splitterSize;
				m_splitterRect.yMax = rect.height;

				m_panel1Rect.xMin = (int)rect.xMin;
				m_panel1Rect.xMax = (int)m_splitterRect.xMin;
				m_panel1Rect.yMin = (int)rect.yMin;
				m_panel1Rect.yMax = (int)rect.yMax;

				m_panel2Rect.xMin = (int)m_splitterRect.xMax;
				m_panel2Rect.xMax = (int)rect.xMax;
				m_panel2Rect.yMin = (int)rect.yMin;
				m_panel2Rect.yMax = (int)rect.yMax;
			} else {
				if (b_dragStarted && e.type == EventType.MouseDrag && EditorGUIUtility.hotControl == GetHashCode()) {
					m_splitterRect.yMin = ClampVerticalSplitter(e.mousePosition.y, rect);
				}

				m_splitterRect.yMax = m_splitterRect.yMin + m_splitterSize;
				m_splitterRect.xMax = rect.width;

				m_panel1Rect.xMin = (int)rect.xMin;
				m_panel1Rect.xMax = (int)rect.xMax;
				m_panel1Rect.yMin = (int)rect.yMin;
				m_panel1Rect.yMax = (int)m_splitterRect.yMin;

				m_panel2Rect.xMin = (int)rect.xMin;
				m_panel2Rect.xMax = (int)rect.xMax;
				m_panel2Rect.yMin = (int)m_splitterRect.yMax;
				m_panel2Rect.yMax = (int)rect.yMax;
			}

			m_panel1.OnGui(m_panel1Rect, e);
			DrawSplitter(m_splitterRect);
			m_panel2.OnGui(m_panel2Rect, e);
		}

		protected float ClampVerticalSplitter(float verticalPos, Rect viewRect) {
			float panel1VertizalMin = viewRect.yMin + m_panel1.MinSize - m_splitterSize / 2.0f;
			float panel2VerticalMin = viewRect.yMax - m_panel2.MaxSize + m_splitterSize / 2.0f;

			float panel1VertizalMax = viewRect.yMin + m_panel1.MaxSize - m_splitterSize / 2.0f;
			float panel2VerticalMax = viewRect.yMax - m_panel2.MinSize + m_splitterSize / 2.0f;

			float verticalMin = panel1VertizalMin >= panel2VerticalMin ? panel1VertizalMin : panel2VerticalMin;
			float verticalMax = panel1VertizalMax <= panel2VerticalMax ? panel1VertizalMax : panel2VerticalMax;

			if (verticalMin > verticalMax) {
				SwapFloats(ref verticalMin, ref verticalMax);
			}

			return Mathf.Clamp(verticalPos, verticalMin, verticalMax);
		}

		protected float ClampHorizontalSplitter(float horizontalPos, Rect viewRect) {
			float panel1HorizontalMin = viewRect.xMin + m_panel1.MinSize - m_splitterSize / 2.0f;
			float panel2HorizontalMin = viewRect.xMax - m_panel2.MaxSize + m_splitterSize / 2.0f;

			float panel1HorizontalMax = viewRect.xMin + m_panel1.MaxSize - m_splitterSize / 2.0f;
			float panel2HorizontalMax = viewRect.xMax - m_panel2.MinSize + m_splitterSize / 2.0f;

			float horizontalMin = panel1HorizontalMin >= panel2HorizontalMin ? panel1HorizontalMin : panel2HorizontalMin;
			float horizontalMax = panel1HorizontalMax <= panel2HorizontalMax ? panel1HorizontalMax : panel2HorizontalMax;

			if (horizontalMin > horizontalMax) {
				SwapFloats(ref horizontalMin, ref horizontalMax);
			}

			return Mathf.Clamp(horizontalPos, horizontalMin, horizontalMax);
		}

		protected void SwapFloats(ref float value1, ref float value2) {
			float temp = value1;
			value1 = value2;
			value2 = temp;
		}

		protected void DrawSplitter(Rect dragRect) {
			if (Event.current.type == EventType.Repaint) {
				Color color = GUI.color;
				Color b = (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.12f, 0.12f, 0.12f, 1.333f);
				GUI.color *= b;

				//Have to cast rectangle fields to int32 to make  pixel perfect
				Rect integerRect = new Rect((int)dragRect.x,
					(int)dragRect.y,
					(int)dragRect.width,
					(int)dragRect.height);

				GUI.DrawTexture(integerRect, EditorGUIUtility.whiteTexture);
				GUI.color = color;
			}
		}

		/// <summary>
		/// Gets or sets the size of a splitter in pixels. The default value is 2.0f
		/// </summary>
		public float SplitterSize {
			get {
				return m_splitterSize;
			}
			set {
				m_splitterSize = value;
			}
		}

		/// <summary>
		/// Gets the left or top panel of the SplitView, depending on Orientation.
		/// </summary>
		public SplitViewPanel Panel1 {
			get {
				return m_panel1;
			}
		}

		/// <summary>
		/// Gets the right or bottom panel of the SplitView, depending on Orientation.
		/// </summary>
		public SplitViewPanel Panel2 {
			get {
				return m_panel2;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the splitter is fixed or movable.
		/// The default value is false.
		/// </summary>
		public bool IsFixed {
			get {
				return b_isFixed;
			}
			set {
				b_isFixed = value;
			}
		}

		/// <summary>
		/// Gets a value indicating the horizontal or vertical orientation of the SplitView panels.
		/// </summary>
		public Orientation Orientation {
			get {
				return m_orientation;
			}
		}
	}
}
