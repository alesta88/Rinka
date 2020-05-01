using UnityEngine;
using UnityEditor;

class StageChunkImporter : AssetPostprocessor {
    const string STAGE_SPRITE_ASSET_PATH = "Assets/Sprites/Stages";
    const string STAGE_DATA_ASSET_PATH = "Assets/Data/StageData/";
    const string STAGE_ASSET_PREFIX = "stage";
    const string STAGE_ASSET_SUFFIX = "walls";
    const string SPAWN_ITEM_ASSET_SUFFIX = "itemspawn";
    const string TEXTURE_EXT = ".png";
    const string IGNORE_EXT = ".meta";

    static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths ) {
        // インポートされたファイルを見て、ステージの画像データファイルの場合に処理する
        foreach(string assetName in importedAssets) {
            if(assetName.Contains( STAGE_SPRITE_ASSET_PATH ) && assetName.Contains( STAGE_ASSET_PREFIX )) {
                StageChunkData chunk = null;
                Sprite itemSpawnAreaSprite = null;

                string[] fileEntries = System.IO.Directory.GetFiles( System.IO.Path.GetDirectoryName( assetName ) );
                foreach(string filename in fileEntries) {
                    if( filename.Contains( IGNORE_EXT ) || !filename.Contains( TEXTURE_EXT ) )
                        continue;

                    var importer = AssetImporter.GetAtPath( assetName ) as TextureImporter;
                    if( filename.Contains( STAGE_ASSET_SUFFIX ) ) {
                        if( !importer.isReadable ) {
                            Debug.LogWarningFormat( importer, "wallsの画像のRead/Write Enabledをチェックして、もう一回インポートしてください" );
                            return;
                        } else {
                            chunk = ImportStageChunk( importer, filename, assetName );
                            if( itemSpawnAreaSprite != null) {
                                chunk.ItemSpawnAreaSprite = itemSpawnAreaSprite;
                                return;
                            }
                        }
                        
                    } else if( filename.Contains( SPAWN_ITEM_ASSET_SUFFIX ) ) {
                        itemSpawnAreaSprite = AssetDatabase.LoadAssetAtPath<Sprite>( filename );
                        if( chunk != null) {
                            chunk.ItemSpawnAreaSprite = itemSpawnAreaSprite;
                            return;
                        }
                    }
                }
                return;
            }
        }
    }

    // 画像ファイルをインポートしてデータを生成
    static StageChunkData ImportStageChunk( TextureImporter importer, string filename, string assetName ) {
        var data = ScriptableObject.CreateInstance<StageChunkData>();
        var tex = AssetDatabase.LoadAssetAtPath( filename, typeof( Texture2D ) ) as Texture2D;
        for(int y = 0; y < tex.height; y++) {
            float pixelAlpha = tex.GetPixel( 0, y ).a;
            if( pixelAlpha < 0.5f) {
                data.LowerEntryPosition = new Vector2( -tex.width / 2f, y );
            }
        }
        for(int y = 0; y < tex.height; y++) {
            float pixelAlpha = tex.GetPixel( tex.width-1, y ).a;
            if(pixelAlpha < 0.5f) {

                data.LowerExitPosition = new Vector2( tex.width / 2f, y );

                importer.isReadable = false;
                SaveAsset( tex, assetName, data );
                return data;
            }
        }


        Debug.Log( "Invalid Stage" );
        return null;
    }

    static void SaveAsset( Texture2D tex, string assetName, StageChunkData data ) {
        string[] splitPath = assetName.Split( new char[] { '/', '.' } );
        var file = splitPath[splitPath.Length - 2];
        AssetDatabase.CreateAsset( data, STAGE_DATA_ASSET_PATH + file + ".asset" );
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.LogFormat( tex, file + "が正常にインポートされました" );
    }
}
