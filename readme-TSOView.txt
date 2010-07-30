ダウンロードありがとうございます

●TSOView ver 0.3.1

これはなに
Direct3D上でTSOをレンダリングします。
カス子とほぼ同じ出力を得られます（toonshader対応）。
TMOを指定するとモーションを確認できます。

動作環境
・.NET Framework 3.5
・DirectX 9.0c
・toonshaderに対応したGPU
・ICSharpCode.SharpZipLib.dll (同梱)
・CSScriptLibrary.dll (同梱)

使い方
エクスプローラからTSOView.exeを起動します。
　png tso tmoファイルを画面上にドラッグ＆ドロップ (d&d) すると追加できます。
または
コマンドプロンプトから起動します。

TSOView.exe [file ... ]
　png/tso/tmo fileを任意の順序で指定します。
　png file：シーンセーブまたはヘビーセーブを指定します。
　tso file：tsoファイル名を指定するか、tsoファイルを含むフォルダ名を指定します。
　tmo file：tmoファイル名を指定します。

注意：
・TSOView.exeと同じフォルダにtoonshader.cgfx Script.csが必要です。
・必ずpng tso tmoファイルをバックアップしてください。

操作方法
　↑キー：Z軸方向に移動（ズームアップ）
　↓キー：Z軸方向に移動（ズームダウン）
　←キー：Y軸中心に左回転
　→キー：Y軸中心に右回転
　Page Upキー：X軸中心に回転（アップ）
　Page Downキー：X軸中心に回転（ダウン）
　Aキー：Z軸中心に左回転
　Dキー：Z軸中心に右回転

　スペースキー：モーションを一旦停止・再開
　リターンキー：スクリーンショット（sample.bmpを作成）

　Sキー：セルフシャドウ描画を切り替え
　Zキー：シャドウマップ描画を切り替え

　Tabキー：次のフィギュアを選択
　　選択フィギュアの root bone (W_Hips) の位置が回転の中心になります。
　Deleteキー：選択フィギュアを削除
　Ctrl+Deleteキー：全フィギュアを削除

　0キー：カメラ位置を初期位置に戻す

　Gキー：フィギュア設定画面を起動

マウス
　左ボタンを押しながらドラッグ：X/Y軸方向に回転
　中央ボタンを押しながらドラッグ：X/Y軸方向に移動
　右ボタンを押しながらドラッグ：Z軸方向に移動
　CRTLキーを押しながらクリック：ライト方向を指定

ドラッグ＆ドロップ
　tsoまたはpngをd&dすると全フィギュアを削除して置き換えます。
　Crtlキーを押しながらtsoまたはpngをd&dすると選択フィギュアに追加します。
　tmoをd&dすると選択フィギュアのモーションを置き換えます。


ver 0.3.0 からの変更点：
・画面サイズ変更時に解像度修正
・man.tso 男モーション対応

ver 0.2.9 からの変更点：
・XPC01766 新テク20100320版 + depth buffer shadow
・姉妹スライダ再現性を向上

ver 0.2.5 からの変更点：
・sources: TSOView, TMOComposer, TMOProportion を統合
・TMOProportion で作成した体型を自動適用
・体型スライダによる変形を再現
・フィギュア設定画面にスライダを追加

ver 0.1.9 からの変更点：
・VSMソフトシャドウ
・tmo未設定のときtsoからtmoを生成
・カメラ位置保存を廃止
・up固定カメラ
・BHLアルファチャンネルが逆になるバグを修正

ver 0.1.6 からの変更点：
・XPC00967 新テク20090622版 + depth buffer shadow
・tsoまたはpngをd&dすると全フィギュアを削除して置き換え
・細かいバグをたくさん修正

ver 0.1.5 からの変更点：
・はとぅねスクリプト的コマンドを用意 LoadMotion Motion Camera

ver 0.1.4 からの変更点：
・スクリプトを実行 (Script.cs)
・テクスチャ V座標の符号を反転（画像dataも反転）

ver 0.1.3 からの変更点：
・画面サイズを外部ファイルから設定 (config.xml)
・起動時はshader設定formを隠す

ver 0.1.2 からの変更点：
・カメラ位置を補間

ver 0.1.1 からの変更点：
・透明化処理を改善 (enable alpha test)
・カメラ位置を読み込み・保存

ver 0.1.0 からの変更点：
・XPC00838 新テク20090522版 + depth buffer shadow
・shader設定formを追加

ver 0.0.9 からの変更点：
・起動時はモーション停止
・起動時はセルフシャドウ無効

ver 0.0.8 からの変更点：
・フィギュア削除時にメモリ解放
・モーション速度を60 FPSに補正（たぶん）
