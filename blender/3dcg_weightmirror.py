#!BPY
"""
Name: '3DCG Weight Mirror'
Blender: 249
Group: 'Object'
Tooltip: '3DCG Weight Mirror'
"""

import Blender
from Blender import *
from Blender.Mathutils import *

center_bones= (\
	"W_Hips",
	"Mata",
	"Anal",
	"Anal_End",
	"W_Spine_Dummy",
	"W_Spine1",
	"W_Spine2",
	"W_Spine3",
	"W_Neck",
	"Head",
	"face_oya",
	"hana",
	"hana_END",
	"Kami_Oya",
	"kami_Back_Mid1",
	"kami_Back_Mid2",
	"kami_Back_Mid3",
	"kami_Back_Mid4",
	"kami_Back_Mid4",
	"sita_01",
	"sita_02",
	"sita_03",
	"sita_03_end",
	"sitakuti_oya",
	"Ha_Down",
	"Ha_Down_END",
	"uekuti_oya",
	"Ha_UP",
	"Ha_UP_END")

mirror_bones= (\
	("W_LeftManko", "W_RightManko"),
	("W_LeftManko_End", "W_RightManko_End"),
	("shiri_Left01", "shiri_Right01"),
	("shiri_Left02", "shiri_Right02"),
	("shiri_Left02_End", "shiri_Right02_End"),
	("W_LeftHips_Dummy", "W_RightHips_Dummy"),
	("W_LeftUpLeg", "W_RightUpLeg"),
	("W_LeftUpLegRoll", "W_RightUpLegRoll"),
	("W_LeftLeg", "W_RightLeg"),
	("W_LeftLegRoll", "W_RightLegRoll"),
	("W_LeftFoot", "W_RightFoot"),
	("W_LeftToeBase", "W_RightToeBase"),
	("W_LeftToes_End", "W_RightToes_End"),
	("skirt_LeftB01", "skirt_RightB01"),
	("skirt_LeftB02", "skirt_RightB02"),
	("skirt_LeftB03", "skirt_RightB03"),
	("skirt_LeftB03_end", "skirt_RightB03_end"),
	("skirt_LeftF01", "skirt_RightF01"),
	("skirt_LeftF02", "skirt_RightF02"),
	("skirt_LeftF03", "skirt_RightF03"),
	("skirt_LeftF03_end", "skirt_RightF03_end"),
	("skirt_LeftS01", "skirt_RightS01"),
	("skirt_LeftS02", "skirt_RightS02"),
	("skirt_LeftS03", "skirt_RightS03"),
	("skirt_LeftS03_end", "skirt_RightS03_end"),
	("Chichi_Left1", "Chichi_Right1"),
	("Chichi_Left2", "Chichi_Right2"),
	("Chichi_Left3", "Chichi_Right3"),
	("Chichi_Left4", "Chichi_Right4"),
	("Chichi_Left5", "Chichi_Right5"),
	("Chichi_Left5_End", "Chichi_Right5_end"),
	("W_LeftShoulder_Dummy", "W_RightShoulder_Dummy"),
	("W_LeftShoulder", "W_RightShoulder"),
	("W_LeftArm_Dummy", "W_RightArm_Dummy"),
	("W_LeftArm", "W_RightArm"),
	("W_LeftArmRoll", "W_RightArmRoll"),
	("W_LeftForeArm", "W_RightForeArm"),
	("W_LeftForeArmRoll", "W_RightForeArmRoll"),
	("W_LeftHand", "W_RightHand"),
	("W_LeftHandIndex1", "W_RightHandIndex1"),
	("W_LeftHandIndex2", "W_RightHandIndex2"),
	("W_LeftHandIndex3", "W_RightHandIndex3"),
	("W_LeftHandIndex4", "W_RightHandIndex4"),
	("W_LeftHandMiddle1", "W_RightHandMiddle1"),
	("W_LeftHandMiddle2", "W_RightHandMiddle2"),
	("W_LeftHandMiddle3", "W_RightHandMiddle3"),
	("W_LeftHandMiddle4", "W_RightHandMiddle4"),
	("W_LeftHandPinky1", "W_RightHandPinky1"),
	("W_LeftHandPinky2", "W_RightHandPinky2"),
	("W_LeftHandPinky3", "W_RightHandPinky3"),
	("W_LeftHandPinky4", "W_RightHandPinky4"),
	("W_LeftHandRing1", "W_RightHandRing1"),
	("W_LeftHandRing2", "W_RightHandRing2"),
	("W_LeftHandRing3", "W_RightHandRing3"),
	("W_LeftHandRing4", "W_RightHandRing4"),
	("W_LeftHandThumb1", "W_RightHandThumb1"),
	("W_LeftHandThumb2", "W_RightHandThumb2"),
	("W_LeftHandThumb3", "W_RightHandThumb3"),
	("W_LeftHandThumb4", "W_RightHandThumb4"),
	("eyeline_sita_R", "eyeline_sita_L"),
	("eyeline_sita_R_end", "eyeline_sita_L_end"),
	("kami_Back_L1", "kami_Back_R1"),
	("kami_Back_L2", "kami_Back_R2"),
	("kami_Back_L3", "kami_Back_R3"),
	("kami_Back_L4", "kami_Back_R4"),
	("kami_Back_L4_End", "kami_Back_R4_End"),
	("kami_Front_L1", "kami_Front_R1"),
	("kami_Front_L2", "kami_Front_R2"),
	("kami_Front_L3", "kami_Front_R3"),
	("kami_Front_L4", "kami_Front_R4"),
	("kami_Front_L4_End", "kami_Front_R4_End"),
	("kami_Front_Mid1_L", "kami_Front_Mid1_R"),
	("kami_Front_Mid2_L", "kami_Front_Mid2_R"),
	("kami_Front_Mid3_L", "kami_Front_Mid3_R"),
	("kami_Front_Mid3_End_L", "kami_Front_Mid3_End_R"),
	("kutiyoko_r", "kutiyoko_l"),
	("kutiyoko_r_end", "kutiyoko_l_end"),
	("R_eyeline_oya_R", "L_eyeline_oya_L"),
	("R_eyeline_2_L", "L_eyeline_2_L"),
	("R_eyeline_3_L", "L_eyeline_3_L"),
	("R_eyeline_3_L_end", "L_eyeline_3_L_end"),
	("R_eyeline_2_R", "L_eyeline_2_R"),
	("R_eyeline_3_R", "L_eyeline_3_R"),
	("R_eyeline_3_R_end", "L_eyeline_3_R_end"),
	("mayu_1_R", "mayu_1_L"),
	("mayu_2_R", "mayu_2_L"),
	("mayu_3_R", "mayu_3_L"),
	("mayu_3_R_end", "mayu_3_L_end"),
	("Me_Left_Futi", "Me_Right_Futi"),
	("Me_Left", "Me_Right"),
	("Me_Left_End", "Me_Right_End"),
	("Me_Left_HighLight_A", "Me_Left_RighLight_A"),
	("Me_Left_HighLight_A_END", "Me_Left_RighLight_A_END"),
	("Me_Left_HighLight_B", "Me_Left_RighLight_B"),
	("Me_Left_HighLight_B_END", "Me_Left_RighLight_B_END"),
	("sitakuti_r_1", "sitakuti_l_1"),
	("sitakuti_r_1_end", "sitakuti_l_1_end"),
	("uekuti_r_1", "uekuti_l_1"),
	("uekuti_r_1_end", "uekuti_l_1_end"))

def main():
	
	block= []
	
	tog_left= Draw.Create(0)
	tog_right= Draw.Create(0)
	limit= Draw.Create(0.001)
	mirror_bone= Draw.Create(0)
	center_bone= Draw.Create(0)
	
	block.append(("Left > Right", tog_left))
	block.append(("Right > Left", tog_right))
	block.append(("Limit: ", limit, 0.001, 100.0))
	block.append(("Mirror Bone", mirror_bone))
	block.append(("Center Bone", center_bone))
	
	if Draw.PupBlock("3DCG X mirror tool", block):
		
		ob= Scene.GetCurrent().objects.active
		if ob==None or ob.type!="Mesh":
			Draw.PupMenu("Please select mesh object%t")
			return
		
		me= ob.getData(False, True)
		vert_group= me.getVertGroupNames()
		limit= limit.val
		
		if tog_left.val:
			reverse= 0
		else:
			reverse= 1
		
		if mirror_bone.val:
			
			for f1 in mirror_bones:
				if f1[reverse] in vert_group:
					weight_verts= me.getVertsFromGroup(f1[reverse], 0, range(len(me.verts)))
					
					for f2 in weight_verts:
						
						reverse_vec= me.verts[f2].co
						reverse_vec= Vector(-reverse_vec.x, reverse_vec.y, reverse_vec.z)
						
						for f3 in me.verts:
							if f3.co.x-limit <= reverse_vec.x <= f3.co.x+limit and\
							f3.co.y-limit <= reverse_vec.y <= f3.co.y+limit and\
							f3.co.z-limit <= reverse_vec.z <= f3.co.z+limit :
								
								if f1[1-reverse] not in vert_group:
									me.addVertGroup(f1[1-reverse])
								
								weight= 0.0
								for f4 in me.getVertexInfluences(f2):
									if f1[reverse] == f4[0]:
										weight= f4[1]
								
								me.assignVertsToGroup(f1[1-reverse], [f3.index], weight, 1)
							
		if center_bone.val:
			
			for f1 in center_bones:
				
				if f1 in vert_group:
					
					weight_verts= []
					for f2 in me.getVertsFromGroup(f1, 0, range(len(me.verts))):
						if reverse:
							if me.verts[f2].co.x < 0:
								weight_verts.append(me.verts[f2])
						else:
							if me.verts[f2].co.x > 0:
								weight_verts.append(me.verts[f2])
					
					for f2 in weight_verts:
						
						mirror_vec= Vector(-f2.co.x, f2.co.y, f2.co.z)
						
						for f3 in me.verts:
							if f3.co.x-limit <= mirror_vec.x <= f3.co.x+limit and\
							f3.co.y-limit <= mirror_vec.y <= f3.co.y+limit and\
							f3.co.z-limit <= mirror_vec.z <= f3.co.z+limit :
								
								weight= 0.0
								for f4 in me.getVertexInfluences(f2.index):
									if f1 == f4[0]:
										weight= f4[1]
								
								me.assignVertsToGroup(f1, [f3.index], weight, 1)
		
	return
	
if __name__ =="__main__":
	main()
