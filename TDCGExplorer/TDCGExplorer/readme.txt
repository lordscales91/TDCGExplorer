TDCGExplorer 始めにお読み下さい

基本全自動でなんでもかんでも管理するツールです。
Version 1.09になりました。

・重要なお知らせ

ここを読まないと困った事になります。

※ 07-11 betaより前のバージョンのarcs.dbは消して再構築して下さい。

※ System.dbをTDCGEXPLORER以外から書き換えるとキャッシュ不整合で
   動作がおかしくなったりarcs.dbが壊れることがあります。

※ カス子の置き場所やZIPの置き場所を変更した時はデータベースの
   初期設定を予め変更してからデータベースの更新をして下さい。
   順番を間違えるとarcs.dbの中身が消えます。

※ 初期設定は開発が進む事でどんどん増えます。動作に差し支えないで
   あろうデフォルト値が自動的に設定されますので、バージョンが
   あがったら初期設定を開いて設定値を確認してください。
   (system.dbはバージョンが上がると自動的に最新版に変換されます)

※ N001OBON_Z00.tbnを再配布するには、次のいずれかを購入している
   方だけに許諾されています。

   (1) 2008/12/24発売の特典tah付き3Dカスタム少女XP。
   (2) ワルキューレ調教・ザーメンタンクの戦乙女１０人姉妹。

   (注) 現在発売中の3Dカスタム少女XPには特典tahは付属していません。
        このファイルを3Dカスタム少女に組み込む以外の用途は
        使用許諾に違反しますのでご遠慮ください。

・ファイル構成

        ArchiveLib.dll
        unrar.dll                               圧縮ファイルコンポーネント
        ICSharpCode.SharpZipLib.dll

        ifpsd.spi                               Photoshopファイルパーサ

        TDCG.dll                                TSOViewアセンブリ
        TDCG.xml                                (TDCGExplorer専用に修正が入っています)

        default.tdcgsav.png                     サムネイル作成用デフォルトヘビーセーブ
                                                (このファイルを変更するとサムネ用キャラクタを変更できます)
        N001OBON_Z00.tbn                        Zカテゴリ用TBNデータ
        N999SAVE_A00.psd                        TAHファイル作成用デフォルトアイコンファイル
                                                (このファイルを変更するとアイコンを変更できます)
        names.txt                               ハッシュ逆引き用ファイル名一覧
        readme.txt                              本ファイル
        SnapShotPose.tdcgpose.png               セーブファイル表示用デフォルトポーズ
                                                (このファイルを変更するとセーブファイルのポーズを変更できます)
        System.Data.SQLite.DLL
        System.Data.SQLite.xml                  SQLiteデータベースエンジン

        TDCGExplorer.exe                        本体プログラム

        toonshader.cgfx                         Toonshader2

        title.jpg                               スプラッシュスクリーン用JPEGファイル
        noicon.jpg                              アイコン無しの場合のアイコンファイル

        source.zip                              ソースコード

・注意事項

.NET Framework 3.5SP1以降、DirectX9 Mar-2009以降が必要です。
.NET Framework 3.5以前・DirectX9 Jan-2009以前では動作しません。

ご利用の前に全てのファイルは必ずバックアップを作成してください。
操作によっては既存のファイルを上書きする事があります。

Windows Vista Business "英語版" x86バージョンで動作確認しています。
それ以外のOSでは現時点では未検証なのでご注意ください。

WindowsXPで表示が重い事があります。これは.NET Framework 3.0以降の
制限です。Windows Vista、Windows7にアップグレードすると解消します。

x64プラットフォームでの動作報告を頂戴いたしました。
作者はx64 Windowsを所有していない為、厳密な動作検証はしておりません

intel ATOMでlzhファイルにアクセスできないことがあります。
最新のBIOS ROMにアップデートする事で対応できる場合があります。

3Dカスタム少女+3Dカスタム少女XPがインストールされていない場合
クラッシュすることがあります。

インストールされている事が大前提なので必ず3Dカスタム少女XPが
インストールされているPCで実行してください（体験版不可）。

SSDで動作させると過剰なウエアレベリングがおきてフリーズしたり
SSDを痛めて寿命を縮める恐れがあります。My Documentsもしくは
DocumentsフォルダのあるディスクはHDDをご利用ください。

TDCGExplorerの各種機能は3Dカスタム少女及び3Dカスタム少女XPの
使用許諾に準じます。製品の取り扱い説明書の使用許諾及び、
3Dカスタム少女XP公式サイトの使用許諾を良くお読みになって
ご利用ください。

機能要望バグ報告は2ch IRC #3Dカスタム少女か、職人ギルド、
電子メールで3dcustomgirl@gmail.comにお願いいたします。

・初めてご利用になる方に

初めての起動

(1) 統合アーカイバプロジェクトからUNLHA32.DLLを入手して
　　システムにインストールする。
　　http://www.madobe.net/archiver/index.html

(2) プログラムを起動してデータベースメニューで初期設定を実行。

(3) 各種パス名（arcsやzipファイル置き場など）を設定。
　　それ以外の要素は判らないならデフォルトのままで
　　使ってください。

(4) データベースの構築・更新を実行。かなり時間がかかります。
　　しばらくPCを放置しておいてください。

２回目からの起動

(1) arcsやZIP置き場の中身を変更している場合はデータベースの
　　構築・更新を実行してください（データベース上にあるデータを
　　操作するだけなら実行しなくても問題はありませんが、アクセス
　　出来なくなったtahやzipへの操作がエラーになります）。

・謝辞

TAHDUMP nomeu氏
TAHファイル解析部を開発して頂きました。

TDCG.DLL nomeu氏
重要なコアコンポーネントアセンブリを開発して頂きました。

names.txt nomeu氏
匿名ファイルの命名部に使用しました。

TDCGMan
幾つかのコアコンポーネントを流用しています。

TSO2MQO
幾つかのコアコンポーネントを流用しています。

TAHdecryptor
LZSSのバグ修正の参考にしました。

TAHdecGUI
基本的な機能を参考にさせて頂きました。

SQLite
コアコンポーネントの中枢として使用しました。

TDCGSaveFileViewer
セーブファイル解析部を流用しています。

toonshader2
kemokemo氏のtoonshader2シェーダーを使用しました。

3Dカスタム少女
セーブファイルポーズを使用しました。

3Dカスタム少女XP
発売当日限定のプレミアム衣装にしか無いtbnファイルを使用しました。

メルセンヌツイスタ MT19937

   A C-program for MT19937, with initialization improved 2002/1/26.
   Coded by Takuji Nishimura and Makoto Matsumoto.

   Before using, initialize the state by using init_genrand(seed)
   or init_by_array(init_key, key_length).

   Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
   All rights reserved.

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote
        products derived from this software without specific prior written
        permission.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

   Any feedback is very welcome.
   http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
   email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)

N256MOD (N256HOHO.TAH)
LZSS 32バイト未満の謎を解析する参考になりました。

ソース出典不明のコード
IArchiveインターフェース及びzip,rar展開コンポーネントを使用しています。
TDCGManの一部でありますがこのコンポーネントのソースコードはありません。

Microsoft .NET CLI派生コード
メタデータをもとに生成したソースコードを含んでいます。
CLI及びCLRはMicrosoftが権利を保有するインフラストラクチャです。

その他多数のコンポーネントを使用しました。
作者の皆様、ありがとうございます。

ソースコードの流用改変は自由です。次のバージョンに向けて開発
プロジェクトに参加希望の方を歓迎しております。

・変更履歴

1.00 リリース初版
1.01 EXEをファイルドラッグドロップで起動するとtmoファイルが読み込めないバグを修正
1.02-1.03 色々修正(忘れた)
1.04 tdcg.dllのソースを分岐、プロジェクトに統合(本家はTMOComposer開発の為互換性が無くなる)
1.05 アイコン表示対応、tahバージョンチェック追加、zipフォルダを開かない様にする
1.06 tah梱包時に梱包順序をソートする様に変更
1.07 beta版 Techが作成したtahファイルにアクセスできないバグを修正。
     ポーズファイル管理機能をちょっとだけ追加(サムネイル差し替え機能のみ)
1.07 ポーズファイルからtmo・tah抽出を追加.ファイルのリネーム、タイムスタンプ書き換え追加。
     ファイル保存時にセーブファイルダイアログを開いて保存先やファイル名を設定可能に。
     最小化、最大化した状態で終了するとウインドウの位置がおかしくなるバグを修正しました。
1.07.2
     データベース更新時にメモリリークがあったのを修正しました
1.07.3
     特定のヘビーセーブがTAHに展開できない不具合を修正。
     TAH展開ルーチンで推定可能な名前を自動割付する様に修正。
1.07.4
     カテゴリ変更で数字のカテゴリに変更できないバグを修正
1.07.5
     TAHEditorでカテゴリ変更・ファイル名変更をした際にソートオーダーが破壊されて、
     エラーが発生するバグを修正。アイコンがないファイルを選択した時は、NO IMAGEを
     表示するように修正。
1.07.6
     セーブファイルはアイコンを表示し、テーブルをダブルクリックもしくはメニュー
     コマンドを実行した時にデータを表示する様変更。
1.07.7
     セーブファイル・ポーズファイルをダブりクリックでTSOビューワに表示。マニュアル改訂。
1.07.8
     セーブファイル表示→新規タブ→ダブルクリックで落ちるバグを修正。
1.08
     TSOView ver 0.2.5 TDCG.DLLアセンブリにバージョンアップ。
     BHL系テクニックで表示に不具合が出ていた問題に対応。
     TDCG.DLLの座標系が変更になったので大規模修正。
1.08.1
     00番tbnがない迷子アイテム用のダミーtbn作成機能を追加。
     TAH集約機能を追加。
1.08.2
     manualのアンカーが機能していない不具合を修正。
     TAH集約再梱包の時、読み込めるTAHを4GBに制限。
     シーンセーブ・ポーズセーブの背景色を変更。公式っぽい色に。
1.08.3
     英数字を含まないzipがアーカイブとして認識されない不具合を修正。
     arcsで使えない文字を自動置換するように修正。
1.08.4
     セーブファイル表示をなるべく軽量化。
     セーブファイルのアイテムから対応tah/zipへのジャンプを追加。
1.09
     検索機能を強化。検索結果をリストアップして、ダブルクリックで目的のファイルに
     ジャンプする機能を追加。またUI統合の為、前提TAH検索でもクリックではなく
     ダブルクリックでジャンプする様に仕様を変更。ポーズファイルのドラッグドロップに対応。
     セーブファイルビューアのメモリリークを修正。

Konoa/N765
