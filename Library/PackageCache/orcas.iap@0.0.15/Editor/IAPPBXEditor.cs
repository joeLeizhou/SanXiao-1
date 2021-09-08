using System.IO;
using UnityEngine;

#if UNITY_IOS


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;

#endif

namespace Orcas.Iap.Editor{
    public class IAPPBXEditor
    {
        //该属性是在build完成后，被调用的callback
        [PostProcessBuildAttribute(0)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            // BuildTarget需为iOS
            if (buildTarget != BuildTarget.iOS)
                return;

            // 初始化
            var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();

            pbxProject.ReadFromFile(projectPath);
            string targetGuid = pbxProject.GetUnityFrameworkTargetGuid(); // pbxProject.TargetGuidByName("Unity-iPhone");
            // 添加SystemCapabilities,
    #if UNITY_2017_2_OR_NEWER
            pbxProject.AddCapability(targetGuid, PBXCapabilityType.InAppPurchase);// available for unity 2017.1.0+            
    #endif

            var projectPath2 = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            var project = new PBXProject();
            project.ReadFromString(System.IO.File.ReadAllText(projectPath2));
#if UNITY_2019_3_OR_NEWER
            var manager = new ProjectCapabilityManager(projectPath2, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());            
            manager.AddInAppPurchase();
            manager.WriteToFile();
#else
            var manager = new ProjectCapabilityManager(projectPath2, "Entitlements.entitlements", PBXProject.GetUnityTargetName());            
            manager.AddInAppPurchase();
            manager.WriteToFile();
#endif
        }
    }
}
#endif