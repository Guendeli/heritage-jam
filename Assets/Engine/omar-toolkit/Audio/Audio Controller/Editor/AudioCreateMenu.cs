#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Oryx
{
    public class AudioCreateMenu : EditorWindow
    {
        [MenuItem( "Assets/Create/Oryx Audio/Create AudioController" ) ]
        static void CreateAudioController()
        {
            var ac = new GameObject( "AudioController" );
            ac.AddComponent<AudioController>();
            CreatePrefab( ac );
        }

        

        [MenuItem( "Assets/Create/Oryx Audio/Create AudioObject prefab" )]
        static void CreateAudioObjectPrefab()
        {
            var ao = new GameObject( "AudioObject" );
            ao.AddComponent<AudioObject>();
            ao.AddComponent<PoolableObject>();

            CreatePrefab( ao );
        }

        public static void CreatePrefab( GameObject gameObject )
        {
            var filePath = GetNoneExistingFilePath( GetSelectedPathOrFallback(), gameObject.name, "prefab" );
            PrefabUtility.CreatePrefab( filePath, gameObject );
            GameObject.DestroyImmediate( gameObject );
        }

        public static string GetNoneExistingFilePath( string path, string fileNameWithoutExtension, string fileExtension )
        {
            for( int index = 0; ; index++ )
            {
                string fn;
                if( index == 0 )
                {
                    fn = fileNameWithoutExtension;
                }
                else
                    fn = fileNameWithoutExtension + " (" + index + ")";

                var pathFinal = path + "/" + fn + "." + fileExtension;
                if( !File.Exists( pathFinal ) ) return pathFinal;
            }
        }

        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach( UnityEngine.Object obj in Selection.GetFiltered( typeof( UnityEngine.Object ), SelectionMode.Assets ) )
            {
                path = AssetDatabase.GetAssetPath( obj );
                if( !string.IsNullOrEmpty( path ) && File.Exists( path ) )
                {
                    path = Path.GetDirectoryName( path );
                    break;
                }
            }
            return path;
        }
    }
}
#endif