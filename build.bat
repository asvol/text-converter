@echo off
chcp 65001

whoami

SET ASSM_INFO_FILE="./src/Asv.TextConverter/Properties/AssemblyInfo.cs"
SET SOLUTION_FILE=./src/Asv.TextConverter.sln
SET RELEASE_FILE="./bin/release/Asv.TextConverter.exe"
SET PUBLISH_FILE="./bin/publish/asv-text-converter.exe"
SET PUBLISH_DIR="../../bin/publish/"

SET BASH_PATH=%SYSTEMDRIVE%\Users\%USERNAME%\.babun\cygwin\bin\
SET PATH=%BASH_PATH%;%PATH%

SET MSBBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild"



git submodule sync --recursive
git submodule update --init --recursive

"tools/nuget.exe" config -set http_proxy=""

"tools/nuget.exe" restore -verbosity quiet src

zsh tools/update_version.sh %ASSM_INFO_FILE%

zsh tools/get_version.sh > ./bin/version.txt

SET /p VERSION=<bin\version.txt

rmdir /s /q bin\publish

mkdir bin\publish

echo %MSBBUILD% /consoleloggerparameters:ErrorsOnly /maxcpucount /nologo /verbosity:quiet /t:release /p:ApplicationVersion=%VERSION%;Configuration=Release;PublishDir=%PUBLISH_DIR% %SOLUTION_FILE%
%MSBBUILD% /consoleloggerparameters:ErrorsOnly /maxcpucount /nologo /verbosity:quiet /t:Build /p:ApplicationVersion=%VERSION%;Configuration=Release;PublishDir=%PUBLISH_DIR% %SOLUTION_FILE%

set BUILD_STATUS=%ERRORLEVEL%
if %BUILD_STATUS%==0 (
echo "  _____ _    _  _____ _____ ______  _____ _____ "
echo " / ____| |  | |/ ____/ ____|  ____|/ ____/ ____|"
echo "| (___ | |  | | |   | |    | |__  | (___| (___  "
echo " \___ \| |  | | |   | |    |  __|  \___ \\___ \ "
echo " ____) | |__| | |___| |____| |____ ____) |___) |"
echo "|_____/ \____/ \_____\_____|______|_____/_____/ "
)

if not %BUILD_STATUS%==0 echo "=======! Build failed !==========" && exit /b 1

zsh "tools/merge_release.sh" %RELEASE_FILE% %PUBLISH_FILE%

xcopy bin\release\NLog.config  bin\publish