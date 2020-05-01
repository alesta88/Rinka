using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SA.Foundation.Editor
{
    public static class SA_ReorderablList
    {
        private static Dictionary<int, bool> s_globalFoldoutItemsState = new Dictionary<int, bool>();

        public delegate string ItemName<T>(T item);
        public delegate void ItemContent<T>(T item);
        public delegate void OnItemAdd();


        public static void Draw<T>(IList<T> list, ItemName<T> itemName, ItemContent<T> itemContent = null, OnItemAdd onItemAdd = null) {


            if (onItemAdd != null) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                bool add = GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20));
                if (add) {
                    onItemAdd();
                    return;
                }
                GUILayout.Space(5);
                EditorGUILayout.EndHorizontal();
            }

            if (itemContent != null) {
                DrawFoldout(list, itemName, itemContent);
            } else {
                DrawLabel(list, itemName);
            }
        }


        private static void DrawFoldout<T>(IList<T> list, ItemName<T> itemName, ItemContent<T> itemContent) {
            for (int i = 0; i < list.Count; i++) {
                var item = list[i];
                EditorGUILayout.BeginVertical(BoxStyle);
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();


                bool foldState = GetFoldoutState(item);
                foldState = EditorGUILayout.Foldout(foldState, itemName(item), true);
                SetFoldoutState(item, foldState);

                bool ItemWasRemoved = DrawButtons(item, list);
                if (ItemWasRemoved) {
                    return;
                }
                EditorGUILayout.EndHorizontal();

                if (foldState) {
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.Space();
                        itemContent(item);
                        EditorGUILayout.Space();
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }

        private static void DrawLabel<T>(IList<T> list, ItemName<T> itemName) {
            foreach (var item in list) {
                EditorGUILayout.BeginVertical(BoxStyle);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.SelectableLabel(itemName(item), GUILayout.Height(16));
 

                bool ItemWasRemoved = DrawButtons(item, list);
                if (ItemWasRemoved) {
                    return;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }


        private static GUIStyle s_boxStyle = null;
        private static GUIStyle BoxStyle {
            get {
                if (s_boxStyle == null) {
                    s_boxStyle = new GUIStyle(GUI.skin.box);

                    //  s_boxStyle.padding = new RectOffset(0, 0, 0, 0);
                    // s_boxStyle.margin = new RectOffset(0, 0, 0, 0);
                }

                return s_boxStyle;
            }
        }

        private static bool GetFoldoutState(object item) {
            if(item == null) {
                return false;
            }
            if (s_globalFoldoutItemsState.ContainsKey(item.GetHashCode())) {
                return s_globalFoldoutItemsState[item.GetHashCode()];
            } else {
                return false;
            }
        }

        private static void SetFoldoutState(object item, bool value) {
            if(item == null) {
                return;
            }
            s_globalFoldoutItemsState[item.GetHashCode()] = value;
        }


        private static bool DrawButtons<T>(T currentObject, IList<T> ObjectsList) {

            int ObjectIndex = ObjectsList.IndexOf(currentObject);
            if (ObjectIndex == 0) {
                GUI.enabled = false;
            }

            bool up = GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
            if (up) {
                T c = currentObject;
                ObjectsList[ObjectIndex] = ObjectsList[ObjectIndex - 1];
                ObjectsList[ObjectIndex - 1] = c;
            }


            if (ObjectIndex >= ObjectsList.Count - 1) {
                GUI.enabled = false;
            } else {
                GUI.enabled = true;
            }

            bool down = GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
            if (down) {
                T c = currentObject;
                ObjectsList[ObjectIndex] = ObjectsList[ObjectIndex + 1];
                ObjectsList[ObjectIndex + 1] = c;
            }


            GUI.enabled = true;
            bool r = GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
            if (r) {
                ObjectsList.Remove(currentObject);
            }

            return r;
        }

    }
}