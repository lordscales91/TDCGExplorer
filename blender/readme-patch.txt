Blender 3DCG Script Ver.0.21 patch 3

■概要
Blender 3DCG Script Ver.0.21 のうちtsoインポータ/エクスポータの修正版です。

■ファイル構成

	Python\3dcg_tso_import.py      tsoファイルインポート用Pythonスクリプト
	Python\3dcg_tso_export.py      tsoファイルエクスポート用Pythonスクリプト
	Python\NvTriStripper-cli.exe   tsoエクスポータが呼び出すツール
	Python\tdcg.py	tsoファイル読み込み用Pythonスクリプト


■導入方法

Blenderのスクリプトフォルダに拡張子 .py .exe のファイルをコピーして下さい。
例    C:\Program Files\Blender Foundation\Blender\.blender\scripts   等...

同名のPythonスクリプトが既にある場合は上書きしてください。
使い方など詳しくは元の配布(XPC01974)をご参照ください。


■動作環境
Blender 2.49b / Python 2.6


■変更履歴
0.21 patch 3
in 3dcg_tso_export.py:
・頂点並びを最適化: NvTriStripper-cli.exe from anz2blend-0.4.0
・export設定を読み込めないバグを修正: Registry Keyをtso_exportに変更
0.21 patch 2
in 3dcg_tso_import.py:
・同じ位置で異なる材質の頂点を同一視
0.21 patch
in 3dcg_tso_import.py:
・ある面でUVがおかしくなるバグを修正
・tsoファイル読み込みにTSOFileクラスを使う
in tdcg.py:
・TSOFileクラスを作成


バグ報告などは居酒屋改造スレでお願いします。
--
nomeu  <n.nomeu@gmail.com>
