#!BPY
"""
Name: 'Armature Mimic'
Blender: 249
Group: 'Object'
Tooltip: 'Make Armature mimic'
"""

import Blender
from Blender import *

def main():
	
	#Select object
	select_ob= Object.GetSelected()
	for f1 in select_ob:
		if f1.type !="Armature":
			select_ob.remove(f1)
	if len(select_ob) <2:
		Draw.PupMenu("Please select object more than 2.%t")
		return
	
	source_ob= select_ob[1]
	ob= select_ob[0]
	source_pose= source_ob.getPose()
	pose= ob.getPose()
	
	for f1 in pose.bones.values():
		if f1.name in source_pose.bones.keys():
			for f2 in f1.constraints:
				f1.constraints.remove(f2)
			
			con= f1.constraints.append(10)
			con.__setitem__(Constraint.Settings.TARGET ,source_ob)
			con.__setitem__(Constraint.Settings.BONE ,f1.name)
			
			con= f1.constraints.append(9)
			con.__setitem__(Constraint.Settings.TARGET ,source_ob)
			con.__setitem__(Constraint.Settings.BONE ,f1.name)
			
			con= f1.constraints.append(8)
			con.__setitem__(Constraint.Settings.TARGET ,source_ob)
			con.__setitem__(Constraint.Settings.BONE ,f1.name)
	
	#source_pose.update()
	#pose.update()
	
	Scene.GetCurrent().update(0)
	
	Blender.Redraw()
	
	return

if __name__ =="__main__":
	main()
