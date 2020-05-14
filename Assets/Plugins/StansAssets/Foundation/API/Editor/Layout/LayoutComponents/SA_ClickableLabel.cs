using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Events;


namespace SA.Foundation.Editor
{
    public class SA_ClickableLabel 
    {

       
        private bool m_isSelected = false;
        private bool m_isMouseOver = false;
        private bool m_isHighlighted = false;
        private Rect m_labelRect = new Rect();
        private Color m_selectionColor = Color.white;


        private GUIStyle m_style;
        private GUIStyle m_highlightedStyle;
        private GUIContent m_content;


        private SA_Event m_onClick = new SA_Event();

        public SA_ClickableLabel(string label, Color selectedColor, GUIStyle style = null)
            :this(new GUIContent(label), selectedColor, style) {
        }

        public SA_ClickableLabel(GUIContent content, Color selectionColor, GUIStyle style = null, GUIStyle highlightedStyle = null) {

            if(style == null) {
                style = new GUIStyle();
            }

            m_style =  new GUIStyle(style);

            if(highlightedStyle == null) {
                highlightedStyle = new GUIStyle(m_style);
                highlightedStyle.normal.textColor = selectionColor;
                m_highlightedStyle = highlightedStyle;
            }

            m_content = content;
            m_selectionColor = selectionColor;
        }



        public void Draw(params GUILayoutOption[] options) {

            
            if (m_isSelected) {
                m_isHighlighted = true;
            }


            if(m_isHighlighted) {
                using(new ISN_GuiChangeColor(m_selectionColor)) {
                    EditorGUILayout.LabelField(m_content, m_highlightedStyle, options);
                }
            } else {
                EditorGUILayout.LabelField(m_content, m_style, options);
            }


            if (Event.current.type == EventType.Repaint) {
                m_labelRect = GUILayoutUtility.GetLastRect();
                m_isMouseOver = m_labelRect.Contains(Event.current.mousePosition);
            }


            if (m_isSelected) {
                return;
            }

            if (Event.current.type == EventType.Repaint) {
                if (m_isMouseOver) {
                    m_isHighlighted = true;
                    EditorGUIUtility.AddCursorRect(m_labelRect, MouseCursor.Link);
                } else {
                    m_isHighlighted = false;
                }
            }

            if(m_isMouseOver) {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                    m_onClick.Invoke();
                }
            }
        }



        private bool IsMouseOver {
            get {
                return GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
            }
        }

        public bool IsSelected {
            get {
                return m_isSelected;
            }

            set {
                m_isSelected = value;
            }
        }


        public SA_iEvent OnClick {
            get {
                return m_onClick;
            }
        }


        public GUIStyle Style {
            get {
                return m_style;
            }

            set {
                m_style = value;
            }
        }

        public GUIContent Content {
            get {
                return m_content;
            }

            set {
                m_content = value;
            }
        }
    }
}