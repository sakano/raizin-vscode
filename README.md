# Raizin extension for Visual Studio Code

[Visual Studio Code](https://code.visualstudio.com/) に雷神7/八星帝のイベントスクリプトのサポート機能を追加する拡張機能です。

![Preview image](https://github.com/user-attachments/assets/0ea40842-7988-48f8-9188-079d39c61e4e)


# 全体の構成

## LanguageServer フォルダ

C# で作成された Language Server が格納されています。
Language Server Protocol を用いていますが、VSCode 以外のエディタではテストしていません。

## VSCodeClient フォルダ

TypeScript で作成された Language Server を起動して接続する VSCode 拡張が格納されています。


# デバッグ実行の方法

1. VSCode で VSCodeClient フォルダを開く
2. F5 キーを押す

以上で VSCodeClient と LanguageServer がビルドされ、開発中の拡張機能が読み込まれた状態の VSCode が起動します。
VisualStudio などから RaizinLanguageServer.dll を実行中の dotnet プロセスにアタッチすれば LanguageServer のデバッグもできます。


# パッケージングの方法

1. VSCode で VSCodeClient フォルダを開く
2. Ctrl+Shift+P でコマンドパレットを開く
3. Run Task と入力し、`Tasks: Run Task` を選択
4. `Package extension` を選択

以上で、ルートフォルダに `raizin-vscode-win32-x64-*.vsix` が生成されます。
