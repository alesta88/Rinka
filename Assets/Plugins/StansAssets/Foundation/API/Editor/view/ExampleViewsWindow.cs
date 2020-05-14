using UnityEditor;
using UnityEngine;
using Moon.EditorTools.View;

public class ExampleViewsWindow : EditorWindow
{
	private static ExampleViewsWindow s_window;

	private SplitView m_rootView;
	private Rect m_rootViewRect = new Rect(Vector2.zero, Vector2.zero);

	[MenuItem("Window/Example Views")]
	private static void ShowWindow() {
		s_window = ScriptableObject.CreateInstance<ExampleViewsWindow>();
		s_window.titleContent = new GUIContent("Example Views");
		s_window.minSize = new Vector2(900.0f, 700.0f);
		s_window.Show();
	}

	private void OnEnable() {
		m_rootView = new SplitView(Orientation.Horizontal);
		m_rootView.SplitterSize = 3.0f;
		m_rootView.IsFixed = true;
		m_rootView.Panel1.MinSize = 250;
		m_rootView.Panel1.StartSize = 300;
		m_rootView.Panel1.SetView(new SimpleView(new GUIContent("view 1")));

		SplitView splitView1 = new SplitView(Orientation.Vertical);
		splitView1.SplitterSize = 3.0f;
		splitView1.Panel1.MinSize = 150;
		splitView1.Panel1.StartSize = 200;
		splitView1.Panel1.SetView(new SimpleView(new GUIContent("view 3")));

		SplitView splitView3 = new SplitView(Orientation.Vertical);
		splitView3.SplitterSize = 3.0f;
		splitView3.Panel1.MinSize = 150;
		splitView3.Panel1.StartSize = 250;
		splitView3.Panel1.SetView(new SimpleView(new GUIContent("view 7")));
		splitView3.Panel2.MinSize = 150;
		splitView3.Panel2.SetView(new SimpleView(new GUIContent("view 8")));

		SplitView splitView2 = new SplitView(Orientation.Horizontal);
		splitView2.SplitterSize = 3.0f;
		splitView2.Panel1.MinSize = 150;
		splitView2.Panel1.StartSize = 300;
		splitView2.Panel1.SetView(splitView3);

		splitView2.Panel2.MinSize = 150;
		splitView2.Panel2.SetView(new SimpleView(new GUIContent("view 6")));

		splitView1.Panel2.MinSize = 300;
		splitView1.Panel2.SetView(splitView2);

		m_rootView.Panel2.MinSize = 300;
		m_rootView.Panel2.SetView(splitView1);

		EditorApplication.update += OnEditorUpdate;
	}

	private void OnDisable() {
		EditorApplication.update -= OnEditorUpdate;
	}

	private void OnEditorUpdate() {
		Repaint();
	}

	private void OnGUI() {
		m_rootViewRect.width = this.position.width;
		m_rootViewRect.height = this.position.height;

		BeginWindows();

		InputEvent e = new InputEvent(Event.current);
		m_rootView.OnGui(m_rootViewRect, e);

		EndWindows();
	}
}
