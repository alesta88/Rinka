////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using SA.Foundation.Editor;
using SA.Foundation.Localization;


namespace SA.IOSDeploy {

	[CustomEditor(typeof(ISD_Settings))]
	public class ISD_SettingsInspector : Editor {

		private static string NewPlistValueName 	= string.Empty;
		private static string NewValueName 			= string.Empty;



		public static int NewBaseFrameworkIndex = 0;
		public static int NewLibraryIndex = 0;

		private static bool GUI_ENABLED  = true;

		private static GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	

		private int _Width = 500;
		public int Width {
			get {
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				Rect scale = GUILayoutUtility.GetLastRect();

                if (scale.width > 1f) {
					_Width = System.Convert.ToInt32(scale.width);
				}

				return _Width;
			}
		}
			




		private Texture[] _ToolbarImages = null;

		public Texture[] ToolbarImages {
			get {
				if(_ToolbarImages == null) {
                    Texture2D buildSettings 	= SA_EditorAssets.GetTextureAtPath (ISD_Settings.IOS_DEPLOY_FOLDER + "Editor/Icons/BuildSettings.png"); 
                    Texture2D framework		 	= SA_EditorAssets.GetTextureAtPath (ISD_Settings.IOS_DEPLOY_FOLDER + "Editor/Icons/frameworks.png");
                    Texture2D languages 		= SA_EditorAssets.GetTextureAtPath (ISD_Settings.IOS_DEPLOY_FOLDER + "Editor/Icons/languages.png");
                    Texture2D libraries 		= SA_EditorAssets.GetTextureAtPath (ISD_Settings.IOS_DEPLOY_FOLDER + "Editor/Icons/Libraries.png");
                    Texture2D linkerFlags 		= SA_EditorAssets.GetTextureAtPath (ISD_Settings.IOS_DEPLOY_FOLDER + "Editor/Icons/linkerFlags.png");		
                    Texture2D plistVariables 	= SA_EditorAssets.GetTextureAtPath (ISD_Settings.IOS_DEPLOY_FOLDER + "Editor/Icons/plistVariables.png");

					List<Texture2D> textures =  new List<Texture2D>();
					textures.Add(buildSettings);
					textures.Add(framework);
					textures.Add(libraries);
					textures.Add(linkerFlags);
					textures.Add(plistVariables);
					textures.Add(languages);

					_ToolbarImages = textures.ToArray();

				}
				return _ToolbarImages;
			}
		}

		public override void OnInspectorGUI () {
			GUI.changed = false;
			EditorGUILayout.LabelField("IOS Deploy Settings", EditorStyles.boldLabel);
			EditorGUILayout.Space();


			GUI.SetNextControlName ("toolbar");
		
			GUILayoutOption[] toolbarSize = new GUILayoutOption[]{GUILayout.Width(Width-10), GUILayout.Height(35)};
			ISD_Settings.Instance.ToolbarIndex = GUILayout.Toolbar(ISD_Settings.Instance.ToolbarIndex, ToolbarImages, toolbarSize);



			EditorGUILayout.Space();
			switch (ISD_Settings.Instance.ToolbarIndex) {
			case 0:
				BuildSettings ();
                Capabilities();
                FilesSettings();
				break;
			case 1:
				Frameworks();
				break;
			case 2:
				Library ();
				break;
			case 3:
				Flags();
				break;
			case 4: 
				PlistValues ();
                ShellScripts();
				break;

			case 5: 
				LanguageValues();
				break;

			}

			EditorGUILayout.Space();
			AboutGUI();


			if(GUI.changed) {
                ISD_Settings.Save();
			}
		}

		public static void BuildSettings(){

			SA_EditorGUILayout.Header ("Build Settings");
            foreach(var property in ISD_Settings.Instance.BuildProperties) {
                property.Value = SA_EditorGUILayout.StringValuePopup(property.Name, property.Value, property.Options);
            }

		}


        private static ISD_CapabilityType s_newCap;
        public static void Capabilities() {
            SA_EditorGUILayout.Header("Capabilities");

            SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Capabilities,
                (ISD_Capability c) => {
                    return c.CapabilityType.ToString();
                },
                (ISD_Capability c) => {

                switch(c.CapabilityType) {
                    case ISD_CapabilityType.PushNotifications:
                            var settings = ISD_Settings.Instance.PushNotificationsCapabilitySettings;
                            settings.Development = SA_EditorGUILayout.ToggleFiled("Development", settings.Development, SA_StyledToggle.ToggleType.YesNo);
                        break;
                    case ISD_CapabilityType.Cloud:
                        var cloudSettings = ISD_Settings.Instance.iCloudCapabilitySettings;
                        cloudSettings.KeyValueStorage = SA_EditorGUILayout.ToggleFiled("Key-value storage", cloudSettings.KeyValueStorage, SA_StyledToggle.ToggleType.EnabledDisabled);
                        cloudSettings.iCloudDocument = SA_EditorGUILayout.ToggleFiled("iCloud Documents", cloudSettings.iCloudDocument, SA_StyledToggle.ToggleType.EnabledDisabled);
           
                        break;
                    default:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Entitlements File Path");
                        c.EntitlementsFilePath = EditorGUILayout.TextField(c.EntitlementsFilePath);
                        EditorGUILayout.EndHorizontal();

                        c.AddOptionalFramework = SA_EditorGUILayout.ToggleFiled("Add Optional Framework", c.AddOptionalFramework, SA_StyledToggle.ToggleType.YesNo);
                        break;
                }
            }                          
         );


            EditorGUILayout.BeginHorizontal();
            s_newCap = (ISD_CapabilityType)EditorGUILayout.EnumPopup(s_newCap);
            if (GUILayout.Button("Add Capability", GUILayout.Height(20))) {
                ISD_Capability c = new ISD_Capability();
                c.CapabilityType = s_newCap;
                ISD_Settings.Instance.Capabilities.Add(c);
            }
            EditorGUILayout.EndHorizontal();
        }

		

		public static void Frameworks() {

			SA_EditorGUILayout.Header ("FRAMEWORKS");

			EditorGUILayout.BeginHorizontal();
			EditorGUI.indentLevel++;
			ISD_Settings.Instance.IsDefFrameworksOpen = EditorGUILayout.Foldout(ISD_Settings.Instance.IsDefFrameworksOpen, "Default Unity Frameworks (17 Enabled)");
			EditorGUI.indentLevel--;
			EditorGUILayout.EndHorizontal();

			if (ISD_Settings.Instance.IsDefFrameworksOpen) {

				EditorGUILayout.BeginVertical (GUI.skin.box);
				foreach (ISD_Framework framework in ISD_FrameworkHandler.DefaultFrameworks) {
                    SA_EditorGUILayout.SelectableLabel (framework.Type.ToString () + ".framework", "");
				}
				EditorGUILayout.EndVertical ();
				EditorGUILayout.Space ();
			}


			EditorGUILayout.Space ();

            SA_EditorGUILayout.HorizontalLine();


            EditorGUILayout.LabelField("Custom IOS Frameworks", EditorStyles.boldLabel);
            SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Frameworks,
               (ISD_Framework framework) => {
                   if (framework.IsOptional && framework.IsEmbeded) {
                        return  framework.Name + "       (Optional & Embeded)";
                   } else if (framework.IsOptional) {
                        return framework.Name + "        (Optional)";
                   } else if(framework.IsEmbeded) {
                       return framework.Name + "        (Embeded)";
                    } else{
                       return framework.Name;
                    }
                },
                (ISD_Framework framework) => {
                    framework.IsOptional = SA_EditorGUILayout.ToggleFiled("Optional", framework.IsOptional, SA_StyledToggle.ToggleType.YesNo);
                    framework.IsEmbeded = SA_EditorGUILayout.ToggleFiled("Embeded", framework.IsEmbeded, SA_StyledToggle.ToggleType.YesNo);  
                }
             );



			EditorGUILayout.BeginHorizontal();
			EditorStyles.popup.fixedHeight = 20;
            NewBaseFrameworkIndex = EditorGUILayout.Popup(NewBaseFrameworkIndex, ISD_FrameworkHandler.BaseFrameworksArray());

			if(GUILayout.Button("Add Framework",  GUILayout.Height(20))) {
				var type = ISD_FrameworkHandler.BaseFrameworksArray () [NewBaseFrameworkIndex];
				NewBaseFrameworkIndex = 0;

				ISD_Framework f =  new ISD_Framework(type);
                ISD_API.AddFramework(f);
			}

			EditorGUILayout.EndHorizontal();


			
            SA_EditorGUILayout.HorizontalLine();


            DrawEmbededBlock();
		}


        private static void DrawEmbededBlock() {

            SA_EditorGUILayout.Header("Embedded Frameworks");
            SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.EmbededFrameworks,
                (ISD_EmbedFramework framework) => {
                      return framework.FileName;
                },
                (ISD_EmbedFramework freamwork) => {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Asset: ");
                    freamwork.Asset = EditorGUILayout.ObjectField(freamwork.Asset, typeof(UnityEngine.Object), false);
                    EditorGUILayout.EndHorizontal();
                },
                () => {
                    ISD_Settings.Instance.EmbededFrameworks.Add(new ISD_EmbedFramework());
                }
             );
        }



		public static void Library () {
			SA_EditorGUILayout.Header ("LIBRARIES");


			EditorGUILayout.BeginHorizontal();
			EditorGUI.indentLevel++;
			ISD_Settings.Instance.IsDefLibrariesOpen = EditorGUILayout.Foldout(ISD_Settings.Instance.IsDefLibrariesOpen, "Default Unity Libraries (2 Enabled)");
			EditorGUI.indentLevel--;
			EditorGUILayout.EndHorizontal();

			if (ISD_Settings.Instance.IsDefLibrariesOpen) {

				EditorGUILayout.BeginVertical (GUI.skin.box);

                SA_EditorGUILayout.SelectableLabel ("libiPhone-lib.a", "");
                SA_EditorGUILayout.SelectableLabel ("libiconv.2.dylib", "");

				EditorGUILayout.EndVertical ();


				EditorGUILayout.Space ();
			}
			EditorGUILayout.Space ();
            SA_EditorGUILayout.HorizontalLine ();


			EditorGUILayout.LabelField("Custom Libraries", EditorStyles.boldLabel);	
            SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Libraries,
               (ISD_Library lib) => {
                  if (lib.IsOptional) {
                       return lib.Name + "    (Optional)";
                    } else{
                       return lib.Name;
                    }
                },                           
                (ISD_Library lib) => {
                    lib.IsOptional = SA_EditorGUILayout.ToggleFiled("Optional", lib.IsOptional, SA_StyledToggle.ToggleType.YesNo);  
                }                            
             );


			
			EditorGUILayout.BeginHorizontal ();
			EditorStyles.popup.fixedHeight = 20;
			NewLibraryIndex = EditorGUILayout.Popup(NewLibraryIndex, ISD_LibHandler.BaseLibrariesArray());

			if(GUILayout.Button("Add Library",  GUILayout.Height(20))) {
				ISD_iOSLibrary type = (ISD_iOSLibrary) ISD_LibHandler.enumValueOf( ISD_LibHandler.BaseLibrariesArray()[NewLibraryIndex]);
				NewLibraryIndex = 0;
                ISD_API.AddLibrary(type);
			}
				
			EditorGUILayout.EndHorizontal();
            SA_EditorGUILayout.HorizontalLine();

		}


		public static void Flags() {
			SA_EditorGUILayout.Header ("LINKER FLAGS");
            SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Flags,
                (ISD_Flag flag) => {
                    if (flag.Type.Equals(ISD_FlagType.CompilerFlag)) {
                        return flag.Name + "     (Compiler)";
                    } else {
                        return flag.Name + "     (Linker)";
                    }
                },
                (ISD_Flag flag) => {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Full Name:", GUILayout.Width(100));
                    flag.Name = EditorGUILayout.TextField(flag.Name, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Type:", GUILayout.Width(100));
                    //flag.Type	 	= EditorGUILayout.TextField(flag.Type, GUILayout.ExpandWidth(true));
                    flag.Type = (ISD_FlagType)EditorGUILayout.EnumPopup(flag.Type);
                    EditorGUILayout.EndHorizontal();
                },
                () => {
                    ISD_Flag newFlag = new ISD_Flag();
                    newFlag.Name = "New Flag";
                    ISD_Settings.Instance.Flags.Add(newFlag);
                }
             );
		}

		public static void PlistValues ()	{

			SA_EditorGUILayout.Header ("PLIST VALUES");




			EditorGUI.indentLevel++; {	
				foreach(ISD_PlistKey var in ISD_Settings.Instance.PlistVariables) {
					EditorGUILayout.BeginVertical (GUI.skin.box);
					DrawPlistVariable (var, (object) var, ISD_Settings.Instance.PlistVariables);
					EditorGUILayout.EndVertical ();

					if(!ISD_Settings.Instance.PlistVariables.Contains(var)) {
						return;
					}

				}
				EditorGUILayout.Space();
			} EditorGUI.indentLevel--;


			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("New Variable Name");
			NewPlistValueName = EditorGUILayout.TextField(NewPlistValueName);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if (NewPlistValueName.Length > 0) {
					ISD_PlistKey var = new ISD_PlistKey ();
					var.Name = NewPlistValueName;
                    ISD_API.SetInfoPlistKey(var);				
				}
				NewPlistValueName = string.Empty;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical ();




           

		}

        public static void FilesSettings() {


            SA_EditorGUILayout.Header("Copy Files to Xcode Phase");
            SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Files,
               (ISD_AssetFile file) => {
                   return file.XCodeRelativePath;
               },
               (ISD_AssetFile file) => {

                   EditorGUILayout.BeginHorizontal();
                   EditorGUILayout.LabelField("Asset: ");
                   file.Asset = EditorGUILayout.ObjectField(file.Asset, typeof(UnityEngine.Object), false);
                   EditorGUILayout.EndHorizontal();

                   EditorGUILayout.BeginHorizontal();
                   EditorGUILayout.LabelField("XCode Path:");
                   file.XCodePath = EditorGUILayout.TextField(file.XCodePath);
                   EditorGUILayout.EndHorizontal();
               },
               () => {
                   ISD_Settings.Instance.Files.Add(new ISD_AssetFile());
               }
            );
        }


        public static void ShellScripts() {
            SA_EditorGUILayout.Header("Shell Scripts Build Phase");
            SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.ShellScripts, 

               (ISD_ShellScript script) => {
                      return script.Name;
                }, 
                (ISD_ShellScript script) => {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name: ");
                    script.Name = EditorGUILayout.TextField(script.Name);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Shell: ");
                    script.Shell = EditorGUILayout.TextField(script.Shell);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Script");
                    script.Script = EditorGUILayout.TextField(script.Script);
                    EditorGUILayout.EndHorizontal();
                },
                () => {
                    ISD_Settings.Instance.ShellScripts.Add(new ISD_ShellScript());
                }
            );
        }
       




		public static void DrawPlistVariable(ISD_PlistKey var, object valuePointer, IList valueOrigin) {
			EditorGUILayout.BeginHorizontal();

			if(var.Name.Length > 0) {
				var.IsOpen = EditorGUILayout.Foldout(var.IsOpen, var.Name + "   (" + var.Type.ToString() +  ")");
			} else {
				var.IsOpen = EditorGUILayout.Foldout(var.IsOpen, var.Type.ToString());
			}



			bool ItemWasRemoved = SrotingButtons (valuePointer, valueOrigin);
			if(ItemWasRemoved) {
				ISD_Settings.Instance.RemoveVariable (var, valueOrigin);
				return;
			}
			EditorGUILayout.EndHorizontal();

			if(var.IsOpen) {						
				EditorGUI.indentLevel++; {

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Type");
					if (var.ChildrensIds.Count > 0) {
						GUI.enabled = false;
						var.Type = (ISD_PlistKeyType)EditorGUILayout.EnumPopup (var.Type);
						GUI.enabled = GUI_ENABLED;
					} else {
						var.Type = (ISD_PlistKeyType)EditorGUILayout.EnumPopup (var.Type);
					}
					EditorGUILayout.EndHorizontal();


					if (var.Type == ISD_PlistKeyType.Array) {
						DrawArrayValues (var);
					} else if (var.Type == ISD_PlistKeyType.Dictionary) {
						DrawDictionaryValues (var);
					} else if (var.Type == ISD_PlistKeyType.Boolean) {
                        var.BooleanValue = SA_EditorGUILayout.ToggleFiled("Value", var.BooleanValue, SA_StyledToggle.ToggleType.YesNo);


					} else {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Value");								
						switch(var.Type) {							
						case ISD_PlistKeyType.Integer:
							var.IntegerValue = EditorGUILayout.IntField (var.IntegerValue);
							break;									
						case ISD_PlistKeyType.String:
							var.StringValue = EditorGUILayout.TextField (var.StringValue);
							break;
						}
						EditorGUILayout.EndHorizontal();
					}

				} EditorGUI.indentLevel--;
			}

		}


		public static void DrawArrayValues (ISD_PlistKey var) {


			var.IsListOpen = EditorGUILayout.Foldout (var.IsListOpen, "Array Values (" + var.ChildrensIds.Count + ")");

			if (var.IsListOpen) {		

				EditorGUI.indentLevel++; {

					foreach	(string uniqueKey in var.ChildrensIds) {
						ISD_PlistKey v = ISD_Settings.Instance.getVariableById(uniqueKey);
						DrawPlistVariable (v, uniqueKey, var.ChildrensIds);

						if(!var.ChildrensIds.Contains(uniqueKey)) {
							return;
						}
					}


					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.Space ();
					if (GUILayout.Button ("Add Value", GUILayout.Width (100))) {
						ISD_PlistKey newVar = new ISD_PlistKey();

						var.AddChild (newVar);
					}
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Space ();

				} EditorGUI.indentLevel--;
			} 
		}

		public static void DrawDictionaryValues (ISD_PlistKey var) {
			var.IsListOpen = EditorGUILayout.Foldout (var.IsListOpen, "Dictionary Values");

			if (var.IsListOpen) {

				EditorGUI.indentLevel++; {

					foreach	(string uniqueKey in var.ChildrensIds) {
						ISD_PlistKey v = ISD_Settings.Instance.getVariableById(uniqueKey);
						DrawPlistVariable (v, uniqueKey, var.ChildrensIds);

						if(!var.ChildrensIds.Contains(uniqueKey)) {
							return;
						}
					}


					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.PrefixLabel ("New Key");
					NewValueName = EditorGUILayout.TextField (NewValueName);

					if (GUILayout.Button ("Add", GUILayout.Width (50))) {
						if (NewValueName.Length > 0) {
							ISD_PlistKey v = new ISD_PlistKey ();
							v.Name = NewValueName;
							var.AddChild (v);									
						}
					}

					EditorGUILayout.EndHorizontal ();
				} EditorGUI.indentLevel--;
			} 

		}

		




        private static int s_newLangindex = 0;
		public static void LanguageValues ()	{

            SA_EditorGUILayout.Header("Languages");

            SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Languages,
                (SA_ISOLanguage lang) => {
                    return lang.Code.ToUpper() +  "     (" + lang.Name + ")"; 
                }
            );

            EditorGUILayout.BeginHorizontal();
            s_newLangindex = EditorGUILayout.Popup(s_newLangindex, SA_LanguagesUtil.ISOLanguagesList.Names.ToArray());
            if (GUILayout.Button("Add Language", GUILayout.Height(20))) {
                var lang = SA_LanguagesUtil.ISOLanguagesList.Languages[s_newLangindex];
                ISD_Settings.Instance.Languages.Add(lang);
            }
            EditorGUILayout.EndHorizontal();
		}





		public static void AboutGUI() {
			GUI.enabled = true;
			EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
			EditorGUILayout.Space();


            SA_EditorGUILayout.SelectableLabel(SdkVersion,   ISD_Settings.VERSION_NUMBER);
            SA_CompanyGUILayout.SupportMail();
            SA_CompanyGUILayout.DrawLogo();

		}





        public static bool SrotingButtons(object currentObject, IList ObjectsList) {

            int ObjectIndex = ObjectsList.IndexOf(currentObject);
            if (ObjectIndex == 0) {
                GUI.enabled = false;
            }

            bool up = GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
            if (up) {
                object c = currentObject;
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
                object c = currentObject;
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
