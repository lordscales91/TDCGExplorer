ダウンロードありがとうございます

●TMOProportion ver 0.3.2

これはなに
体型変更を簡単に行うためのツールです。

TAHBoin系ツールと比較して、下記のような利点があります。
・リアルタイムに体型変更の結果を確認できます。
・体型変更の割合を変更できます。
・複数の体型を一度に処理します。
・体型パラメータを編集できます。

動作環境
・.NET Framework 3.5
・DirectX 9.0c
・toonshaderに対応したGPU
・ICSharpCode.SharpZipLib.dll (同梱)
・CSScriptLibrary.dll (同梱)

使い方
下記のプログラムがあります。
・TMOProportion.exe：体型変更レシピを作る。
・TAHProportion.exe：体型変更レシピを使って差分tahを作る。
・TPOEditor.exe：体型パラメータを編集する。

・PNGProportion.exe：体型変更レシピを使ってpngを体型変更する（コマンドライン）。
・TMOProp.exe：体型変更レシピを使ってtmoを体型変更する（コマンドライン）。

◆TMOProportion.exe
エクスプローラからTMOProportion.exeを起動します。
マイドキュメントのカス子セーブ (system.tdcgsav.png) が自動的に読み込まれます。

	参考：png tso tmoファイルを画面上にドラッグ＆ドロップ (d&d) すると追加できます。

注意：
・TMOProportion.exeと同じフォルダにtoonshader.cgfxが必要です。
・必ずpng tso tmoファイルをバックアップしてください。

操作方法
操作方法は基本的にTSOViewと同じです（ただしキーボードでの操作は無効）。

マウス
　左ボタンを押しながらドラッグ：X/Y軸方向に回転
　中央ボタンを押しながらドラッグ：X/Y軸方向に移動
　右ボタンを押しながらドラッグ：Z軸方向に移動
　Ctrlキーを押しながらクリック：ライト方向を指定

体型変更スライダ
左右に動かすと、体型変更の割合を変更できます。

Saveボタン
Saveボタンを押すと、体型変更レシピとしてスライダ情報を保存します（TPOConfig.xml）。


◆TAHProportion.exe
エクスプローラからTAHProportion.exeを起動します。
体型変更レシピは自動的に読み込まれます。

注意：
・TAHProportion.exeと同じフォルダにTPOConfig.xmlが必要です。
・必ずtahファイルをバックアップしてください。

Loadボタン
Loadボタンを押すと、ファイル選択ダイアログが開きます。
ここで、差分を作るtahファイルを選択します。
普通はbase.tahかbase_xp.tahを選択することになります。

Compressボタン
tahファイルを選択したらCompressボタンを押します。
すると、tahファイル中のtmoを展開して体型変更レシピを適用しつつ差分tahを作ってくれます。
注意：base.tahやbase_xp.tahにはtmoが大量に含まれるため、処理には時間がかかります。

差分tahはプログラム起動時のフォルダに作られます。
差分tahのファイル名は元のtahファイル名の先頭に tmo- を付加した名前になります。
例：base.tah なら tmo-base.tah が作られます。
注意：差分tahのversionは 16 (0x10) になります。

あとは差分tahをarcsフォルダに移動してください。
これで体型変更完了です！


◆TPOEditor.exe
readme-TPOEditor.txtを参照


◆体型ファイル
Proportion/*.cs は体型ファイルです。
これは C# script形式のファイルです。TPOEditor.exeまたはテキストエディタで編集できます。

標準体型ファイル
・Boin.cs：TAHBoin TMOBoinと同じ体型 (TAC00155 XPC00970)
・Pedo.cs：TAHPedo TMOPedoと同じ体型 (TAC00156 XPC00970)
・Dom.cs：TAHDom TMODomと同じ体型 (XPC00342 XPC00970)
・Ika.cs：TAHIka TMOIkaと同じ体型 (XPC00953 XPC00965)
・Kemo.cs：TAHKemo TMOKemoと同じ体型 (XPC01071)
・Zoom.cs：身体を拡大する体型（サンプル体型ファイル）


◆PNGProportion.exe
コマンドラインから起動します。
PNGProportion.exe your.tdcgpose.png

体型変更レシピは自動的に読み込まれます。
注意：pngは上書きされます。


◆TMOProp.exe
コマンドラインから起動します。
TMOProp.exe your.tmo

体型変更レシピは自動的に読み込まれます。
注意：tmoは上書きされます。


ver 0.3.0 からの変更点：
・TAHProportion: 標準ポーズを処理しないバグを修正

ver 0.2.8 からの変更点：
・フィギュアを入れ替えると体型変更スライダが効かなくなるバグを修正

ver 0.0.4 からの変更点：
・sources: TSOView, TMOComposer, TMOProportion を統合

ver 0.0.3 からの変更点：
・TPOEditor.exeを追加
・TMOProp.exeを追加
・カス子セーブを読み込む（tmoを自動生成）

ver 0.0.2 からの変更点：
・Move変形がスライダを無視するバグを修正

ver 0.0.1 からの変更点：
・ポーズエディタの標準ポーズに対応
