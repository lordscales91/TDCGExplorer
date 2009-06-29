ダウンロードありがとうございます

●TMOTool ver 0.0.3

これはなに
簡単にtmoを修正するためのツールキットです。

注意事項
・再配布は原則として禁止とします
・添付ソースの利用は自由です

動作環境
・.NET Framework v3.5
・DirectX 9.0c

使い方
コマンドプロンプトから起動します。
具体的な使い方についてはチュートリアル (tutorial.htm) を参照してください。

TMOTool.exe <tmo file> [script name ...]
　指定したtmo fileに対して体型変更を行います。
　フォルダを指定すると再帰的に処理します。

　script nameには体型変更スクリプトの名前を指定します。
　Boin Dom Pedo Ika Kemo を指定できます（複数指定可）。

　処理したtmo fileはカレントディレクトリに書き出されます。

TMOMove.exe <tmo file> [x y z]
　指定したtmo fileをx y z方向に移動します。
　注意：ファイルは上書きされます。

　x y zを省略すると、現在の位置をx y zの順に表示します。

TMORotY.exe <tmo file> <angle>
　指定したtmo fileをangle角度で Y軸回転します。
　注意：ファイルは上書きされます。

TMOZoom.exe <tmo file> <ratio>
　指定したtmo fileをratio倍率で拡大縮小します。
　注意：ファイルは上書きされます。

TMOBoin.exe <tmo file>
　指定したtmo fileを膨乳にします。
　注意：ファイルは上書きされます。

TMODom.exe <tmo file>
　指定したtmo fileを巨尻にします。
　注意：ファイルは上書きされます。

TMOPedo.exe <tmo file>
　指定したtmo fileを幼女にします。
　注意：ファイルは上書きされます。

TMOAppend.exe <source tmo> <motion tmo>
　source tmoにmotion tmoを追加します。
　注意：ファイルは上書きされます。

TMOMotionCopy.exe <source tmo> <motion tmo>
　source tmoをひな型としてmotion tmoの動きを合成します。
　注意：ファイルは上書きされます。

TMONodeCopy.exe <source tmo> <motion tmo> [node name=W_Hips]
　source tmoにmotion tmoのポーズを複写します。
　注意：ファイルは上書きされます。
　注意：nodeの並びが同じでないと、うまく処理できません。

　複写の起点とするnode nameを指定できます（省略するとW_Hips）。
　指定したnodeから子nodeをたどっていきます。
　例えば、
　W_Neckを指定すると、首から先（顔の表情など）を複写、
　W_Hipsを指定すると、全体を複写します。

TMONodeDump.exe <tmo file>
　指定したtmo fileのボーンを出力します。

TMOPose.exe <tmo file> [frame=0]
　指定したtmo fileをframeのポーズのみにします。
　注意：ファイルは上書きされます。

　frameを省略すると、最初のポーズを適用します。

注意
・必ずtmoファイルをバックアップしてください。


開発者の方へ
ソース (sources) を同梱しています。
致命的な間違いは、改造スレまで報告いただけると幸いです。
