using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SA.Foundation.Editor { 

    public static class SA_EditorGUILayout {


        public static void ReorderablList<T>(IList<T> list, SA_ReorderablList.ItemName<T> itemName, SA_ReorderablList.ItemContent<T> itemContent = null, SA_ReorderablList.OnItemAdd onItemAdd = null) {
            SA_ReorderablList.Draw(list, itemName, itemContent, onItemAdd);
        }



        public static string StringValuePopup(string title, string value, string[] displayedOptions, string tooltip = "") {
            return StringValuePopup(new GUIContent(title, tooltip), value, displayedOptions);
        }

        public static string StringValuePopup(GUIContent content, string value, string[] displayedOptions) {

            int index = Array.IndexOf(displayedOptions, value);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(content);
            index = EditorGUILayout.Popup(index, displayedOptions);
            EditorGUILayout.EndHorizontal();

            return displayedOptions[index];
        }


        public static void Header(string header) {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(header, MessageType.None);
            EditorGUILayout.Space();
        }


        public static void HorizontalLine() {
            bool guiState = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            GUI.enabled = guiState;
        }

        public static void HorizontalLinePR() {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("", (GUIStyle)"PR Insertion", GUILayout.MaxWidth(300f));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }


        public static string TextField(string label, string text) {
            GUIContent c = new GUIContent(label, "");
            return TextField(c, text);
        }

        public static string TextField(GUIContent label, string text) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);
            text = EditorGUILayout.TextField(text);
            if (text.Length > 0) {
                text  = text.Trim();
            }
            EditorGUILayout.EndHorizontal();

            return text;
        }

        public static Enum EnumPopup(string label, Enum selected) {
            return EnumPopup(new GUIContent(label, ""), selected);
        }

        public static Enum EnumPopup(GUIContent label, Enum selected) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);
            selected = EditorGUILayout.EnumPopup(selected);
            EditorGUILayout.EndHorizontal();

            return selected;
        }


      


        public static void SelectableLabel(string title, string message) {
            GUIContent c = new GUIContent(title, "");
            SelectableLabel(c, message);
        }

        public static void SelectableLabel(GUIContent label, string message) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
            EditorGUILayout.SelectableLabel(message, GUILayout.Height(16));
            EditorGUILayout.EndHorizontal();
        }




        public static bool ToggleFiled(string title, bool value, SA_StyledToggle.ToggleType type) {
            return SA_StyledToggle.ToggleFiled(new GUIContent(title, title), value, type);
        }

        public static bool ToggleFiled(GUIContent content, bool value, SA_StyledToggle.ToggleType type) {
            return SA_StyledToggle.ToggleFiled(content, value, type);
        }



		public static void HorizontalLineThin() {
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label("", (GUIStyle)"sv_iconselector_sep", GUILayout.MaxWidth(300f));
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		}

    }
}