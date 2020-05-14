using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Foundation.Utility
{

    public static class SA_IconManager   {

		private static Dictionary<string, Texture2D> s_icons = new Dictionary<string, Texture2D>();
        private static Dictionary<float, Texture2D> s_colorIcons = new Dictionary<float, Texture2D>();


        /// <summary>
        /// Generates plane color 1*1 <see cref="Texture2D"/> object from a give html color string.
        /// </summary>
        /// <param name="htmlString"> Case insensitive html string to be converted into a color.</param> 
		public static Texture2D GetIconFromHtmlColorString(string htmlString) {
			return GetIconFromHtmlColorString(htmlString, Color.black);
        }


        /// <summary>
        /// Generates plane color 1*1 <see cref="Texture2D"/> object from a give html color string.
        /// </summary>
        /// <param name="htmlString"> Case insensitive html string to be converted into a color.</param> 
        /// <param name="fallback"> Fall back  <see cref="Color"/> in case of unsuccessful color convertation.</param> 
        public static Texture2D GetIconFromHtmlColorString (string htmlString, Color fallback ) {
			Color color = fallback;
			ColorUtility.TryParseHtmlString (htmlString, out color);
			return GetIcon (color);
		}



        /// <summary>
        /// Generates plane color <see cref="Texture2D"/> object of a given size
        /// </summary>
        /// <param name="color"> Texture color. </param> 
        /// <param name="width"> Texture width. </param> 
        /// <param name="width"> Texture height. </param> 
        public static Texture2D GetIcon(Color color, int width = 1, int height = 1) {
            float colorId = color.r * 100000f + color.g * 10000f + color.b * 1000f + color.a * 100f + width * 10f + height;

            if (s_colorIcons.ContainsKey(colorId) && s_colorIcons[colorId] != null) {
                return s_colorIcons[colorId];
            } else {


                Texture2D tex = new Texture2D(width, height);
                for (int w = 0; w < width; w++) {
                    for (int h = 0; h < height; h++) {
                        tex.SetPixel(w, h, color);
                    }
                }
                
                tex.Apply();
                

                s_colorIcons[colorId] = tex;
                return GetIcon(color, width, height);
            }
        }



        /// <summary>
        /// Loads a <see cref="Texture2D"/> object from the spesified Resources folder relative path.
        /// Object also will be cached in memory.
        /// </summary>
        /// <param name="path"> Resources folder relative path. </param> 
		public static Texture2D GetIconAtPath(string path) {

            if(s_icons.ContainsKey(path)) {
                return s_icons[path];
            } else {
                Texture2D tex = Resources.Load(path) as Texture2D;
                if(tex == null) {
                    tex = new Texture2D(1, 1);
                }

                s_icons.Add(path, tex);
				return GetIconAtPath(path);
            }
        }

        /// <summary>
        /// Rotates <see cref="Texture2D"/> pixels to a spesified angle
        /// </summary>
        /// <param name="tex"> Source texture to rotate. </param> 
        /// <param name="angle"> Rotate angle </param> 
        public static Texture2D Rotate(Texture2D tex, float angle) {
            Texture2D rotImage = new Texture2D(tex.width, tex.height);
            int x, y;
            float x1, y1, x2, y2;

            int w = tex.width;
            int h = tex.height;
            float x0 = rot_x(angle, -w / 2.0f, -h / 2.0f) + w / 2.0f;
            float y0 = rot_y(angle, -w / 2.0f, -h / 2.0f) + h / 2.0f;

            float dx_x = rot_x(angle, 1.0f, 0.0f);
            float dx_y = rot_y(angle, 1.0f, 0.0f);
            float dy_x = rot_x(angle, 0.0f, 1.0f);
            float dy_y = rot_y(angle, 0.0f, 1.0f);



            x1 = x0;
            y1 = y0;

            for (x = 0; x < tex.width; x++) {
                x2 = x1;
                y2 = y1;
                for (y = 0; y < tex.height; y++) {
                    //rotImage.SetPixel (x1, y1, Color.clear);          

                    x2 += dx_x;//rot_x(angle, x1, y1);
                    y2 += dx_y;//rot_y(angle, x1, y1);
                    rotImage.SetPixel((int)Mathf.Floor(x), (int)Mathf.Floor(y), getPixel(tex, x2, y2));
                }

                x1 += dy_x;
                y1 += dy_y;

            }

            rotImage.Apply();
            return rotImage;
        }


        /// <summary>
        /// Attempts to convert a html color string.
        /// </summary>
        /// <param name="htmlString">Case insensitive html string to be converted into a color.</param>
        public static Color GetColorFromHtml(string htmlString) {
            return GetColorFromHtml(htmlString, Color.black);
        }


        /// <summary>
        /// Attempts to convert a html color string.
        /// </summary>
        /// <param name="htmlString">Case insensitive html string to be converted into a color.</param>
        /// <param name="fallback">The fallback color in case convertation error.</param>
        public static Color GetColorFromHtml(string htmlString, Color fallback) {
            Color color = fallback;
            ColorUtility.TryParseHtmlString(htmlString, out color);
            return color;
        }


        private static Color getPixel(Texture2D tex, float x, float y) {
            Color pix;
            int x1 = (int)Mathf.Floor(x);
            int y1 = (int)Mathf.Floor(y);

            if (x1 > tex.width || x1 < 0 ||
               y1 > tex.height || y1 < 0) {
                pix = Color.clear;
            } else {
                pix = tex.GetPixel(x1, y1);
            }

            return pix;
        }

        private static float rot_x(float angle, float x, float y) {
            float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
            float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
            return (x * cos + y * (-sin));
        }
        private static float rot_y(float angle, float x, float y) {
            float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
            float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
            return (x * sin + y * cos);
        }




    }
}
