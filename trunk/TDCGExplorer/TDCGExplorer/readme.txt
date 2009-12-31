TDCGExplorer 始めにお読み下さい

基本全自動でなんでもかんでも管理するツールです。
Version 1.10になりました。

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

Windows7 x64 Professionalで動作検証しています。x64環境に対応しました。

WindowsXPで表示が重い事があります。これは.NET Framework 3.0以降の
制限です。Windows Vista、Windows7にアップグレードすると解消します。

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
     セーブファイルビューアのメモリリークを修正。アイコンや画像を表示している状態で
     データベースを再構築するとエラーでアプリが終了する不具合を修正。リファレンス
     マニュアルから総合マニュアルにマニュアルを改訂。マニュアルをオンラインマニュアル化。
     データグリッドビュー全部で行を選んでDELキーを押すと行が削除できてしまえるバグ修正。
     ポーズファイルのサムネを作り続けるとOut of memory exceptionが出るバグを修正。
1.09.1
     TAHWriterでThumbs.dbのハッシュ値がおかしいバグを修正。実害は無いですが。
1.09.2
     ポーズファイルで時々表示されないバグを修正。
1.09.3
     Windows7/x64対応しました。tahファイルが削除されずに例外で落ちる不具合修正しました。
　　 スプラッシュスクリーンのななみちゃんは、hissaさんのななみちゃんをお借りしました。
1.09.4
     検索を高速化。スプラッシュスクリーンのサイズがいつの間にかに変わっていたので修正。
1.09.5
     連続して同じファイルのサムネイルを作成すると、作成したポーズファイルが壊れるバグを修正。
1.09.6
     TAHキャッシュファイルの制限を緩和。tahファイル名に関するバグを修正。
1.10
     セーブファイルのスライダーに対応。TDCG.DLL 008にバージョンアップ。
1.10.1
     Vista英語版+日本語ランゲージパックで動作しない不具合を修正。
     英語版とソースコードを統合。HongfireのClawさんありがとう。


Please read TDCGExplorer. 

It is all tools managed by a basic all automatic operations. 
It became Version 1.10. 

･Important news

If here was not read, it embarrassed it. 

- 07-11 Please erase and restructure arcs.db of the version before beta. 

- Operation might become amusing because of no cash adjustment and arcs.
  db break if System.db is rewritten excluding TDCGEXPLORER. 

- Please update the data base after changing the initialization of the data base
  beforehand when you change fuck-shit child's location and the location of ZIP. 
  Contents of arcs.db disappear when it makes a mistake in order. 

- Initialization increases fast by a thing advanced by development. 
  Please default value must be set automatically, and open initialization and 
  confirm a set value when the version goes up. 
  (When the version goes up, system.db is automatically converted into the latest version. )

- To distribute N001OBON_Z00.tbn again, it is permitted only to the purchase of the following either. 

(1) 3D custom girl XP with privilege tah of 2008/12/24 sales. 
(2) Ten war maiden sisters in Walkure training and Samen tank. 

(note)Privilege tah is not attached to 3D custom girl XP who is putting
      it on the market now. The usages other than building this file into
      3D custom girl violate the use permission and hold back, please. 

･File composition

ArchiveLib.dll
Unrar.dll compression file component ICSharpCode.SharpZipLib.dll

Ifpsd.spi Photoshop [fairupa-sa]

TDCG.dll TSOView assembly
TDCG.xml (There is a correction only for TDCGExplorer). 

Default heavy save for default.tdcgsav.png thumbnail making
(When this file is changed, it is thumbnail revokable. )

Default icon file for TBN data N999SAVE_A00.psd TAH creation of file for N001OBON_Z00.tbn Z category
(When this file is changed, it is icon revokable. )

Default pose for this file SnapShotPose.tdcgpose.png save file display of file name list readme.txt for names.txt hush reverse-haul
(When this file is changed, it is pose of the save file revokable. )

System.Data.SQLite.DLL System.Data.SQLite.xml SQLite data base engine

Main body of TDCGExplorer.exe program

toonshader.cgfx Toonshader2

Icon file when there is no JPEG file noicon.jpg icon for title.jpg splash screen

Source.zip source code

･Notes

.It is necessary since DirectX9 Mar-2009 since NET Framework 3.5SP1. 
.It doesn't operate before NET Framework 3.5 and before DirectX9 Jan-2009. 

Please make the backup for all files ahead of the use. 
There is overwriting an existing file according to the operation. 

The operation verification is done with Windows7 x64 Professional and
WindowsVista x86 Business. It corresponded to the x64 environment. 

It is WindowsXP and there is a thing with a heavy display.
This. It is a limitation since NET Framework 3.0.
It cancels it when upgrading to Windows Vista and Windows7. 

The lzh file might be inaccessible in intel ATOM. 
It is likely to be able to correspond by the thing updated to latest BIOS ROM. 

It is likely to crash when 3D custom girl +3D custom girl XP is not installed. 

Please execute it with PC in which 3D custom girl XP is installed without fail
because the installed thing is a major premise (not acceptable the trial version). 

When operating with SSD, the law excessive wear leveling or freezing,
and it hurts and longevity might be shortened shortening SSD.
Please use HDD for the disk with My Documents or the Documents folder. 

Various functions of TDCGExplorer apply to the use permission of 3D custom girl
and 3D custom girl XP. Please use the use 3D custom girl XP official site permission well reading. 

Are 2ch IRC #3D custom do function demand bug report girls or 3dcustomgirl@gmail.com I hope in the workman guild and E-mail. 

･To the person who uses it for the first time

The first start

(1) UNLHA32.DLL is obtained from the integrated archiver project and it installs it in the system. 
http://www.madobe.net/archiver/index.html

(2) Initialization is executed by the data base menu by starting the program. 

(3) Various path names (arcs and zip file depository, etc.) are set. Please use other elements like default if you do not understand. 

(4) Construction and the update of the data base are executed. It takes a considerable time. Please leave PC for a while. 

Start from the second times

(1) Please execute construction and the update of the data base when you have changed contents of arcs and the ZIP depository (It makes an error of the operation to tah and zip that was not able to be accessed though there is no problem even if it doesn't execute it only if the data that exists in the data base is operated). 

･Special thanks

TAHDUMP Mr.nomeu
The TAH file analysis part was developed. 

TDCG.DLL Mr.nomeu
An important core component assembly was developed. 

names.txt Mr.nomeu
It used it for the naming part of an anonymous file. 

TDCGMan
Some core components are misappropriated. 

TSO2MQO
Some core components are misappropriated. 

TAHdecryptor
It referred to the bug correction of LZSS. 

TAHdecGUI
I was allowed to refer to a basic function. 

SQLite
It used it as a center of the core component. 

TDCGSaveFileViewer
The save file analysis part is misappropriated. 

toonshader2
Mr. kemokemo's toonshader2 Schaeder was used. 

3D custom girl
The save file pose was used. 

3D custom girl XP
The tbn file that existed only in the premium clothes of that day of the sale of the limitation was used. 

[Merusennutsuisuta] MT19937

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
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
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
It became reference that analyzed the mystery in less than LZSS 32 byte. 

Code of source uncertain source
The IArchive interface, zip, and the rar development component are used. 
There is no source code of this component though it is a part of TDCGMan. 

Microsoft .NET CLI derivation code
The source code generated based on the meta data is contained. 
CLI and CLR are infrastructures to which Microsoft retains one's right. 

A lot of other components were used. 
Thank you for authors. 

The misappropriation modification of the source code is free. 
The participation hope is welcomed in the development project aiming at the upcoming version. 

1.10.1
   The trouble that doesn't operate by Vista English version + Japanese language packing is corrected. 
   An English version and the source code are integrated. Thank you for Mr. Claw about Hongfire. 

Konoa/N765
