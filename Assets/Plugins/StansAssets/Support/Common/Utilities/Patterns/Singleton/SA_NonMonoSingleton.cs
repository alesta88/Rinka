////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

namespace SA.Common.Pattern {

	public abstract class NonMonoSingleton<T>  where T : NonMonoSingleton<T>, new() {

        private static T s_instance = null;
		
		public static T Instance {
			get {
				if (s_instance == null) {
					s_instance = new T();
				}

				return s_instance;
			}

		}
	}
}
