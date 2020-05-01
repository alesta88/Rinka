using System;
using System.Collections;
#if DEVELOP_ENV
using System.Reflection;
#endif
using System.Text;
using UnityEngine;

#pragma warning disable 0168
#pragma warning disable 0219


/// <summary>
/// ダンプの表示オプション
/// </summary>
[Flags]
public enum DataDumpOption {
	DeepDump = (1<<0),
	PrivateParam = (1<<1),
	DeclaredOnly = (1<<2),
}

/// <summary>
/// 様々な型に対応したデータダンプユーティリティ
/// DEVELOP_ENVのプリプロセッサが有効なときにのみ動作
/// </summary>
public static class DataDumpUtil {

	private const string INDENT_SPACE = "    ";
	
	/// <summary>
	/// 受け渡し用のデータをコンソールログに出力
	/// </summary>
	public static void DebugLog( object data, DataDumpOption flag = 0 ) {
	#if DEVELOP_ENV
		Debug.Log( GetDumpStr( data, null, flag ) );
	#else
		return;
	#endif
	}

	/// <summary>
	/// 受け渡し用のデータをコンソールログに出力
	/// </summary>
	public static void DebugLog(object data, string name, DataDumpOption flag = 0) {
	#if DEVELOP_ENV
		Debug.Log( GetDumpStr( data, name, flag ) );
	#else
		return;
	#endif
	}

	/// <summary>
	/// Dumpしたデータの文字列を返す
	/// </summary>
	public static string GetDumpStr(object data, DataDumpOption flag = 0) {
		return GetDumpStr( data, null, flag );
	}

	/// <summary>
	/// Dumpしたデータの文字列を返す
	/// </summary>
	public static string GetDumpStr( object data, string name, DataDumpOption flag = 0 ) {
		#if DEVELOP_ENV
		StringBuilder sb = new StringBuilder();

		// null
		if( data == null ) {
			return string.Format( "GetDumpStr() >> data is null" );
		}

		// とりあえず型名を表示
		string dataName = name;
		if( string.IsNullOrEmpty(name) ) {
			dataName = data.GetType().Name;
		}

		sb.AppendLine( string.Format( "-- {0} -------------------------", dataName ) );

		if( data != null ) {
			AppendDataLog( sb, data, 0, flag );
		}
		else {
			sb.AppendLine( "null" );
		}
		return sb.ToString();
		#else
		return "";
		#endif
	}



	#if DEVELOP_ENV

	/// <summary>
	/// データの階層を掘り下げていくための再起関数
	/// </summary>
	/// <param name="sb"></param>
	/// <param name="data"></param>
	/// <param name="indent"></param>
	/// <returns></returns>
	private static StringBuilder AppendDataLog(StringBuilder sb, object data, int indent, DataDumpOption flag, string rootVariableName = "(root)") {
		if( indent > 4 ) return sb;
		string indentStr = "";
		for( int i = 0; i < indent; i++ )
			indentStr += INDENT_SPACE;

		//bool isPrivate = (flag & DataDumpOption.DeepDump) != 0;
		var type = data.GetType();
		//var members = GetMembers( type, isPrivate );

		if( data == null ) {
			sb.AppendLine( string.Format( "{0}{1} {2} : {3}", indentStr, data.GetType().Name, rootVariableName, "(null)" ) );
		}
		else if( data is string ) {
			sb.AppendLine( string.Format( "{0}{1} {2} : \"{3}\"", indentStr, data.GetType().Name, rootVariableName, data ) );
		}
		else if( data is DateTime || data is DateTime? ) {
			DateTime dt = (DateTime)data;
			sb.AppendLine( string.Format( "{0}{1} {2} : {3}", indentStr, data.GetType().Name, rootVariableName, dt.ToString("yyyy/MM/dd HH:mm:ss") ) );
		}
		else if( data is IDictionary ) {
			sb.AppendLine( string.Format( "{0}{1} {2} : ", indentStr, data.GetType().Name, rootVariableName ) );
			sb = AppendLog_IDictionary( sb, data as IDictionary, indent + 1, flag );
		}
		else if( data is IList ) {
			sb.AppendLine( string.Format( "{0}{1} {2} : ", indentStr, data.GetType().Name, rootVariableName ) );
			sb = AppendLog_IList( sb, data as IList, indent + 1, flag );
		}
		else if( type.IsClass == true ) {
			sb.AppendLine( string.Format( "{0}{1} {2} : ", indentStr, data.GetType().Name, rootVariableName ) );
			sb = AppendLog_ClassRefrection( sb, data, indent + 1, flag );
		}
		else {
			sb.AppendLine( string.Format( "{0}{1} {2} : {3}", indentStr, data.GetType().Name, rootVariableName, data ) );
		}

		return sb;
	}

	private static StringBuilder AppendLog_ClassRefrection( StringBuilder sb, object data, int indent, DataDumpOption flag ) {
		if( indent > 4 ) return sb;
		Type type = null;
		MemberInfo[] members2 = null;

		string indentStr = "";
		for( int i = 0; i < indent; i++ )
			indentStr += INDENT_SPACE;

		bool isDeepDump = (flag & DataDumpOption.DeepDump) != 0;
		bool isPrivate = (flag & DataDumpOption.PrivateParam) != 0;
		bool isDeclaredOnly = (flag & DataDumpOption.DeclaredOnly) != 0;

		//メンバを取得する
		Type t = data.GetType();
		MemberInfo[] members = GetMembers( t, isPrivate, isDeclaredOnly );

		foreach( MemberInfo m in members ) {
			
			switch( m.MemberType ) {
				case MemberTypes.Field:
					object fieldValue = null;
					FieldInfo fieldInfo = null;
					try {
						fieldInfo = t.GetField( m.Name );
						if( fieldInfo == null ) {
							fieldInfo = t.GetField( m.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic );
							if( fieldInfo == null ) continue;
						}
						fieldValue = fieldInfo.GetValue( data );
						type = fieldValue.GetType();
						members2 = GetMembers( type, isPrivate, isDeclaredOnly );
					}
					catch( Exception e ) {
						continue;
					}

					if( fieldValue == null ) {
						sb.AppendLine( string.Format( "{0}{1} {2} {3} : {4}", indentStr, GetAccessStr( fieldInfo ), type.Name, m.Name, "(null)" ) );
					}
					else if( fieldValue is string ) {
						sb.AppendLine( string.Format( "{0}{1} {2} {3} : \"{4}\"", indentStr, GetAccessStr(fieldInfo), type.Name, m.Name, fieldValue ) );
					}
					else if( fieldValue is DateTime ) {
						DateTime dt = (DateTime)data;
						sb.AppendLine( string.Format( "{0}{1} {2} {3} : {4}", indentStr, GetAccessStr( fieldInfo ), type.Name, m.Name, dt.ToString( "yyyy/MM/dd HH:mm:ss" ) ) );
					}
					else if( fieldValue is IDictionary ) {
						sb.AppendLine( string.Format( "{0}{1} {2} {3} : ", indentStr, GetAccessStr( fieldInfo ), type.Name, m.Name ) );
						sb = AppendLog_IDictionary( sb, fieldValue as IDictionary, indent + 1, flag );
					}
					else if( fieldValue is IList ) {
						sb.AppendLine( string.Format( "{0}{1} {2} {3} : ", indentStr, GetAccessStr( fieldInfo ), type.Name, m.Name ) );
						sb = AppendLog_IList( sb, fieldValue as IList, indent + 1, flag );
					}
					else if( isDeepDump && type.IsClass == true ) {
						sb.AppendLine( string.Format( "{0}{1} {2} {3} : ", indentStr, GetAccessStr( fieldInfo ), type.Name, m.Name ) );
						sb = AppendLog_ClassRefrection( sb, fieldValue, indent + 1, flag );
					}
					else {
						sb.AppendLine( string.Format( "{0}{1} {2} {3} : {4}", indentStr, GetAccessStr( fieldInfo ), type.Name, m.Name, fieldValue ) );
					}
					break;

				case MemberTypes.Property:
					object propertyValue = null;
					try {
						propertyValue = t.GetProperty( m.Name ).GetValue( data, null );
						type = propertyValue.GetType();
						members2 = GetMembers( type, isPrivate, isDeclaredOnly );
					}
					catch( Exception e ) {
						continue;
					}

					if( propertyValue == null ) {
						sb.AppendLine( string.Format( "{0}{1} {2} : {3}", indentStr, type.Name, m.Name, "(null)" ) );
					}
					else if( propertyValue is string ) {
						sb.AppendLine( string.Format( "{0}{1} {2} : \"{3}\"", indentStr, type.Name, m.Name, propertyValue ) );
					}
					else if( propertyValue is DateTime ) {
						DateTime dt = (DateTime)propertyValue;
						sb.AppendLine( string.Format( "{0}{1} {2} : {3}", indentStr, type.Name, m.Name, dt.ToString( "yyyy/MM/dd HH:mm:ss" ) ) );
					}
					else if( propertyValue is IDictionary ) {
						sb.AppendLine( string.Format( "{0}{1} {2} : ", indentStr, type.Name, m.Name ) );
						sb = AppendLog_IDictionary( sb, propertyValue as IDictionary, indent + 1, flag );
					}
					else if( propertyValue is IList ) {
						sb.AppendLine( string.Format( "{0}{1} {2} : ", indentStr, type.Name, m.Name ) );
						sb = AppendLog_IList( sb, propertyValue as IList, indent + 1, flag );
					}
					else if( isDeepDump && type.IsClass == true ) {
						sb.AppendLine( string.Format( "{0}{1} {2} : ", indentStr, type.Name, m.Name ) );
						sb = AppendLog_ClassRefrection( sb, propertyValue, indent + 1, flag );
					}
					else {
						sb.AppendLine( string.Format( "{0}{1} {2} : {3}", indentStr, type.Name, m.Name, propertyValue ) );
					}
					break;
			}
		}
		return sb;
	}

	/// <summary>
	/// データの階層を掘り下げていくための再起関数(IList型)
	/// </summary>
	/// <param name="sb"></param>
	/// <param name="enumerableData"></param>
	/// <param name="indent"></param>
	/// <returns></returns>
	private static StringBuilder AppendLog_IList(StringBuilder sb, IList enumerableData, int indent, DataDumpOption flag ) {
		if( indent > 4 ) return sb;
		string indentStr = "";
		for( int i = 0; i < indent; i++ )
			indentStr += INDENT_SPACE;

		bool isDeepDump = (flag & DataDumpOption.DeepDump) != 0;
		//bool isPrivate = (flag & DataDumpOption.PrivateParam) != 0;
		int index = 0;
		foreach( var data in enumerableData ) {

			var type = data.GetType();
			//var members = GetMembers( type,isPrivate );

			if( data == null ) {
				sb.AppendLine( string.Format( "{0}{1} list[{2}] : {3}", indentStr, type.Name, index, "(null)" ) );
			}
			else if( data is string ) {
				sb.AppendLine( string.Format( "{0}{1} list[{2}] : \"{3}\"", indentStr, type.Name, index, data ) );
			}
			else if( data is DateTime ) {
				DateTime dt = (DateTime)data;
				sb.AppendLine( string.Format( "{0}{1} list[{2}] : {3}", indentStr, type.Name, index, dt.ToString( "yyyy/MM/dd HH:mm:ss" ) ) );
			}
			else if( data is IDictionary ) {
				sb.AppendLine( string.Format( "{0}{1} list[{2}] : ", indentStr, type.Name, index ) );
				sb = AppendLog_IDictionary( sb, data as IDictionary, indent + 1, flag );
			}
			else if( data is IList ) {
				sb.AppendLine( string.Format( "{0}{1} list[{2}] : ", indentStr, type.Name, index ) );
				sb = AppendLog_IList( sb, data as IList, indent + 1, flag );
			}
			else if( isDeepDump && type.IsClass == true ) {
				sb.AppendLine( string.Format( "{0}{1} list[{2}] : ", indentStr, type.Name, index ) );
				sb = AppendLog_ClassRefrection( sb, data, indent + 1, flag );
			}
			/*else if( deepDump && members != null && members.Length > 0 ) {
				sb.AppendLine( string.Format( "{0}{1} list[{2}] : ", indentStr, type.Name, index ) );
				sb = AppendLog_ClassRefrection( sb, data, indent + 1 );
			}*/
			else {
				sb.AppendLine( string.Format( "{0}{1} list[{2}] : {3}", indentStr, type.Name, index, data ) );
			}
			index++;
		}
		return sb;
	}

	/// <summary>
	/// データの階層を掘り下げていくための再起関数(IDictionary型)
	/// </summary>
	/// <param name="sb"></param>
	/// <param name="dict"></param>
	/// <param name="indent"></param>
	/// <returns></returns>
	private static StringBuilder AppendLog_IDictionary(StringBuilder sb, IDictionary dict, int indent, DataDumpOption flag) {
		if( indent > 4 ) return sb;
		string indentStr = "";
		for( int i = 0; i < indent; i++ )
			indentStr += INDENT_SPACE;

		bool isDeepDump = (flag & DataDumpOption.DeepDump) != 0;
		//bool isPrivate = (flag & DataDumpOption.PrivateParam) != 0;
		int index = 0;
		foreach( var key in dict.Keys ) {
			var data = dict[key];
			var type = data.GetType();
			//var members = GetMembers( type, isPrivate );

			if( data == null ) {
				sb.AppendLine( string.Format( "{0}{1} \"{2}\" : {3}", indentStr, type.Name, key, "(null)" ) );
			}
			else if( data is string ) {
				sb.AppendLine( string.Format( "{0}{1} \"{2}\" : \"{3}\"", indentStr, type.Name, key, data ) );
			}
			else if( data is DateTime ) {
				DateTime dt = (DateTime)data;
				sb.AppendLine( string.Format( "{0}{1} \"{2}\" : {3}", indentStr, type.Name, key, dt.ToString( "yyyy/MM/dd HH:mm:ss" ) ) );
			}
			else if( data is IDictionary ) {
				sb.AppendLine( string.Format( "{0}{1} \"{2}\" : ", indentStr, type.Name, key ) );
				sb = AppendLog_IDictionary( sb, data as IDictionary, indent + 1, flag );
			}
			else if( data is IList ) {
				sb.AppendLine( string.Format( "{0}{1} \"{2}\" : ", indentStr, type.Name, key ) );
				sb = AppendLog_IList( sb, data as IList, indent + 1, flag );
			}
			else if( isDeepDump && type.IsClass == true ) {
				sb.AppendLine( string.Format( "{0}{1} \"{2}\" : ", indentStr, type.Name, key ) );
				sb = AppendLog_ClassRefrection( sb, data, indent + 1, flag );
			}
			/*else if( deepDump && members != null && members.Length > 0 ) {
				sb.AppendLine( string.Format( "{0}{1} \"{2}\" : ", indentStr, type.Name, key ) );
				sb = AppendLog_ClassRefrection( sb, data, indent + 1 );
			}*/
			else {
				sb.AppendLine( string.Format( "{0}{1} \"{2}\" : {3}", indentStr, type.Name, key, data ) );
			}
			index++;
		}
		return sb;
	}


	private static MemberInfo[] GetMembers(Type type, bool isPrivate, bool isDeclaredOnly = false) {
		BindingFlags flags = BindingFlags.Public
				| BindingFlags.Instance
				//| BindingFlags.Static
				;
		if( isPrivate ) {
			flags |= BindingFlags.NonPublic;
		}
		if( isDeclaredOnly ) {
			flags |= BindingFlags.DeclaredOnly;
		}
		return type.GetMembers( flags );
	}
	
	/// <summary>
	/// アクセスタイプを文字列で返す
	/// </summary>
	/// <param name="fi"></param>
	/// <returns></returns>
	private static string GetAccessStr(FieldInfo fi) {
		if( fi.IsPrivate )
			return "private";
		else if( fi.IsPublic )
			return "public ";

		return "protected";
	}
	#endif // end #if DEVELOP_ENV
}
