using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace com.ktgame.save.core.editor
{
    public class BuildPreProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;
        
        private const string SettingName = "com.ktgame.save.core";
        private const string PackageName = "com.ktgame.save.core";
        
        public void OnPreprocessBuild(BuildReport report)
        {
            var pluginPath = Path.Combine(Application.dataPath, $"Plugins/Ktgame/Settings/{SettingName}");
            if (!Directory.Exists(pluginPath))
            {
                Directory.CreateDirectory(pluginPath);
            }

            if (AssetDatabase.IsValidFolder($"Packages/{PackageName}"))
            {
                AssetDatabase.CopyAsset($"Packages/{PackageName}/Runtime/link.xml", $"Assets/Plugins/Ktgame/Settings/{SettingName}/link.xml");
            }
        }
    }
}
