set GEN_CLIENT=dotnet ..\Luban.ClientServer\Luban.ClientServer.dll
set INPUT_ROOT=.
set INPUT_DEFINES_RELATIVE_DIR=Defines
set INPUT_DEFINES_RELATIVE_PATH=Defines\__root__.xml
set INPUT_DATA_RELATIVE_DIR=Datas
set OUT_ROOT=..\..\Assets
set OUT_DATA_RELATIVE_DIR=Bundles\Config\LubanConfig
set OUT_CODE_RELATIVE_DIR=Scripts\Generate\Config\ConfigGenerate
set OUT_EXCLUDE_TAGS=

%GEN_CLIENT% ^
-j cfg ^
-w %INPUT_ROOT%\%INPUT_DEFINES_RELATIVE_DIR%,%INPUT_ROOT%\%INPUT_DATA_RELATIVE_DIR% ^
 --^
 -d %INPUT_ROOT%\%INPUT_DEFINES_RELATIVE_PATH% ^
 --input_data_dir %INPUT_ROOT%\%INPUT_DATA_RELATIVE_DIR% ^
 --output_data_dir %OUT_ROOT%\%OUT_DATA_RELATIVE_DIR% ^
 --output_code_dir %OUT_ROOT%\%OUT_CODE_RELATIVE_DIR% ^
 --gen_types code_cs_unity_json,data_json ^
 -s client ^
 --output:exclude_tags %OUT_EXCLUDE_TAGS% ^
pause