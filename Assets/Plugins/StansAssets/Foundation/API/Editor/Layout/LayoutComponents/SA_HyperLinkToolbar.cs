using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SA.Foundation.Editor
{
    public class SA_HyperLinkToolbar 
    {

        private GUIStyle m_style;
        private Color m_selectedColor;

        private List<SA_ClickableLabel> m_tabs = new List<SA_ClickableLabel>();
        private SA_ClickableLabel m_selectedTab;
        private int m_elementWidth = 0;

        private int m_lastSelectedIndex = 0;


        public SA_HyperLinkToolbar(GUIStyle style, Color selectedColor) {
            m_style = style;
            m_style.alignment = TextAnchor.MiddleCenter;
            m_selectedColor = selectedColor;
        }


        public void SetSelectedIndex(int index) {
            m_lastSelectedIndex = index;
            m_selectedTab = m_tabs[index];
            ActivateTab(m_selectedTab);
        }

        public int Draw () {


            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();
                foreach (var tab in m_tabs) {
                    var options = new List<GUILayoutOption>();
                    if (m_elementWidth != 0) {
                        options.Add(GUILayout.Width(m_elementWidth));
                    }
                    tab.Draw(options.ToArray());
                }
                EditorGUILayout.Space();
            }
          
            EditorGUILayout.EndHorizontal();


            int selectedIndex = m_tabs.IndexOf(m_selectedTab);
            if(selectedIndex != m_lastSelectedIndex) {
                if(Event.current.type == EventType.Layout) {
                    m_lastSelectedIndex = selectedIndex;
                }
            }

            return m_lastSelectedIndex;
        }

        public void AddItem(string name) {
            var tab = new SA_ClickableLabel(name, m_selectedColor, m_style);
            tab.OnClick.AddListener(() => {
                ActivateTab(tab);
            });

            m_tabs.Add(tab);


            ActivateTab(m_tabs[0]);
        }


        public void SetElementWidth(int width) {
            m_elementWidth = width;
        }


        private void ActivateTab(SA_ClickableLabel selectedTab) {
            foreach(var tab in m_tabs) {
                tab.IsSelected = false;
            }

            selectedTab.IsSelected = true;
            m_selectedTab = selectedTab;
        }


            

    }
}