またarcs.dbのテーブルを変更しました。TechArts3D\TDCG\TDCGExplorer下の
arcs.dbファイルは削除してから、Create from arcsを再実行してください。

二回目のCreate from arcsの実行速度を大幅に高速化しました。この速度なら
バックグラウンドで動かす必要は無いような気もしますが、それはさておき。

TAHファイル展開の途中で例外が発生してしまう為にdbに全ての情報が格納
されないtahファイルが何種類か見つかっています。しかも最低な事にその
MODの一つは私の自作でした。

TDCG.DLLと、ArchiveLib.DLLの間で、ICSharpCode.SharpZipLib.dllの
バージョン不整合がおきており暫定でTDCGExplorer.exe.configファイルを
配置して対策していますが、これは最終仕様ではありません。

ZIPファイルが展開できなかった不具合を修正しましたが、それでもまだ
展開できないZIPファイルがあります（どうしてくれようZIPの仕様）。

・簡単な使い方

初めての起動

(1) 統合ArchiverプロジェクトからUNLHA32.DLLを入手して
　　システムにインストールする。
(2) DatabaseメニューでEdit system databaseを実行。
(3) 各種パス名（arcsやzipファイル置き場など）を設定。
　　それ以外の要素は判らないならデフォルトのままで
　　使ってください。
(4) Create from arcsを実行。かなり時間がかかります。
　　しばらくPCを放置しておいてください。
(5) Database build completeと下に出たら、
　　Display arcs databaseを実行するとツリー表示。

２回目からの起動

(1) 初めての起動手順を実行してあると、自動的に(5)まで
　　スキップする。
(2) arcsやZIP置き場を変更している場合は、Create from arcsを
　　再度実行する。大きな変更が無ければあっという間に終わる。
(3) DBを更新した時はDisplay arcs databaseを実行すると表示を
　　更新する。

・ファイルはどこにできるの？

XPの場合、マイドキュメントのTechArts3D\TDCG\TDCGExplorerに、
Vistaの場合、ドキュメントのTechArts3D\TDCG\TDCGExplorerに
各種データベースやキャッシュファイルを配置します。ZIP置き場の
デフォルトもこのディレクトリの下になっていますが、System Databaseを
変更する事で他のディレクトリに変更できます。
