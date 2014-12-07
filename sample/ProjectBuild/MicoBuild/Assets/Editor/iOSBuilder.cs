using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


// ref: https://github.com/animetrics/PlistCS
using PlistCS;

// ref: https://github.com/gforeman/XcodeProjectModifier
using UnityEditor.XCodeEditor;



public static class PlistValue {
    public static string APP_ICON_76 = "Icon-76.png";
    public static string APP_ICON_120 = "Icon-60@2x.png";
    public static string APP_ICON_152 = "Icon-76@2x.png";
    public static string URL_TYPE_ID = "com.gisoek.ios.enterprise";
    public static string URL_TYPE_ROLE_NONE = "None";
}



public class iOSBuilder
{
    static readonly string XCODEPRJ_FPATH = Path.GetFullPath(Path.Combine(Application.dataPath, "../../BuildResult"));

    // ref: /Users/eunpyoungkim/Library/MobileDevice/Provisioning Profiles
    const string PROVISIONING_UUID = "978518f8-7431-4f54-b889-34bdf8312408";
    const string CODE_SIGN_NAME = "iPhone Distribution: APPLE, Inc.";

    static void PerformiOSBuild()
    {
        var scenes = EditorBuildSettings.scenes;
        List<string> sceneList = new List<string>();

        foreach (var scene in scenes)
        {
            if (scene.enabled)
                sceneList.Add(scene.path);
        }

        // 씬리스트 스트링 배열화.
        var sceneArray = sceneList.ToArray();


        // XCODEPRJ를 생성하고,
        BuildPipeline.BuildPlayer(sceneArray, XCODEPRJ_FPATH, BuildTarget.iPhone, BuildOptions.None);

        // XCODEPRJ에 있는, project.pbxproj를 수정해준다.
        EdittingPbxProj(XCODEPRJ_FPATH);

        // XCODEPRJ에 있는, Info.plist를 수정해준다.
        EdittingInfoPlist(Path.Combine(XCODEPRJ_FPATH, "Info.plist"));
    }


    public static void EdittingPbxProj(string pathToBuiltProject)
    {
        // Xcode 프로젝트 불러오기.(project.pbxproj 수정 용도.)
        XCProject prj = new XCProject(pathToBuiltProject);

        // .projmods적용. 외부 프래임워크 추가, 파일 복사(폴더).
        string[] files = Directory.GetFiles(Application.dataPath + "/Editor/XcodeProjectMods", "*.projmods", SearchOption.AllDirectories);
        foreach( string file in files )
            prj.ApplyMod( file );

        // 테스트로 아무 프로비저닝이나 추가함.
        prj.overwriteBuildSetting("CODE_SIGN_NAME_IDENTITY[sdk=iphoneos*]", CODE_SIGN_NAME, "Release");
        prj.overwriteBuildSetting("CODE_SIGN_NAME_IDENTITY[sdk=iphoneos*]", CODE_SIGN_NAME, "Debug");
        prj.overwriteBuildSetting("CODE_SIGN_NAME_IDENTITY", CODE_SIGN_NAME, "Release");
        prj.overwriteBuildSetting("CODE_SIGN_NAME_IDENTITY", CODE_SIGN_NAME, "Debug");
        prj.overwriteBuildSetting("PROVISIONING_PROFILE", PROVISIONING_UUID, "Release");
        prj.overwriteBuildSetting("PROVISIONING_PROFILE", PROVISIONING_UUID, "Debug");
    
        // 설정이 변경된 프로젝트 저장.
        prj.Save();
    }

    static void EdittingInfoPlist(string plistPath)
    {
        // plist 읽기.
        var plst = (Dictionary<string, object>)Plist.readPlist(plistPath);

        // App Icon 설정.
        var CFBundleIconFiles = (List<object>)plst["CFBundleIconFiles"];
        CFBundleIconFiles.Add(PlistValue.APP_ICON_76);
        CFBundleIconFiles.Add(PlistValue.APP_ICON_120);
        CFBundleIconFiles.Add(PlistValue.APP_ICON_152);

        // URL scheme 설정.
        plst["CFBundleURLTypes"] = new List<object>{
            new Dictionary<string, object> {
                {"CFBundleTypeRole", PlistValue.URL_TYPE_ROLE_NONE},
                {"CFBundleURLName", PlistValue.URL_TYPE_ID},
                {"CFBundleURLSchemes", new List<object>{PlistValue.URL_TYPE_ID}},
            }
        };

        // plist 덮어 씌우기
        Plist.writeXml(plst, plistPath);
    }
}
