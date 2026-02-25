@echo off
chcp 65001 > nul
set "ASSETS_PATH=Assets"

:: 创建核心全局资源文件夹
mkdir "%ASSETS_PATH%\01_Core"
mkdir "%ASSETS_PATH%\01_Core\Scripts"
mkdir "%ASSETS_PATH%\01_Core\Scripts\Data"
mkdir "%ASSETS_PATH%\01_Core\Scripts\Utility"
mkdir "%ASSETS_PATH%\01_Core\Settings"
mkdir "%ASSETS_PATH%\01_Core\Resources"
mkdir "%ASSETS_PATH%\01_Core\Resources\Configs"
mkdir "%ASSETS_PATH%\01_Core\Resources\GlobalPrefabs"

:: 创建游戏玩法核心资源文件夹
mkdir "%ASSETS_PATH%\02_Gameplay"
mkdir "%ASSETS_PATH%\02_Gameplay\Customers"
mkdir "%ASSETS_PATH%\02_Gameplay\Customers\Prefabs"
mkdir "%ASSETS_PATH%\02_Gameplay\Customers\Prefabs\Chapter1"
mkdir "%ASSETS_PATH%\02_Gameplay\Customers\Prefabs\Chapter2"
mkdir "%ASSETS_PATH%\02_Gameplay\Customers\Sprites"
mkdir "%ASSETS_PATH%\02_Gameplay\Customers\Sprites\Chapter1"
mkdir "%ASSETS_PATH%\02_Gameplay\Customers\Sprites\Chapter2"
mkdir "%ASSETS_PATH%\02_Gameplay\Customers\Scripts"
mkdir "%ASSETS_PATH%\02_Gameplay\Props"
mkdir "%ASSETS_PATH%\02_Gameplay\Props\Prefabs"
mkdir "%ASSETS_PATH%\02_Gameplay\Props\Prefabs\Chapter1"
mkdir "%ASSETS_PATH%\02_Gameplay\Props\Prefabs\Chapter2"
mkdir "%ASSETS_PATH%\02_Gameplay\Props\Sprites"
mkdir "%ASSETS_PATH%\02_Gameplay\Props\Sprites\Icons"
mkdir "%ASSETS_PATH%\02_Gameplay\Props\Sprites\Display"
mkdir "%ASSETS_PATH%\02_Gameplay\Props\Scripts"
mkdir "%ASSETS_PATH%\02_Gameplay\Maker"
mkdir "%ASSETS_PATH%\02_Gameplay\Maker\Prefabs"
mkdir "%ASSETS_PATH%\02_Gameplay\Maker\Sprites"
mkdir "%ASSETS_PATH%\02_Gameplay\Maker\Scripts"

:: 创建场景文件夹
mkdir "%ASSETS_PATH%\03_Scenes"
mkdir "%ASSETS_PATH%\03_Scenes\01_Boot"
mkdir "%ASSETS_PATH%\03_Scenes\02_ChapterSelect"
mkdir "%ASSETS_PATH%\03_Scenes\03_Gameplay"
mkdir "%ASSETS_PATH%\03_Scenes\04_ChapterTransition"

:: 创建UI文件夹
mkdir "%ASSETS_PATH%\04_UI"
mkdir "%ASSETS_PATH%\04_UI\Prefabs"
mkdir "%ASSETS_PATH%\04_UI\Sprites"
mkdir "%ASSETS_PATH%\04_UI\Sprites\Buttons"
mkdir "%ASSETS_PATH%\04_UI\Sprites\Backgrounds"
mkdir "%ASSETS_PATH%\04_UI\Sprites\Icons"
mkdir "%ASSETS_PATH%\04_UI\Scripts"

:: 创建美术资源文件夹
mkdir "%ASSETS_PATH%\05_Art"
mkdir "%ASSETS_PATH%\05_Art\Scenes"
mkdir "%ASSETS_PATH%\05_Art\Scenes\Chapter1"
mkdir "%ASSETS_PATH%\05_Art\Scenes\Chapter1\Sprites"
mkdir "%ASSETS_PATH%\05_Art\Scenes\Chapter1\Models"
mkdir "%ASSETS_PATH%\05_Art\Scenes\Chapter2"
mkdir "%ASSETS_PATH%\05_Art\Scenes\Chapter2\Sprites"
mkdir "%ASSETS_PATH%\05_Art\Scenes\Chapter2\Models"
mkdir "%ASSETS_PATH%\05_Art\Effects"
mkdir "%ASSETS_PATH%\05_Art\Effects\Prefabs"
mkdir "%ASSETS_PATH%\05_Art\Effects\Textures"
mkdir "%ASSETS_PATH%\05_Art\Audio"
mkdir "%ASSETS_PATH%\05_Art\Audio\BGM"
mkdir "%ASSETS_PATH%\05_Art\Audio\SFX"
mkdir "%ASSETS_PATH%\05_Art\Audio\Voice"

:: 创建测试资源文件夹
mkdir "%ASSETS_PATH%\06_Testing"
mkdir "%ASSETS_PATH%\06_Testing\TestScenes"
mkdir "%ASSETS_PATH%\06_Testing\TestPrefabs"

echo 文件夹创建完成！
pause