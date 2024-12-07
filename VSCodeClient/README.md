# Raizin extension for Visual Studio Code

[Visual Studio Code](https://code.visualstudio.com/) に雷神7/八星帝のイベントスクリプトのサポート機能を追加する拡張機能です。

![Preview image](https://github.com/user-attachments/assets/0ea40842-7988-48f8-9188-079d39c61e4e)


## 機能一覧
- シンタックスハイライト
- 人物ID、惑星ID、状態IDなど数値指定で分かりづらいパラメータにインレイヒントを表示
- コマンド名、パラメータの補完
- コマンド入力中にドキュメントを表示
- コマンドやパラメータにマウスホバーするとドキュメントを表示
- txtload、gotoコマンドからジャンプ先を開く
- アウトラインにラベル一覧を表示
- ラベルの参照一覧を表示
- 人物ID、惑星ID、アイテム名などから pson1.txt などの定義ファイルへジャンプ
- 不明なパラメータなどのエラー表示


## 簡単な使い方
1. この拡張機能をインストール
2. e001.txt や u0001.txt などのスクリプトファイルをVSCodeで開く
3. VSCode右下の「プレーンテキスト」をクリックして一覧から「Raizin」を選択

![Language select](https://github.com/user-attachments/assets/419895af-b143-4cde-81dd-45efddd6552c)


## おすすめ設定
VSCodeからスクリプトファイルを開くたびに、言語設定をRaizinに切り替えるのは面倒です。VSCodeの設定を変更することで、eveフォルダ内のファイルを自動的にRaizin言語として開くように設定できます。

1. VSCode に eve フォルダをドロップする。
2. 上部メニューの ファイル > ユーザー設定 > 設定 を選択する。
3. ワークスペースに切り替えて、files.associations で検索する
4. 項目の追加をクリックし、項目には「*.txt」、値には「raizin」を入力する

![files.associations](https://github.com/user-attachments/assets/b3737ac0-12db-4b8b-b9d9-c810c6aee81a)

以上で、eve フォルダを開いてから txt ファイルを開くと、自動的に Raizin 言語として開かれるようになります。

他にも以下の設定を変更すると、より快適に使えるようになります。
- 「files.autoGuessEncoding」のチェックを外す
- 「files.encoding」を Japanese(Shift JIS) に変更する
  - ファイルが文字化けしなくなります。
- 「editor.unicodeHighlight.ambiguousCharacters」のチェックを外す
  - 全角文字の強調表示が無効にします。
- 「editor.inlayHints.enabled」を「offUnlessPressed」に変更する
  - 「on」のままだとインレイ表示が常に表示されますが、「offUnlessPressed」にすると、Ctrl+Altキーを押している間だけ表示されるようになります。
- 「Raizin-vscode」で検索すると、拡張機能の設定も変更できます。
  - 雷神7のeveフォルダの場合は「Target」を「八星帝」から「雷神7」に切り替えてください。
    - 使えるコマンドなどが変わります。
