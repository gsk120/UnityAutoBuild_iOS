# 빌드 자동화 설명 문서

###빌드 자동화 Step
1. 빌드 Step
  * Unity Editor Menu에서 @Build > AutoBuild > iOSBuild를 눌러 빌드한다.
  * 유니티에서 제공하는 BuildPipeline 클래스를 이용하여 Xcode 프로젝트를 만드는 기본적인 빌드를 수행.
2. Xcode Setting Step
  * Unity Editor Menu에서 @Build > AutoBuild > XcodeSetting을 눌러 빌드한다.
  * XCodePostProcess.cs 코드에서 [PostProcessBuild]문구가 주석처리 되있지 않다면 빌드 후 자동으로 Xcode 세팅을 진행한다.
  * Assets 폴더 밑으로 존재하는 .projmods 파일들을 파싱하여 Xcode 세팅 진행.
  * 빌드로 생성된 Xcode프로젝트 경로로 XCProject 클래스 객체를 만들고, .projmods 파일로부터 읽은 정보를 XCProject의 ApplyMod함수를 이용하여 설정값 세팅 후 Save함수를 통해 실제 Xcode프로젝트에 적용.

###.projmods 작성 항목
  * group: Xcode 프로젝트 루트에 해당 이름의 그룹 폴더 생성 후 아래서 지정하는 라이브러리, 프레임워크, 파일 등을 저장
  * libs: 추가할 *.a와 같은 라이브러리 파일의 경로를 입력
  * frameworks: 추가할 framework의 리스트를 입력 => 이름만 입력할 경우 iOS SDK로 부터 해당 프레임워크를 찾음
  * headerpaths: 추가할 헤더의 경로를 입력
  * files: 추가할 파일의 리스트를 입력
  * folders: 입력한 경로에 있는 폴더를 추가 => 재귀적으로 하위 폴더 및 파일 모두 추가함
  * excludes: 파일, 폴더 등을 추가할 때 추가하지 않아도 되는 파일의 확장자명을 적는 부분

#####example)

```javascript
    {
        "group": "Services",
        "libs": ["libsqlite3.dylib"],
        "frameworks": ["MessageUI.framework","StoreKit.framework","iAd.framework",
                        "CoreData.framework","SystemConfiguration.framework","Social.framework:",
                        "Security.framework:","Accounts.framework:","AdSupport.framework:"],
        "headerpaths": ["Editor/iOS/GameCenter/**"],
        "files": ["/Users/beannt/Documents/FacebookSDK/FacebookSDK.framework",
                  "Editor/iOS/GameCenter/GameCenterBinding.m",
                  "Editor/iOS/GameCenter/GameCenterController.h",
                  "Editor/iOS/GameCenter/GameCenterController.mm",
                  "Editor/iOS/GameCenter/GameCenterManager.h",
                  "Editor/iOS/GameCenter/GameCenterManager.m"],
        "folders": ["iOS/Store/","iOS/GoogleAnalytics/"],
        "excludes": ["^.*.DS_Store$","^.*.meta$", "^.*.mdown^", "^.*.pdf$", "^.*.svn$"]
    }
```

###.projmods 파일 목록
1. Common.projmods
  * 국가에 상관없이 공통적으로 들어가는 세팅을 설정하는 파일
  * StoreKit.framework를 알맞은 위치에 추가한다.
2. Framework.projmods
  * 해당 국가에 필요한 프레임 워크를 세팅해 주는 파일
3. Appicon.projmods
  * App의 아이콘이미지 파일(76, 120, 152)을 세팅해 주는 파일
  * 이 파일은 저장 기능만 하기에, XCodePostProcess.cs에서 Info.plist를 수정해 주는 코드를 통해 자동 임포트 되도록 해준다.
4. Classes.projmos
  * Classes폴더 내부에 폴더 및 소스코드를 추가해 주는 기능

###Info.plist 변경
1. Appicon 항목 추가
  * plist파일을 파싱하여 아이콘 파일 리스트 항목 추가
  * key값은 CFBundleIconFile로 추가
  * 해상도별 아이콘 파일 이름
  > - 57x57 = Icon.png
    - 114x114 = Icon@2x.png
    - 72x72 = Icon-72.png
    - 144x144 = Icon-144.png
    - 120x120 = Icon-60@2x.png
    - 76x76 = Icon-76.png
    - 152x152 = Icon-76@2x.png

###Provisioning 선택
  * 빌드시 선택한 빌드 용도에 맞게 프로비저닝과 코드 사이닝을 설정해 준다.

###URL Types 추가
  * 현재 빌드의 Bundle identifier를 이용하여 URL Scheme을 작성해 준다.
  * Bundle identifier는 빌드시 입력받은 값을 이용하거나 Info.plist로부터 읽어와 사용

###스크립트 빌드
  1. Rakefile
	* 해당 스크립트를 실행하여 자동으로 빌드 및 세팅이 진행되도록 함.
  * 스크립트 커멘드 실행 순서
  > - 현재 실행 중인 Xcode프로젝트 종료
    - 빌드하려는 Unity 프로젝트 실행
    - 커멘드에 입력된 국가, 빌드 용도(프로비저닝), 광고 SDK에 맞게 빌드 실행
    - 설정이 완료된 Xcode프로젝트 실행
