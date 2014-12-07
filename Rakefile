# -*- coding: utf-8 -*-

# Unity3D ios빌드 자동화.

require 'xcodeproj'


UNITY = '/Applications/Unity/Unity.app/Contents/MacOS/Unity'
PROVISIONING_NAME = "LSOJP_Enterprise2"


PROJECT_DIR = "#{ENV['HOME']}/prj/toolbox/sample/ProjectBuild/MicoBuild" # ***
XCODEPRJ_FPATH = "#{ENV['HOME']}/prj/toolbox/sample/ProjectBuild/BuildResult/Unity-iPhone.xcodeproj" # ***
ARCHIVE_FPATH = "#{ENV['HOME']}/Library/Developer/Xcode/Archives/helloworld/Unity-iPhone-helloworld.xcarchive" # ***
IPA_FPATH = "#{ENV['HOME']}/temp/test.ipa" # ***


BUILD_METHOD = 'iOSBuilder.PerformiOSBuild'



task :default => [:clean, :build]
task :build do
    # 기존에 Xcode가 떠 있다면, 킬시켜주고,
    sh "(kill $(ps aux | grep '[X]code' | awk '{print $2}')) | true"

    # Unity를 활용하여 XcodeProject를 생성한다.
    sh "#{UNITY} -batchmode -projectPath #{PROJECT_DIR} -executeMethod #{BUILD_METHOD} -quit"

    # 사용자 스킴 재생성.
    # ref: http://stackoverflow.com/a/20941812
    # 혹시 문제시 다음 방법 시도: http://stackoverflow.com/questions/5304031/where-does-xcode-4-store-scheme-data
    prj = Xcodeproj::Project.open("#{XCODEPRJ_FPATH}")
    prj.recreate_user_schemes()
    prj.save()

    # xcarchive를 생성한다.
    sh "xcodebuild -project #{XCODEPRJ_FPATH} -scheme Unity-iPhone archive -archivePath #{ARCHIVE_FPATH}"

    # ipa를 뽑아낸다.
    sh "xcodebuild -exportArchive -archivePath #{ARCHIVE_FPATH} -exportPath #{IPA_FPATH} -exportFormat ipa -exportProvisioningProfile #{PROVISIONING_NAME}"
end


task :clean do
    sh "rm -rf #{IPA_FPATH}"
    sh "rm -rf #{ENV['HOME']}/toolbox/sample/ProjectBuild/BuildResult" # ***
end
