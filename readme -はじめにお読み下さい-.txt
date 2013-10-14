ダウンロードありがとうございます

同梱ファイル
　・toonshader2.tah・・・新規technique追加用toonshader

コメ
　・toonshader2.tahをarcsフォルダに追加する事により、乗算、減算、ネガ反転等の
　　新規techniqueを使用したMODをゲーム内で正しく表示する事が出来ます。

更新履歴

//-----------------------------------------------------------------------------------
//◎20130916　ATI RADEON 2400 XT、NVidia GeForce GTX-660でデフォ男が消える不具合を修正
//-----------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------
//◎20130911　両面描画のBIASが抜けていたので修正、エフェクト追加
//　　　　　　本ドキュメントの商用禁止事項の削除、クリエイティブ・コモンズの明記
//-----------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------
//◎20130910　両面描画に対応(MODの表示可否に関係する重要な変更の為)
//            視野角については非対応（改変箇所が多いため）
//-----------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------
//★20120501　乗算、減算、ネガ反転の式を変更
//-----------------------------------------------------------------------------------
	その他改編有り
//-----------------------------------------------------------------------------------
//★20120401　MODS1118[toonshader2 UVスクロールで炎テスト]をAlphaScroll系techniqueとして追加
//-----------------------------------------------------------------------------------
	以下元ファイルのりーどみーより抜粋
		UVスクロールのアルファチャンネルとRGBチャンネルを分離して個別に動かせるようにしてみた。
		「toonshader2テスト確認後削除して下さい.tah」はその名の通りテスト版なので、現行のtoonshader2.tah
		を一度退避させてから使って下さい。
		「FIRETEST.tah」確認後は両tahと退避した現行toonshader2.tahを入れ替えるのを忘れずに。
		
		・FIRETEST.tah 手持ちアイテムのどこかにあります(^^; 線香花火の左あたりかも　アイコン無し
		　燃え上がる炎を作ってみたかったけど、従来のUVスクロールでは「徐々に消えていく炎の飛沫」を表現
		することが出来なかった。アルファチャンネルでテクスチャの端を透過状態にしても、アルファチャンネルも
		一緒にスクロールのでこれは無理。
		　ならばと言う事で、アルファチャンネルを固定して、テクスチャのRGBチャンネルを固定したアルファチャンネル
		の透過部へ動かす事で「徐々に消えて行く様に見せられるだろう」と思ってシェーダを弄りました。
		
		　本当なら花火作った時にこれが原因で色々と悩んだ経緯があるので、その時にやってしまえばよかったなぁと思う。
		もっと綺麗に作り直す事も出来そうだけど後回し（多分やらない）
		
		　焚き火とかみたいな、もうちょっと炎っぽく見せられる様に改良できたら実装しようかと思っています。
		
		20110313追記
		・テクスチャを変更しました。
		・東北地方太平洋沖地震被災地の復興支援活動に参加中の為、MOD作成やpixivその他諸々の活動をしばらくの間一時停止します。
		・このMODの使用についてですが、時期が時期ですから、この性質上あまり好ましくないSSが出来上がってしまう可能性があります（火災、火事等）。
		　もし使用する場合は上記の点を考慮した上でお願い致します。
		
		kemokemo/mimosa
	抜粋ここまで

	FIRETESTの内容は新しいtoonshader2に合わせて調整した上でshadertechniquetestに移植

	新規追加technique
	XYSCROLL_AlphaScroll
	XYSCROLL_AlphaScroll_Emissive
	XYSCROLL_KAZAN_AlphaScroll
	XYSCROLL_KAZAN_AlphaScroll_Emissive
	XYSCROLL_KAZAN2_AlphaScroll
	XYSCROLL_KAZAN2_AlphaScroll_Emissive
	XYSCROLL_JOUZAN_AlphaScroll
	XYSCROLL_JOUZAN_AlphaScroll_Emissive
	XYSCROLL_GENZAN_AlphaScroll
	XYSCROLL_GENZAN_AlphaScroll_Emissive
	XYSCROLL_NEGA_AlphaScroll
	XYSCROLL_NEGA_AlphaScroll_Emissive
	RGBとAを別々にスクロールできるtechnique
//-----------------------------------------------------------------------------------
//★20120401　ParaHUD系techniqueを再編集
//-----------------------------------------------------------------------------------
	カメラだけでなくボーン（ウェイト）の影響も受けないように
//-----------------------------------------------------------------------------------
//◎20120328　fxcでコンパイル
//-----------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------
//★20110806　エセ画面効果techniqueを追加
//-----------------------------------------------------------------------------------
	新規追加technique
	ParaHUD
	SCROLL_ParaHUD
	XYSCROLL_ParaHUD
	XREPEAT_ParaHUD
	BOUND_ParaHUD
	CIRCLE_ParaHUD
	BLANCO_ParaHUD
	EIGHT_ParaHUD
	WAVE_ParaHUD
	ROTATE_ParaHUD
	CLOCK_ParaHUD
	FLASH_ParaHUD
	SCROLL_FLASH_ParaHUD
	XYSCROLL_FLASH_ParaHUD
	XREPEAT_FLASH_ParaHUD
	BOUND_FLASH_ParaHUD
	CIRCLE_FLASH_ParaHUD
	BLANCO_FLASH_ParaHUD
	EIGHT_FLASH_ParaHUD
	WAVE_FLASH_ParaHUD
	ROTATE_FLASH_ParaHUD
	CLOCK_FLASH_ParaHUD
	SCROLL_FLASH_Emissive_ParaHUD
	XYSCROLL_FLASH_Emissive_ParaHUD
	XREPEAT_FLASH_Emissive_ParaHUD
	BOUND_FLASH_Emissive_ParaHUD
	CIRCLE_FLASH_Emissive_ParaHUD
	BLANCO_FLASH_Emissive_ParaHUD
	EIGHT_FLASH_Emissive_ParaHUD
	WAVE_FLASH_Emissive_ParaHUD
	ROTATE_FLASH_Emissive_ParaHUD
	CLOCK_FLASH_Emissive_ParaHUD
	SCROLL_FLASH_KAZAN_ParaHUD
	XYSCROLL_FLASH_KAZAN_ParaHUD
	XREPEAT_FLASH_KAZAN_ParaHUD
	BOUND_FLASH_KAZAN_ParaHUD
	CIRCLE_FLASH_KAZAN_ParaHUD
	BLANCO_FLASH_KAZAN_ParaHUD
	EIGHT_FLASH_KAZAN_ParaHUD
	WAVE_FLASH_KAZAN_ParaHUD
	ROTATE_FLASH_KAZAN_ParaHUD
	CLOCK_FLASH_KAZAN_ParaHUD
	XYSCROLL_FLASH_KAZAN_Emissive_ParaHUD
	XREPEAT_FLASH_KAZAN_Emissive_ParaHUD
	BOUND_FLASH_KAZAN_Emissive_ParaHUD
	CIRCLE_FLASH_KAZAN_Emissive_ParaHUD
	BLANCO_FLASH_KAZAN_Emissive_ParaHUD
	EIGHT_FLASH_KAZAN_Emissive_ParaHUD
	WAVE_FLASH_KAZAN_Emissive_ParaHUD
	ROTATE_FLASH_KAZAN_Emissive_ParaHUD
	CLOCK_FLASH_KAZAN_Emissive_ParaHUD
	SCROLL_FLASH_KAZAN2_ParaHUD
	XYSCROLL_FLASH_KAZAN2_ParaHUD
	XREPEAT_FLASH_KAZAN2_ParaHUD
	BOUND_FLASH_KAZAN2_ParaHUD
	CIRCLE_FLASH_KAZAN2_ParaHUD
	BLANCO_FLASH_KAZAN2_ParaHUD
	EIGHT_FLASH_KAZAN2_ParaHUD
	WAVE_FLASH_KAZAN2_ParaHUD
	ROTATE_FLASH_KAZAN2_ParaHUD
	CLOCK_FLASH_KAZAN2_ParaHUD
	SCROLL_FLASH_KAZAN2_Emissive_ParaHUD
	XYSCROLL_FLASH_KAZAN2_Emissive_ParaHUD
	XREPEAT_FLASH_KAZAN2_Emissive_ParaHUD
	BOUND_FLASH_KAZAN2_Emissive_ParaHUD
	CIRCLE_FLASH_KAZAN2_Emissive_ParaHUD
	BLANCO_FLASH_KAZAN2_Emissive_ParaHUD
	EIGHT_FLASH_KAZAN2_Emissive_ParaHUD
	WAVE_FLASH_KAZAN2_Emissive_ParaHUD
	ROTATE_FLASH_KAZAN2_Emissive_ParaHUD
	CLOCK_FLASH_KAZAN2_Emissive_ParaHUD

	カメラに左右されず、常に画面に対して定位置に表示し続けるtechnique
//-----------------------------------------------------------------------------------
//★20110805　ハイライトだけを表示するtechniqueを追加
//-----------------------------------------------------------------------------------
	新規追加technique
	OnlyHighLight
	OnlyHighLight_HLC
	OnlyHighLight_Emissive
	OnlyHighLight_HLC_Emissive

	_HLC要素と_Emissive要素については過去の更新を参考の事。
//-----------------------------------------------------------------------------------
//★20110515　主にFlach系techniqueとUVスクロール系techniqueの組み合わせを追加
//-----------------------------------------------------------------------------------
	新規追加technique
	ROTATE_FLASH
	SCROLL_FLASH
	XYSCROLL_FLASH
	XREPEAT_FLASH
	BOUND_FLASH
	CIRCLE_FLASH
	BLANCO_FLASH
	EIGHT_FLASH
	WAVE_FLASH
	ROTATE_FLASH
	CLOCK_FLASH
	SCROLL_FLASH_Emissive
	XYSCROLL_FLASH_Emissive
	XREPEAT_FLASH_Emissive
	BOUND_FLASH_Emissive
	CIRCLE_FLASH_Emissive
	BLANCO_FLASH_Emissive
	EIGHT_FLASH_Emissive
	WAVE_FLASH_Emissive
	ROTATE_FLASH_Emissive
	CLOCK_FLASH_Emissive
	SCROLL_FLASH_KAZAN
	XYSCROLL_FLASH_KAZAN
	XREPEAT_FLASH_KAZAN
	BOUND_FLASH_KAZAN
	CIRCLE_FLASH_KAZAN
	BLANCO_FLASH_KAZAN
	EIGHT_FLASH_KAZAN
	WAVE_FLASH_KAZAN
	ROTATE_FLASH_KAZAN
	CLOCK_FLASH_KAZAN
	XYSCROLL_FLASH_KAZAN_Emissive
	XREPEAT_FLASH_KAZAN_Emissive
	BOUND_FLASH_KAZAN_Emissive
	CIRCLE_FLASH_KAZAN_Emissive
	BLANCO_FLASH_KAZAN_Emissive
	EIGHT_FLASH_KAZAN_Emissive
	WAVE_FLASH_KAZAN_Emissive
	ROTATE_FLASH_KAZAN_Emissive
	CLOCK_FLASH_KAZAN_Emissive
	SCROLL_FLASH_KAZAN2
	XYSCROLL_FLASH_KAZAN2
	XREPEAT_FLASH_KAZAN2
	BOUND_FLASH_KAZAN2
	CIRCLE_FLASH_KAZAN2
	BLANCO_FLASH_KAZAN2
	EIGHT_FLASH_KAZAN2
	WAVE_FLASH_KAZAN2
	ROTATE_FLASH_KAZAN2
	CLOCK_FLASH_KAZAN2
	SCROLL_FLASH_KAZAN2_Emissive
	XYSCROLL_FLASH_KAZAN2_Emissive
	XREPEAT_FLASH_KAZAN2_Emissive
	BOUND_FLASH_KAZAN2_Emissive
	CIRCLE_FLASH_KAZAN2_Emissive
	BLANCO_FLASH_KAZAN2_Emissive
	EIGHT_FLASH_KAZAN2_Emissive
	WAVE_FLASH_KAZAN2_Emissive
	ROTATE_FLASH_KAZAN2_Emissive
	CLOCK_FLASH_KAZAN2_Emissive

//-----------------------------------------------------------------------------------
//★20110119　_eyedotn系techniqueを追加
//-----------------------------------------------------------------------------------
	新規追加technique
	ShadowOn_eyedotn
	AllAmb_ShadowOn_eyedotn

	カメラベクトルに対して垂直なほど面のαをどうこうする誰得technique
	網タイツの好きな人には良いらしい？
//-----------------------------------------------------------------------------------
//★20101021　ShadingFur_KAZAN系をちょっと調整
//-----------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------
//★20101019　新テクニックを追加…fur_kazan系
//　　　　　　毛の描画が加算
//-----------------------------------------------------------------------------------
	新規追加technique
	ShadingFur_KAZAN_Emissive
	ShadingFur_KAZAN2_Emissive

//-----------------------------------------------------------------------------------
//★20100612　XPC00417よりパース変換用行列及び対応したVertexShader部分を移植しました
//　　　　　　見た目上、描画距離が伸びました
//-----------------------------------------------------------------------------------
	この変更に当たって、
	パース変換の作者様
	それを使って描画距離を変更する事を思いついた方
	その仕組みについてアドバイスを頂いたnomeu氏
	に、心から感謝を

	パース変換用行列の仕組みの理解にはhttp://marupeke296.com/DXG_No55_WhatIsW.htmlを参考にさせて頂きました
	著者のIKD氏と、
	このＨＰを紹介してくれたnomeu氏に、
	重ねて感謝を
	『ミ』

//-----------------------------------------------------------------------------------
//★20100608　新テクニックを追加…Emissive系
//　　　　　　環境光に左右されず、自己照明を得るtechniqueを追加しました
//-----------------------------------------------------------------------------------
	新規追加technique
	#普通のシェーディング
	AllAmb_ShadowOff_InkOff_Emissive
	AllAmb_ShadowOff_InkOff_BHL_Emissve
	AllAmb_ShadowOff_InkOff_Front_Emissive
	AllAmb_ShadowOff_InkOff_BHL_Front_Emissve
	NAT_AllAmb_ShadowOff_InkOff_Emissive
	NAT_AllAmb_ShadowOff_InkOff_BHL
	NAT_AllAmb_ShadowOff_InkOff_Front_Emissive
	NAT_AllAmb_ShadowOff_InkOff_BHL_Front_Emissve
	#KAZAN系
	KAZAN_Emissive
	KAZAN_FRONT_Emissive
	SCROLL_KAZAN_Emissive
	XYSCROLL_KAZAN_Emissive
	XREPEAT_KAZAN_Emissive
	BOUND_KAZAN_Emissive
	CIRCLE_KAZAN_Emissive
	BLANCO_KAZAN_Emissive
	EIGHT_KAZAN_Emissive
	WAVE_KAZAN_Emissive
	ROTATE_KAZAN_Emissive
	CLOCK_KAZAN_Emissive
	FLASH_KAZAN_Emissives
	WASHOUT_KAZAN_Emissive
	WASHOUT_KAZAN_FRONT_Emissive
	WASHOUT_KAZAN_FLASH_Emissive
	#KAZAN2系
	KAZAN2_Emissive
	KAZAN2_FRONT_Emissive
	SCROLL_KAZAN2_Emissive
	XYSCROLL_KAZAN2_Emissive
	XREPEAT_KAZAN2_Emissive
	BOUND_KAZAN2_Emissive
	CIRCLE_KAZAN2_Emissive
	BLANCO_KAZAN2_Emissive
	EIGHT_KAZAN2_Emissive
	WAVE_KAZAN2_Emissive
	ROTATE_KAZAN2_Emissive
	CLOCK_KAZAN2_Emissive
	FLASH_KAZAN2_Emissive
	WASHOUT_KAZAN2_Emissive
	WASHOUT_KAZAN2_FRONT_Emissive
	WASHOUT_KAZAN2_FLASH_Emissive

	シェーダー設定ファイルに以下のパラメータを記述することにより、自己照明の色や透明度を調整することができます。
	float4 Emissive = [ r.r, g.g, b.b, a.a ]			//自己照明調整パラメータ	※デフォルト値[ 0.0, 0.0, 0.0, 0.0 ]

	emissiveの原案はnomeu氏が行って下さいました

//-----------------------------------------------------------------------------------
//★20100607　新テクニックを追加…KAZAN2系
//　　　　　　色が飽和し難い加算合成を追加しました
//-----------------------------------------------------------------------------------
	新規追加technique
	KAZAN2
	KAZAN2_FRONT
	SCROLL_KAZAN2
	XYSCROLL_KAZAN2
	XREPEAT_KAZAN2
	BOUND_KAZAN2
	CIRCLE_KAZAN2
	BLANCO_KAZAN2
	EIGHT_KAZAN2
	WAVE_KAZAN2
	ROTATE_KAZAN2
	CLOCK_KAZAN2
	FLASH_KAZAN2
	WASHOUT_KAZAN2
	WASHOUT_KAZAN2_FRONT
	WASHOUT_KAZAN2_FLASH

	シェーダー設定ファイルに以下のパラメータを記述することにより、全体の色や透明度を調整することができます。
	float4 KAZAN2COLOR = [ r.r, g.g, b.b, a.a ]			//KAZAN2調整パラメータ	※デフォルト値[ 1.0, 1.0, 1.0, 1.0 ]

	KAZAN2の作成にあたって、http://d.hatena.ne.jp/kataho/20080111/p1を参考にさせて頂きました
	kataho氏に感謝いたします
	『ミ』

//-----------------------------------------------------------------------------------
//★20100320　新テクニックを追加…Tex2Alpha
//　　　　　　テクスチャの明度からアルファチャンネルを作成するtechniqueを追加しました
//-----------------------------------------------------------------------------------
	背景の影付けを補佐するらしいtechnique
	Tex2Alpha

	colortextureのrgbの明度から、描画する不透明度を決定します
	明るければ透明に、暗ければ不透明に描画します

　　シェーダー設定ファイルに以下のパラメータを記述することにより、描画される部分の色を指定することができます
	float4 Tex2AlphaCol = [ r.r, g.g, b.b, a.a ]			//色指定	※デフォルト値[ 0, 0, 0, 1 ]

	
//-----------------------------------------------------------------------------------
//★20100305　NAT系techniqueを追加しました
//　　　　　　テクスチャのアルファチャンネルを全て描画するtechniqueの種類が増えました
//-----------------------------------------------------------------------------------
	新規NAT系technique
	NAT_AllAmb_ShadowOff
	NAT_AllAmb_ShadowOn
	NAT_AllAmb_ShadowOff_InkOff
	NAT_AllAmb_ShadowOn_InkOff
	NAT_ShadowOff_BL
	NAT_ShadowOn_BL
	NAT_ShadowOff_InkOff_BL
	NAT_ShadowOn_InkOff_BL
	NAT_AllAmb_ShadowOff_BL
	NAT_AllAmb_ShadowOn_BL
	NAT_AllAmb_ShadowOff_InkOff_BL
	NAT_AllAmb_ShadowOn_InkOff_BL
	NAT_AllAmb_ShadowOff_BHL
	NAT_AllAmb_ShadowOff_InkOff_BHL
	NAT_AllAmb_ShadowOff_HLC
	NAT_AllAmb_ShadowOn_HLC
	NAT_AllAmb_ShadowOff_InkOff_HLC
	NAT_AllAmb_ShadowOn_InkOff_HLC
	NAT_AllAmb_ShadowOff_BHL_HLC
	NAT_AllAmb_ShadowOff_InkOff_BHL_HLC
	NAT_ShadowOff_Front
	NAT_ShadowOn_Front
	NAT_ShadowOff_InkOff_Front
	NAT_ShadowOn_InkOff_Front
	NAT_AllAmb_ShadowOff_Front
	NAT_AllAmb_ShadowOn_Front
	NAT_AllAmb_ShadowOff_InkOff_Front
	NAT_AllAmb_ShadowOn_InkOff_Front
	NAT_AllAmb_ShadowOff_BHL_Front
	NAT_AllAmb_ShadowOff_InkOff_BHL_Front
	NAT_AllAmb_ShadowOff_HLC_Front
	NAT_AllAmb_ShadowOn_HLC_Front
	NAT_AllAmb_ShadowOff_InkOff_HLC_Front
	NAT_AllAmb_ShadowOn_InkOff_HLC_Front
	NAT_AllAmb_ShadowOff_BHL_HLC_Front
	NAT_AllAmb_ShadowOff_InkOff_BHL_HLC_Front

	デフォルトのNAT系technique
	NAT_ShadowOff_InkOff
	NAT_ShadowOff
	NAT_ShadowOn

//-----------------------------------------------------------------------------------
//★20100122　ハイライトがカメラベクトルに合わせて動くシェーダーのプログラム変更部分を一部見直し
//-----------------------------------------------------------------------------------
	HongFireにあったと言われるtoonshaderより、ハイライトがカメラベクトルに合わせて動く為の
	プログラム変更部分を統合した内容を一部見直しました

//-----------------------------------------------------------------------------------
//★20100120　新テクニックを追加…_Frontシリーズ
//　　　　　　常に前面、を指定できるtechniqueが増えました
//-----------------------------------------------------------------------------------
	常に前面に表示、追加technique
	ShadowOff_Front
	ShadowOn_Front
	ShadowOff_InkOff_Front
	ShadowOn_InkOff_Front
	AllAmb_ShadowOff_Front
	AllAmb_ShadowOn_Front
	AllAmb_ShadowOff_InkOff_Front
	AllAmb_ShadowOn_InkOff_Front
	AllAmb_ShadowOff_BHL_Front
	AllAmb_ShadowOff_InkOff_BHL_Front
	AllAmb_ShadowOff_HLC_Front
	AllAmb_ShadowOn_HLC_Front
	AllAmb_ShadowOff_InkOff_HLC_Front
	AllAmb_ShadowOn_InkOff_HLC_Front
	AllAmb_ShadowOff_BHL_HLC_Front
	AllAmb_ShadowOff_InkOff_BHL_HLC_Front

	オブジェクトを常に前面に描画します

//-----------------------------------------------------------------------------------
//★20091030　新テクニックを追加…_HLCシリーズ
//　　　　　　ハイライトの色を任意に変更することができるtechniqueを追加しました
//-----------------------------------------------------------------------------------
	ハイライト色変更用technique
	AllAmb_ShadowOff_HLC
	AllAmb_ShadowOn_HLC
	AllAmb_ShadowOff_InkOff_HLC
	AllAmb_ShadowOn_InkOff_HLC
	AllAmb_ShadowOff_BHL_HLC
	AllAmb_ShadowOff_InkOff_BHL_HLC

	_BHLについては逆光ハイライトを参照の事
　　シェーダー設定ファイルに以下のパラメータを記述することにより、ハイライトの色を任意の色に変更することができます
	float4 HighLightColor = [ r.r, g.g, b.b a.a ]			//ハイライト色指定	※デフォルト値[ 1, 1, 1, 1 ]

　　[ r.r, g.g, b.b a.a ] はそれぞれ [ 赤, 緑, 青, 不透明度 ] となっています
　　最小値=[0.0] 最大値=[1.0]

//-----------------------------------------------------------------------------------
//★20091030　ハイライトがカメラベクトルに合わせて動くシェーダーを統合
//-----------------------------------------------------------------------------------
	HongFireにあったと言われるtoonshaderより、ハイライトがカメラベクトルに合わせて動く為の
	プログラム変更部分を統合しました

//-----------------------------------------------------------------------------------
//★20090824　新テクニックを追加…WASHOUTシリーズ
//　　　　　　逆光ハイライトをベースにした輪郭をぼかす表現用のtechniqueを追加しました
//-----------------------------------------------------------------------------------
	輪郭ぼかし表現用technique

	WASHOUT
	WASHOUT_KAZAN
	WASHOUT_JOUZAN
	WASHOUT_GENZAN
	WASHOUT_NEGA
	WASHOUT_KAZAN_FRONT
	WASHOUT_JOUZAN_FRONT
	WASHOUT_GENZAN_FRONT
	WASHOUT_NEGA_FRONT

	輪郭のぼやけたオブジェクトを作ることが出来ます

	_KAZAN
	_JOUZAN
	_GENZAN
	_NEGA
	の４項目は、透過した時の描画の変更です

	_FRONT
	は常に最前面に表示する変更です

//-----------------------------------------------------------------------------------
//・20090622　シェーダー「toonshader2_20090522_add_fur+kazan」を統合
//　　　　　　「XPC00855 新テク20090522版_add_fur+kazan」 のプログラム変更部分をtoonshader2ヘ統合しました
//-----------------------------------------------------------------------------------
　　以下元MOD 説明書.txt より抜粋
	>追加technique
	>ShadingFur_KAZAN・・・モデル表面の「毛」の描画が加算効果

　　以前より統合する予定となっておりましたが、こちらの勝手な都合で遅れてしまいました事を深くお詫びいたします。

//-----------------------------------------------------------------------------------
//・20090622　シェーダー「ｔhickness」を統合
//　　　　　　「XPC00458 thickness ver 0.0.1 スケールに影響されない輪郭線」 のプログラム変更部分をtoonshader2ヘ統合しました
//　　　　　　※20090522版 ShadingFurへ適用済み
//-----------------------------------------------------------------------------------
　　以下元MOD readme.txt より抜粋
>これはなに
>輪郭線の太さを補正します。

>tmo でスケール（拡大縮小）を行っている場合、
>標準shaderでは輪郭線の太さがスケールに比例してしまいます。
>計算式を変更することでこの問題を解決します。

>概要
>カス子shaderでは、モデルを描く前に、法線方向にthickness値だけ太くした頂点位置を使い、
>黒く塗っておくことで輪郭線を実現しています。

>標準shaderは、太くした後にスキン変形を行うため、線の太さがスケールに比例します。
>この改良shaderでは、頂点位置と同時に法線もスキン変形し、変形後の法線方向に太くします。
>こうすることで、輪郭線の太さはスケールに影響されません。

　　作者様より統合の了承を得、20090522UP時に統合版として公開しておりました。
　　統合についてのコメントが遅れてしまいました事を深くお詫び申し上げます。

//-----------------------------------------------------------------------------------
//・20090620　新テクニックを追加…AllAmb_ShadowOff_BHL,AllAmb_ShadowOff_InkOff_BHL
//　　　　　　逆光によるハイライト表現用のtechniqueを追加しました
//-----------------------------------------------------------------------------------
　　逆光ハイライト表現用technique

　　AllAmb_ShadowOff_BHL
　　AllAmb_ShadowOff_InkOff_BHL

　　詳細は別途解説書を用意する予定です。もうしばらくお待ち下さい。

//-----------------------------------------------------------------------------------
//・20090522　新テクニックを追加…ShadingFurSkin,ShadingFur
//　　　　　　「毛」表現用のtechniqueを追加しました
//-----------------------------------------------------------------------------------
　　ShadingFurSkin,ShadingFur・・・モデルの表面に「毛」を生やす事が出来ます。

　　詳細は同梱のShadingFur_manual.zipをご覧下さい。

//-----------------------------------------------------------------------------------
//・20090408　新テクニックを追加…CLOCK,CLOCK_KAZAN　※雪原背景tbn依存
//　　　　　　時計針回転用techniqueを追加しました
//-----------------------------------------------------------------------------------
　　CLOCK,CLOCK_KAZAN・・・ROTATEの回転角度を段階的に変更

　　秒針用、長針用、短針用に回転時間を設定する事が出来ます。シェーダー設定ファイルに以下の行を追加して下さい

	float UVClock = [***]		//回転時間（1,60,720）

		※UVClock = [1]		…秒針用、６０秒で１回転（１秒毎に６度）
		※UVClock = [60]	…長針用、１時間で１回転（１分毎に６度）
		※UVClock = [720]	…短針用、１２時間で１回転（１２分毎に６度）

//-----------------------------------------------------------------------------------
//・20090405　新テクニックを追加…FLASH,FLASH_KAZAN　※雪原背景tbn依存
//　　　　　　テクスチャ点滅用のtechniqueを追加しました
//-----------------------------------------------------------------------------------
　　FLASH,FLASH_KAZAN・・・テクスチャの透明度をリアルタイムに書き換えて点滅している様に見せます（２パターン）

　　点滅速度、点滅効果を変更する場合は、シェーダー設定ファイルに以下の２行を追加して下さい

	float UVFlash = [****]		//点滅速度（0～5000）1.0毎に変化
	float FlashFunc = [*]		//点滅効果（0,1）

		※FlashFunc = [0] …テクスチャの透明度を、0,1,0,1,0,1……と瞬間的に切り替えます
		※FlashFunc = [1] …テクスチャの透明度を、1から0、0から1（繰り返し）とスムーズに変化させます

　　※点滅時の最大不透明度は、テクスチャ自身のアルファ値になります

//-----------------------------------------------------------------------------------
//・20090313　新テクニックを追加…ROTATE,ROTATE_KAZAN　※雪原背景tbn依存
//　　　　　　反射光ハイライト表現用のtechniqueを修正…一部MODでパーツが消えてしまう症状を修正しました
//-----------------------------------------------------------------------------------
　　ROTATE,ROTATE_KAZAN・・・ＵＶテクスチャ座標（0.0,0.0）を中心にテクスチャが回転、拡大縮小

　　回転速度、拡大縮小速度を変更する場合は、シェーダー設定ファイルに以下の行を追加して下さい

	float UVRotate = [****]		//回転速度（-5000～5000）1.0毎に変化
	float UVScale = [****]		//拡大縮小速度（-5000～5000）1.0毎に変化

　　※数値を[0]に設定すると停止します
　　　回転だけさせたい時は UVScale = [0] に、拡大縮小だけさせたい時は UVRotate = [0] に設定

//-----------------------------------------------------------------------------------
//・20090307　新テクニックを追加
//　　　　　　反射光ハイライト表現用のtechniqueを追加しました
//-----------------------------------------------------------------------------------
　　反射光ハイライト表現用technique

　　ShadowOff_BL
　　ShadowOn_BL
　　ShadowOff_InkOff_BL
　　ShadowOn_InkOff_BL
　　AllAmb_ShadowOff_BL
　　AllAmb_ShadowOn_BL
　　AllAmb_ShadowOff_InkOff_BL
　　AllAmb_ShadowOn_InkOff_BL

　　仕組みは、ライトのベクトルに対して正方向と逆方向から射す光２つを定義し、オブジェクトがその光を反射して
　　視点（カメラ位置）の方向へ光を飛ばす様に見える様にしています。
　　カメラの動きによって光る部分も変化します。
　　効果の程は同梱のヘビーセーブデータ"49010A86-8981-4EA8-A41DA2B92C7BF118.tdcgsav.png"にて確認して下さい。

　　更に、シェーダー設定ファイルに以下のパラメータを記述することにより、正方向、逆方向の光の色と強さを設定できるようにしました。

　　float4 FrontLight = [r.r, g.g, b.b a.a]　		//正方向ライト色　※デフォルト値[0.9, 0.9, 0.9, 0.2]
　　float FrontLightPower = [p.p]　			//正方向ライト強さ　※デフォルト値[0.1]
　　float4 BackLight = [r.r, g.g, b.b a.a]　		//逆方向ライト色　※デフォルト値[0.2, 0.4, 0.5, 0.6]
　　float BackLightPower = [p.p]　			//逆方向ライト強さ　※デフォルト値[0.4]

　　[r.r, g.g, b.b a.a] はそれぞれ [赤,緑,青,不透明度] となっています
　　　最小値=[0.0] 最大値=[1.0]
　　[p.p] は光の強さ
　　　最小値=[-10.0] 最大値=[10.0] ※0.01毎に指定
　　上記パラメータを記述していなかった場合は、デフォルト値が適用されます。

　　※フリルの様に”一部透過”している様なオブジェクトの場合、透過部分にもこの効果が出てしまう為、あまりお勧めできません。

//-----------------------------------------------------------------------------------
//・20090220　新テクニックを追加
//　　　　　　シェーディングテクスチャの適用範囲を変更したtechniqueを追加しました
//-----------------------------------------------------------------------------------
　　シェーディングテクスチャ適用範囲変更用technique

　　AllAmb_ShadowOff
　　AllAmb_ShadowOn
　　AllAmb_ShadowOff_InkOff
　　AllAmb_ShadowOn_InkOff

　　上記４つのtechniqueを使用する事で、モデルの全域に渡ってシェーディングを適用する事ができます
　　※float Ambient = [50]　に変更してください

//-----------------------------------------------------------------------------------
//・20090219　新テクニックを追加　※雪原背景tbn依存
//　　　　　　各techniqueのＸ，Ｙ軸へのスクロール速度、振り幅を個別に指定出来る様にしました（追加分のみ）
//-----------------------------------------------------------------------------------
　　XYSCROLL,XYSCROLL_KAZAN・・・Ｘ，Ｙ軸両方へのスクロール可
　　XREPEAT,XREPEAT_KAZAN・・・Ｘ軸に沿って往復
　　BOUND,BOUND_KAZAN・・・Ｘ軸方向にバウンドする様にスクロール
　　CIRCLE,CIRCLE_KAZAN・・・ＵＶ座標(0.5,0.5)を中心に回転　※1
　　BLANCO,BLANCO_KAZAN・・・振り子の様に往復
　　EIGHT,EIGHT_KAZAN・・・８の字状に往復
　　WAVE,WAVE_KAZAN・・・波状にスクロール

　　スクロール速度、振り幅を変更する場合は、シェーダー設定ファイルに以下の行を追加して下さい

float UVScrollX = [****]　Ｘ軸方向へのスクロール速度（-5000～5000）1.0毎に変化
float UVScrollY = [****]　Ｙ軸方向へのスクロール速度（-5000～5000）1.0毎に変化
float UVScrollAmpX = [**]　Ｘ軸方向への振り幅（-10～10）0.01毎に変化
float UVScrollAmpY = [**]　Ｙ軸方向への振り幅（-10～10）0.01毎に変化

　　※techniqueによっては数値を指定しても変化しない場合があります
　　                           UVScrollX  UVScrollY  UVScrollAmpX  UVScrollAmpY
　　XYSCROLL,XYSCROLL_KAZAN           ○         ○            ×            ×
　　XREPEAT,XREPEAT_KAZAN             ○         ×            ○            ×
　　BOUND,BOUND_KAZAN                 ○         ○            ×            ○
　　CIRCLE,CIRCLE_KAZAN               ○         ○            ○            ○
　　BLANCO,BLANCO_KAZAN               ○         ○            ○            ○
　　EIGHT,EIGHT_KAZAN                 ○         ○            ○            ○
　　WAVE,WAVE_KAZAN                   ○         ×            ○            ○

　　※20090213追加のSCROLL,SCROLL_KAZANについては仕様の変更はありません

　　※1…ＵＶ座標（２次元テクスチャ座標）はテクスチャ境界の左上が(0,0)、右下が(1,1)となります

//-----------------------------------------------------------------------------------
//・20090213　新テクニックを追加　SCROLL、SCROLL_KAZAN　※雪原背景tbn依存
//　　　　　　string technique = "SCROLL" string technique = "SCROLL_KAZAN" 使用時のUVスクロールの速度を
//　　　　　　シェーダー設定ファイル内で変更出来る様にしました
//-----------------------------------------------------------------------------------
　　スクロール速度を変更する場合は、シェーダー設定ファイルのtechniqueにSCROLLまたはSCROLL_KAZANを指定し、
　　更に以下の行を追加して下さい

float UVScroll = [****]

　　※[****]内は-5000～5000の間で指定
　　　0で停止、1以上でスクロール、0未満で逆方向にスクロール
　　　1はデフォルトの速度となります
　　※背景によってはスクロールしません。現在雪原風景でのみスクロールする事を確認しています

//-----------------------------------------------------------------------------------
//・20090212　string technique = "AURORA" 使用時のUVスクロールの速度を
//　シェーダー設定ファイル内で変更出来る様にしました　　　　　　　　　　　※20090213版にて廃止
//-----------------------------------------------------------------------------------
　　スクロール速度を変更する場合は、シェーダー設定ファイルのtechniqueにAURORAを指定し、
　　更に以下の行を追加して下さい

float UVScroll = [****]

　　※[****]内は0～5000の間で指定
　　　0で停止、1以上でスクロール
　　　1はデフォルトの速度となります
　　　シェーダー設定ファイル内にfloat UVScroll = [****]の記述がない場合はデフォルトの速度でスクロールします
　　※背景によってはスクロールしません。現在雪原風景でのみスクロールする事を確認しています

//-----------------------------------------------------------------------------------
//・20090211　新規technique追加
//-----------------------------------------------------------------------------------
　　JOUZAN・・・乗算効果
　　GENZAN・・・減算効果
　　NEGA・・・ネガ反転効果
　　KAZAN_FRONT・・・加算効果（常にどのオブジェクトよりも前面に描画）
　　JOUZAN_FRONT・・・乗算効果（常にどのオブジェクトよりも前面に描画）
　　GENZAN_FRONT・・・減算効果（常にどのオブジェクトよりも前面に描画）
　　NEGA_FRONT・・・ネガ反転効果（常にどのオブジェクトよりも前面に描画）

注意事項
・商用に用いる場合には、下記作者にご連絡下さい。
・内部データの２次使用、修正、改変等制限はありません。
・再配布可。ライセンスはクリエイティブ・コモンズ【表示】【継承】に準じます。

◆◆◆◆◆各改造ツールの作者様方に感謝致します◆◆◆◆◆

kemokemo
mailto:mimosa211@gmail.com
お知らせ：
kemokemoはMOD整理計画を応援、3Dカスタム少女職人ギルドに参加しています
http://www42.atwiki.jp/kasukomod/
また、２ちゃんねるIRC　＃３Ｄカスタム少女　に参加しています。改造に携わる職人の皆様が多数参加されています。
ご興味のある方は是非いらして下さい。
主要開発者：
ミさんの努力により非常にたくさんのエフェクトを搭載しました。ありがとうございます。
ミ（tdcg_racl@materialize-54.sakura.ne.jp）
職人ギルド主催：
http://3dcustom.ath.cx
こちらで職人ギルドを開設しております。カス子における技術的問題・法的問題を解決する為の機関です。
(株)テックアーツ社、(株)マイクロソフト社との法的交渉も行っております。クリエイティブ・コモンズの
連絡先はこちらとなります。
主催であるkonoaの連絡先は
mailto:3dcustomgirl@gmail.com
OpenPNE参加申し込みも同アドレスで行っております。
新テク（両面描画）：
両面描画など新しいテクニックを開発したぱるたさん、ありがとうございます。
ぱるた（http://www.pixiv.net/member.php?id=4458432）

★
★印の項目は、『ミ』が加筆修正している部分です
◎
◎印の項目は、『konoa』が加筆修正している部分です
