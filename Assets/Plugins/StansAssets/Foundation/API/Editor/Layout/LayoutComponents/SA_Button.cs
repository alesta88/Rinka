using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Events;


namespace SA.Foundation.Editor
{
    public class SA_Button 
    {

        private Texture2D m_normal;
        private Texture2D m_selected;
        private GUIStyle m_style;
        private Color m_hoverColor = Color.white;
        private Color m_normalColor = Color.white;
        private bool m_isHighlighted = false;


        private bool m_isMouseOver = false;
        private Rect m_labelRect = new Rect();

        private SA_Event m_onClick = new SA_Event();


       


        public SA_Button(Texture2D normal, Color selectionColor)
            :this(normal, null, selectionColor, null) {}

        public SA_Button(Texture2D normal, Texture2D hover)
            : this(normal, hover, Color.white, null) { }


        public SA_Button(Texture2D normal, Texture2D hover, Color selectionColor, GUIStyle style) {

            if (style == null) {
                style = new GUIStyle();
            }

            m_style = style;

            m_normal = normal;
            m_selected = hover;

            m_hoverColor = selectionColor;
        }

       

        public void SetNormalState(Texture2D normal) {
            m_normal = normal;
        }

        public void SetNormalColor(Color normalColor) {
            m_normalColor = normalColor;
        }

        public void SetHoverColor(Color hoverColor) {
            m_hoverColor = hoverColor;
        }


        public void SetHoverState(Texture2D hover) {
            m_selected = hover;
        }


        public void Draw( params GUILayoutOption[] options) {


            if(m_isHighlighted) {
                using (new ISN_GuiChangeColor(m_hoverColor)) {
                    if(m_selected != null) {
                        GUILayout.Label(m_selected, m_style, options);
                    } else {
                        GUILayout.Label(m_normal, m_style, options);
                    }
                }
            } else {
                using (new ISN_GuiChangeColor(m_normalColor)) {
                    GUILayout.Label(m_normal, m_style, options);
                }  
            }


            if (Event.current.type == EventType.Repaint) {
                m_labelRect = GUILayoutUtility.GetLastRect();
                m_isMouseOver = m_labelRect.Contains(Event.current.mousePosition);
            }


            if (Event.current.type == EventType.Repaint) {
                if (m_isMouseOver) {
                    m_isHighlighted = true;
                    EditorGUIUtility.AddCursorRect(m_labelRect, MouseCursor.Link);
                } else {
                    m_isHighlighted = false;
                }
            }

            if (m_isMouseOver) {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                    m_onClick.Invoke();
                }
            }
        }



        public SA_iEvent OnClick {
            get {
                return m_onClick;
            }
        }

    }
}