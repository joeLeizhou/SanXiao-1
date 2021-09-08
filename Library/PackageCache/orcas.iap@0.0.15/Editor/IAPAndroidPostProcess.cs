#if UNITY_ANDROID
using System;
using System.IO;
using System.Xml;
using UnityEditor.Android;

public class IAPAndroidPostProcess : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 0; } }
    private const string kAndroidNamespaceURI = "http://schemas.android.com/apk/res/android";
    private const string kBillingPermission = "com.android.vending.BILLING";
    public void OnPostGenerateGradleAndroidProject(string projectPath)
    {
        var manifestPath = string.Format("{0}/src/main/AndroidManifest.xml", projectPath);
        if (!File.Exists(manifestPath))
            throw new FileNotFoundException(string.Format("'{0}' doesn't exist.", manifestPath));
        
        XmlDocument manifestDoc = new XmlDocument();
        manifestDoc.Load(manifestPath);
        AppendAndroidPermissionField(manifestPath, manifestDoc,kBillingPermission);
        manifestDoc.Save(manifestPath);
    }
    
    private void AppendAndroidPermissionField(string manifestPath, XmlDocument xmlDoc, string name)
    {
        var manifestNode = xmlDoc.SelectSingleNode("manifest");
        if (manifestNode == null)
            throw new ArgumentException(string.Format("Missing 'manifest' node in '{0}'.", manifestPath));

        foreach (XmlNode node in manifestNode.ChildNodes)
        {
            if (!(node is XmlElement) || node.Name != "uses-permission")
                continue;

            var elementName = ((XmlElement)node).GetAttribute("name", kAndroidNamespaceURI);
            if (elementName == name)
                return;
        }

        XmlElement metaDataNode = xmlDoc.CreateElement("uses-permission");
        metaDataNode.SetAttribute("name", kAndroidNamespaceURI, name);
        manifestNode.AppendChild(metaDataNode);
    }
}
#endif