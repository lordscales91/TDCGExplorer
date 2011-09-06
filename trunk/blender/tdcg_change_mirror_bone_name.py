#!BPY
# -*- coding: utf-8 -*-
"""
Name: '3DCG Change mirror bone name'
Blender: 249
Group: 'Object'
Tooltip: '左右対称の頂点グループ名とボーン名を一時的に変換できます。'
"""

import Blender
from Blender import *

MIRROR_BONE = [ \
	["shiri_Right01", "shiri_01.R"],
	["shiri_Right02", "shiri_02.R"],
	["shiri_Right02_End", "shiri_02_End.R"],

	["shiri_Left01", "shiri_01.L"],
	["shiri_Left02", "shiri_02.L"],
	["shiri_Left02_End", "shiri_02_End.L"],

	["W_LeftHips_Dummy", "W_Hips_Dummy.L"],
	["W_LeftUpLeg", "W_UpLeg.L"],
	["W_LeftUpLegRoll", "W_UpLegRoll.L"],
	["W_LeftLeg", "W_Leg.L"],
	["W_LeftLegRoll", "W_LegRoll.L"],
	["W_LeftFoot", "W_Foot.L"],
	["W_LeftToeBase", "W_ToeBase.L"],
	["W_LeftToes_End", "W_Toes_End.L"],

	["W_RightHips_Dummy", "W_Hips_Dummy.R"],
	["W_RightUpLeg", "W_UpLeg.R"],
	["W_RightUpLegRoll", "W_UpLegRoll.R"],
	["W_RightLeg", "W_Leg.R"],
	["W_RightLegRoll", "W_LegRoll.R"],
	["W_RightFoot", "W_Foot.R"],
	["W_RightToeBase", "W_ToeBase.R"],
	["W_RightToes_End", "W_Toes_End.R"],

	["W_RightShoulder_Dummy", "W_Shoulder_Dummy.R"],
	["W_RightShoulder", "W_Shoulder.R"],
	["W_RightArm_Dummy", "W_Arm_Dummy.R"],
	["W_RightArm", "W_Arm.R"],
	["W_RightArmRoll", "W_ArmRoll.R"],
	["W_RightForeArm", "W_ForeArm.R"],
	["W_RightForeArmRoll", "W_ForeArmRoll.R"],
	["W_RightHand", "W_Hand.R"],
	["W_RightHandPinky1", "W_HandPinky1.R"],
	["W_RightHandPinky2", "W_HandPinky2.R"],
	["W_RightHandPinky3", "W_HandPinky3.R"],
	["W_RightHandPinky4", "W_HandPinky4.R"],
	["W_RightHandRing1", "W_HandRing1.R"],
	["W_RightHandRing2", "W_HandRing2.R"],
	["W_RightHandRing3", "W_HandRing3.R"],
	["W_RightHandRing4", "W_HandRing4.R"],
	["W_RightHandMiddle1", "W_HandMiddle1.R"],
	["W_RightHandMiddle2", "W_HandMiddle2.R"],
	["W_RightHandMiddle3", "W_HandMiddle3.R"],
	["W_RightHandMiddle4", "W_HandMiddle4.R"],
	["W_RightHandIndex1", "W_HandIndex1.R"],
	["W_RightHandIndex2", "W_HandIndex2.R"],
	["W_RightHandIndex3", "W_HandIndex3.R"],
	["W_RightHandIndex4", "W_HandIndex4.R"],
	["W_RightHandThumb1", "W_HandThumb1.R"],
	["W_RightHandThumb2", "W_HandThumb2.R"],
	["W_RightHandThumb3", "W_HandThumb3.R"],
	["W_RightHandThumb4", "W_HandThumb4.R"],

	["W_LeftShoulder_Dummy", "W_Shoulder_Dummy.L"],
	["W_LeftShoulder", "W_Shoulder.L"],
	["W_LeftArm_Dummy", "W_Arm_Dummy.L"],
	["W_LeftArm", "W_Arm.L"],
	["W_LeftArmRoll", "W_ArmRoll.L"],
	["W_LeftForeArm", "W_ForeArm.L"],
	["W_LeftForeArmRoll", "W_ForeArmRoll.L"],
	["W_LeftHand", "W_Hand.L"],
	["W_LeftHandPinky1", "W_HandPinky1.L"],
	["W_LeftHandPinky2", "W_HandPinky2.L"],
	["W_LeftHandPinky3", "W_HandPinky3.L"],
	["W_LeftHandPinky4", "W_HandPinky4.L"],
	["W_LeftHandRing1", "W_HandRing1.L"],
	["W_LeftHandRing2", "W_HandRing2.L"],
	["W_LeftHandRing3", "W_HandRing3.L"],
	["W_LeftHandRing4", "W_HandRing4.L"],
	["W_LeftHandMiddle1", "W_HandMiddle1.L"],
	["W_LeftHandMiddle2", "W_HandMiddle2.L"],
	["W_LeftHandMiddle3", "W_HandMiddle3.L"],
	["W_LeftHandMiddle4", "W_HandMiddle4.L"],
	["W_LeftHandIndex1", "W_HandIndex1.L"],
	["W_LeftHandIndex2", "W_HandIndex2.L"],
	["W_LeftHandIndex3", "W_HandIndex3.L"],
	["W_LeftHandIndex4", "W_HandIndex4.L"],
	["W_LeftHandThumb1", "W_HandThumb1.L"],
	["W_LeftHandThumb2", "W_HandThumb2.L"],
	["W_LeftHandThumb3", "W_HandThumb3.L"],
	["W_LeftHandThumb4", "W_HandThumb4.L"],

	["sitakuti_l_1", "sitakuti_1.R"],
	["sitakuti_l_1_end", "sitakuti_1_end.R"],

	["sitakuti_r_1", "sitakuti_1.L"],
	["sitakuti_r_1_end", "sitakuti_1_end.L"],

	["kutiyoko_r", "kutiyoko.L"],
	["kutiyoko_r_end", "kutiyoko_end.L"],

	["kutiyoko_l", "kutiyoko.R"],
	["kutiyoko_l_end", "kutiyoko_end.R"],

	["uekuti_l_1", "uekuti_1.R"],
	["uekuti_l_1_end", "uekuti_1_end.R"],

	["uekuti_r_1", "uekuti_1.L"],
	["uekuti_r_1_end", "uekuti_1_end.L"],

	["eyeline_sita_R", "eyeline_sita.R"],
	["eyeline_sita_R_end", "eyeline_sita_end.R"],

	["eyeline_sita_L", "eyeline_sita.L"],
	["eyeline_sita_L_end", "eyeline_sita_end.L"],

	["L_eyeline_oya_L", "eyeline_oya.R"],
	["L_eyeline_2_L", "eyeline_2_L.R"],
	["L_eyeline_3_L", "eyeline_3_L.R"],
	["L_eyeline_3_L_end", "eyeline_3_L_end.R"],
	["L_eyeline_2_R", "eyeline_2_R.R"],
	["L_eyeline_3_R", "eyeline_3_R.R"],
	["L_eyeline_3_R_end", "eyeline_3_R_end.R"],

	["R_eyeline_oya_R", "eyeline_oya.L"],
	["R_eyeline_2_L", "eyeline_2_L.L"],
	["R_eyeline_3_L", "eyeline_3_L.L"],
	["R_eyeline_3_L_end", "eyeline_3_L_end.L"],
	["R_eyeline_2_R", "eyeline_2_R.L"],
	["R_eyeline_3_R", "eyeline_3_R.L"],
	["R_eyeline_3_R_end", "eyeline_3_R_end.L"],

	["mayu_1_R", "mayu_1.L"],
	["mayu_2_R", "mayu_2.L"],
	["mayu_3_R", "mayu_3.L"],
	["mayu_3_R_end", "mayu_3_end.L"],

	["mayu_1_L", "mayu_1.R"],
	["mayu_2_L", "mayu_2.R"],
	["mayu_3_L", "mayu_3.R"],
	["mayu_3_L_end", "mayu_3_end.R"],

	["Me_Left_Futi", "Me_Futi.L"],
	["Me_Left", "Me.L"],
	["Me_Left_End", "Me_End.L"],
	["Me_Left_HighLight_A", "Me_HighLight_A.L"],
	["Me_Left_HighLight_A_END", "Me_HighLight_A_END.L"],
	["Me_Left_HighLight_B", "Me_HighLight_B.L"],
	["Me_Left_HighLight_B_END", "Me_HighLight_B_END.L"],

	["Me_Right_Futi", "Me_Futi.R"],
	["Me_Right", "Me.R"],
	["Me_Right_End", "Me_End.R"],
	["Me_Left_RighLight_A", "Me_HighLight_A.R"],
	["Me_Left_RighLight_A_END", "Me_HighLight_A_END.R"],
	["Me_Left_RighLight_B", "Me_HighLight_B.R"],
	["Me_Left_RighLight_B_END", "Me_HighLight_B_END.R"],

	["kami_Front_L1", "kami_Front_1.L"],
	["kami_Front_L2", "kami_Front_2.L"],
	["kami_Front_L3", "kami_Front_3.L"],
	["kami_Front_L4", "kami_Front_4.L"],
	["kami_Front_L4_End", "kami_Front_4_End.L"],

	["kami_Front_R1", "kami_Front_1.R"],
	["kami_Front_R2", "kami_Front_2.R"],
	["kami_Front_R3", "kami_Front_3.R"],
	["kami_Front_R4", "kami_Front_4.R"],
	["kami_Front_R4_End", "kami_Front_4_End.R"],

	["kami_Back_L1", "kami_Back_1.L"],
	["kami_Back_L2", "kami_Back_2.L"],
	["kami_Back_L3", "kami_Back_3.L"],
	["kami_Back_L4", "kami_Back_4.L"],
	["kami_Back_L4_End", "kami_Back_4_End.L"],

	["kami_Back_R1", "kami_Back_1.R"],
	["kami_Back_R2", "kami_Back_2.R"],
	["kami_Back_R3", "kami_Back_3.R"],
	["kami_Back_R4", "kami_Back_4.R"],
	["kami_Back_R4_End", "kami_Back_4_End.R"],

	["kami_Front_Mid1_L", "kami_Front_Mid1.L"],
	["kami_Front_Mid2_L", "kami_Front_Mid2.L"],
	["kami_Front_Mid3_L", "kami_Front_Mid3.L"],
	["kami_Front_Mid3_End_L", "kami_Front_Mid3_End.L"],

	["kami_Front_Mid1_R", "kami_Front_Mid1.R"],
	["kami_Front_Mid2_R", "kami_Front_Mid2.R"],
	["kami_Front_Mid3_R", "kami_Front_Mid3.R"],
	["kami_Front_Mid3_End_R", "kami_Front_Mid3_End.R"],

	["Chichi_Right1", "Chichi_1.R"],
	["Chichi_Right2", "Chichi_2.R"],
	["Chichi_Right3", "Chichi_3.R"],
	["Chichi_Right4", "Chichi_4.R"],
	["Chichi_Right5", "Chichi_5.R"],
	["Chichi_Right5_end", "Chichi_5_end.R"],

	["Chichi_Left1", "Chichi_1.L"],
	["Chichi_Left2", "Chichi_2.L"],
	["Chichi_Left3", "Chichi_3.L"],
	["Chichi_Left4", "Chichi_4.L"],
	["Chichi_Left5", "Chichi_5.L"],
	["Chichi_Left5_End", "Chichi_5_end.L"],

	["skirt_RightB01", "skirt_B01.R"],
	["skirt_RightB02", "skirt_B02.R"],
	["skirt_RightB03", "skirt_B03.R"],
	["skirt_RightB03_end", "skirt_B03_end.R"],

	["skirt_LeftB01", "skirt_B01.L"],
	["skirt_LeftB02", "skirt_B02.L"],
	["skirt_LeftB03", "skirt_B03.L"],
	["skirt_LeftB03_end", "skirt_B03_end.L"],

	["skirt_RightS01", "skirt_S01.R"],
	["skirt_RightS02", "skirt_S02.R"],
	["skirt_RightS03", "skirt_S03.R"],
	["skirt_RightS03_end", "skirt_S03_end.R"],

	["skirt_LeftS01", "skirt_S01.L"],
	["skirt_LeftS02", "skirt_S02.L"],
	["skirt_LeftS03", "skirt_S03.L"],
	["skirt_LeftS03_end", "skirt_S03_end.L"],

	["skirt_RightF01", "skirt_F01.R"],
	["skirt_RightF02", "skirt_F02.R"],
	["skirt_RightF03", "skirt_F03.R"],
	["skirt_RightF03_end", "skirt_F03_end.R"],

	["skirt_LeftF01", "skirt_F01.L"],
	["skirt_LeftF02", "skirt_F02.L"],
	["skirt_LeftF03", "skirt_F03.L"],
	["skirt_LeftF03_end", "skirt_F03_end.L"],

	["W_RightManko", "W_Manko.R"],
	["W_RightManko_End", "W_Manko_End.R"],

	["W_LeftManko", "W_Manko.L"],
	["W_LeftManko_End", "W_Manko_End.L"] ]

def main():
	
	# 選択したオブジェクトを取得
	selectOb = Object.GetSelected()
	# メッシュとアーマチュア以外削除
	for i in reversed(range(len(selectOb))):
		if selectOb[i].type != "Mesh" and selectOb[i].type != "Armature":
			del selectOb[i]
	# リストが空白の場合
	if len(selectOb) == 0:
		Draw.PupMenu("メッシュかアーマチュアを一つ以上選択して下さい。%t")
		return
	
	# エンコードかデコードか選択
	mode = Draw.PupMenu("ボーン名変換モードを選択して下さい。%t|Blender形式に変換|3DCG形式に変換")
	# 無選択の場合
	if mode == -1:
		Draw.PupMenu("キャンセルしました%t")
		return
	
	# 全てのオブジェクトをループ
	for ob in selectOb:
		
		# ■メッシュの場合
		if ob.type == "Mesh":
			# メッシュオブジェクト取得
			me = ob.getData(False, True)
			# 変換雛形ループ
			for change in MIRROR_BONE:
				# デコードの場合
				if mode == 2:
					change = [change[1], change[0]]
				# 頂点グループがあるか
				if change[0] in me.getVertGroupNames():
					me.renameVertGroup(change[0], change[1])
		
		# ■アーマチュアの場合
		elif ob.type == "Armature":
			# アーマチュアオブジェクト取得
			arm = ob.getData(False, False)
			pose = ob.getPose()
			# エディットモードに
			arm.makeEditable()
			# 変換雛形ループ
			poses = []
			for change in MIRROR_BONE:
				# デコードの場合
				if mode == 2:
					change = [change[1], change[0]]
				# ボーンがあるか
				if change[0] in arm.bones.keys():
					poses.append(( change[1], pose.bones[change[0]].size.copy(), pose.bones[change[0]].quat.copy(), pose.bones[change[0]].loc.copy() ))
					arm.bones[change[0]].name = change[1]
			# 情報更新
			arm.update()
			# ポーズを元通りに
			pose = ob.getPose()
			for name, size, quat, loc in poses:
				pose.bones[name].size = size
				pose.bones[name].quat = quat
				pose.bones[name].loc = loc
			pose.update()

if __name__ =="__main__":
	main()
