using System.IO;
using Oryx.SaveLoad.Utilities;

namespace Oryx.SaveLoad
{
    public class SaveLoadSystem
    {
        private string _directory;

        public SaveLoadSystem()
        {
            _directory = FileUtility.SaveDataDirectory;
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }

        public void DeleteSavedData()
        {
            string targetDir = _directory;
            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            Directory.Delete(targetDir, false);
        }
        
        public void Init()
        {
            _directory = FileUtility.SaveDataDirectory;
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }
        
        public T GetData<T>()
        {
            T data = FileUtility.ReadObjectSmart<T>(_directory);
            return data;
        }

        public void SaveData<T>(T data)
        {
            FileUtility.WriteObjectSmart(FileUtility.SaveDataDirectory, data);
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Delete Saved Data")]
#endif
        public static void Delete()
        {
            var quickSaveSystem = new SaveLoadSystem();
            quickSaveSystem.DeleteSavedData();
        }
    }
}
