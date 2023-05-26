# 変数の定義
PROJECT_NAME = wevtutilCS
OUTPUT_DIR = bin/Release/net6.0
DLL_NAME = $(PROJECT_NAME).dll
WARNINGS =
# タスクの定義
build:
	dotnet build --configuration Release
publish:
	dotnet publish --configuration Release --output $(OUTPUT_DIR) -r win10-x64 --self-contained -p:PublishSingleFile=True -p:PublishTrimmed=True -p:TrimMode=Link -p:PublishReadyToRun=False
run:
	dotnet run --nowarn $(WARNINGS) --configuration Release
clean:
	dotnet clean
# ファイルの削除
clean-files:
	rm -rf $(OUTPUT_DIR)
# .NET Core プロジェクトのビルド
$(DLL_NAME): $(wildcard *.cs)
	dotnet build --configuration Release
# ビルド結果の出力先フォルダを作成する
$(OUTPUT_DIR):
	mkdir -p $(OUTPUT_DIR)
# .NET Core プロジェクトのビルドと実行
all: $(OUTPUT_DIR) $(DLL_NAME)
	dotnet $(OUTPUT_DIR)/$(DLL_NAME)
# phonyターゲットの定義
.PHONY: build publish run clean clean-files all