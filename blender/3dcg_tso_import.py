#!BPY
"""
Name: '3DCG tso file (.tso)...'
Blender: 249
Group: 'Import'
Tooltip: 'Import 3DCustomGirl tso (.tso) file.'
"""

import Blender
from Blender import *
from struct import *
from Blender.Mathutils import *
from tdcg import TSOFile

class Heap(object):
	def __init__(self):
		self.ary = []
		self.map = {}

	def append(self, key):
		if key not in self.map:
			idx = len(self.ary)
			self.map[key] = idx
			self.ary.append(key)
		else:
			idx = self.map[key]
		return idx

	def clear(self):
		del self.ary[:]
		self.map.clear()

####################
#     Import()     #
####################
def Import(Option):
	
	####################
	## Import() sub functions
	
	def ImportArmature():
		BoneData= []
		for node in tso.nodes:
			BoneData.append({})
			BoneData[-1]["Name"]= node.name
			BoneData[-1]["Parent"]= node.parent_name
			mat= node.transform
			BoneData[-1]["Matrix"]= Matrix( mat[0:4], mat[4:8], mat[8:12], mat[12:16] )
		
		CopyData= BoneData[:]
		Arm= Armature.New(Blender.sys.makename(Option["File"],"",1))
		Arm_ob= Scene.GetCurrent().objects.new(Arm)
		Arm.makeEditable()
		ReviseMatrix= ScaleMatrix(0.5,3) *RotationMatrix(90,3,"x")
		while len(CopyData) !=0:
			data= CopyData.pop(0)
			EditBone= Armature.Editbone()
			EditBone.head= data["Matrix"].translationPart()*ReviseMatrix
			EditBone.tail= data["Matrix"].translationPart()*ReviseMatrix +Vector(0,0,-1)
			EditBone.roll= 0
			if data["Parent"] !="":
				try:
					EditBone.parent= Arm.bones[data["Parent"]]
					EditBone.head+= Arm.bones[data["Parent"]].head
					EditBone.tail+= Arm.bones[data["Parent"]].head
					Arm.bones[data["Name"]]= EditBone
				except KeyError:
					CopyData.append(data)
			else:
				Arm.bones[data["Name"]]= EditBone
		Arm.update()
		Bones= Arm.bones.values()
		Arm.makeEditable()
		for bone in Bones:
			if bone.hasChildren():
				if len(bone.children) ==1:
					Arm.bones[bone.name].tail= bone.children[0].head["ARMATURESPACE"]
					Arm.bones[bone.children[0].name].options= Armature.CONNECTED
				else:
					vec= Vector(0,0,0)
					for chld in bone.children:
						vec+= chld.head["ARMATURESPACE"]
					Arm.bones[bone.name].tail= Vector(\
					vec.x/len(bone.children),\
					vec.y/len(bone.children),\
					vec.z/len(bone.children) )
			elif bone.hasParent():
				Arm.bones[bone.name].tail=\
				(bone.head["ARMATURESPACE"] -bone.parent.head["ARMATURESPACE"])\
				*0.1 +bone.head["ARMATURESPACE"]
		Arm.update()
		Pose= Arm_ob.getPose()
		for data in BoneData:
			eul= data["Matrix"].toEuler()
			mat= Arm.bones[data["Name"]].matrix["ARMATURESPACE"].rotationPart()
			Pose.bones[data["Name"]].localMatrix=(\
			mat *Euler(eul.x,eul.z,-eul.y).toMatrix() *mat.invert() ).resize4x4()
	
	def ImportNode():
		txt= Text.New("N: " +Blender.sys.makename(Option["File"],"",1))
		for node in tso.nodes:
			txt.write(node.path +"\n")
	
	def ImportImage():
		for tex in tso.textures:
			# tex.depth must be 4.
			img= Image.New(tex.name, tex.width, tex.height, 32)
			cnt= 0
			for y in xrange(tex.height):
				for x in xrange(tex.width):
					p = ( ord(tex.data[cnt+0]), ord(tex.data[cnt+1]), ord(tex.data[cnt+2]), ord(tex.data[cnt+3]) )
					img.setPixelI(x, y, p)
					cnt+= 4
			img.pack()
	
	def ImportShader():
		for scr in tso.scripts:
			txt= Text.New("S: " +scr.name)
			for line in scr.lines:
				txt.write(line +"\n")
	
	def ImportMaterial():
		for scr in tso.sub_scripts:
			txt= Text.New("M: " +scr.name)
			for line in scr.lines:
				txt.write(line +"\n")
	
	def ImportMesh():
		heap	= Heap()
		faces	= []
		uvs	= []
		specs	= []
		skin_weights_map	= {}

		materials= [ Blender.Material.New(scr.name) for scr in tso.sub_scripts ]
		
		for mesh in tso.meshes:
			heap.clear()
			del faces[:]
			del uvs[:]
			del specs[:]
			skin_weights_map.clear()

			local_materials= []

			for sub in mesh.sub_meshes:
				cnt = 0
				va = None
				vb = None
				vc = None
				a = 0
				b = 0
				c = 0

				if materials[sub.spec] not in local_materials:
					local_materials.append(materials[sub.spec])
			
				local_bone_names= [ tso.nodes[bone_index].name for bone_index in sub.bone_indices ]
			
				for v in sub.vertices:
					va = vb
					vb = vc
					vc = v
					key = ( v.co, v.no )
					a = b
					b = c
					c = heap.append(key)

					if key not in skin_weights_map:
						skin_weights = []
						for bone_index, w in v.skin_weights:
							skin_weights.append(( local_bone_names[bone_index], w ))
						skin_weights_map[key] = skin_weights

					if cnt < 2:
						cnt+= 1
						continue
					if a == b or b == c or c == a:
						cnt+= 1
						continue
					if cnt % 2 == 0:
						faces.append(( a, b, c ))
					else:
						faces.append(( a, c, b ))
					uvs.append({ a: va.uv, b: vb.uv, c: vc.uv })
					specs.append(sub.spec)
					cnt+= 1

			me= Mesh.New(mesh.name)
			me.materials= local_materials
			
			VertVector= []
			for co, no in heap.ary:
				VertVector.append( Vector(co[0], -co[2], co[1]) *0.5 )
			me.verts.extend(VertVector)

			# print "dump faces"
			# for i in xrange(len(faces)):
			# 	print faces[i]

			me.faces.extend(faces)

			# print "dump me.faces"
			# for i in xrange(len(faces)):
			# 	print me.faces[i]

			for i in xrange(len(faces)):
				face = me.faces[i]
				uv = uvs[i]
				face.mat= me.materials.index(materials[specs[i]])
				face.uv= [ Vector(uv[v.index]) for v in face.verts ]

			ob= Scene.GetCurrent().objects.new(me, mesh.name)

			for i, key in enumerate(heap.ary):
				for name, w in skin_weights_map[key]:
					if name not in me.getVertGroupNames():
						me.addVertGroup(name)
					me.assignVertsToGroup(name, [i], w, 1)
				
	##
	####################
	
	start_time = Blender.sys.time()

	tso= TSOFile()
	tso.load(Option["File"])
	
	if Option["Armature"]:
		ImportArmature()
	if Option["Node Text"]:
		ImportNode()
	if Option["Image"]:
		ImportImage()
	if Option["Shader Text"]:
		ImportShader()
	if Option["Material Text"]:
		ImportMaterial()
	if Option["Mesh"]:
		ImportMesh()

	end_time = Blender.sys.time()
	print 'tso import time:', end_time - start_time

####################
#       Gui()      #
####################
def Gui():
	
	####################
	## Gui() sub functions
	
	def update_Registry():
		data= {}
		data["Option"]= Option
		Blender.Registry.SetKey("tso_import", data, True)
	##
	####################
	
	####################
	##Draw.Register
	
	####################
	#      Event()     #
	####################
	def Event(evt,val):
		#Esc
		if evt ==Draw.ESCKEY:
			update_Registry()
			Draw.Exit()
			return
		
	####################
	#     Button()     #
	####################
	def Button(evt):
		
		global Option
		
		if False:
			pass
		
		elif evt ==0:
			update_Registry()
			Draw.Exit()
			Import(Option)
			return
		
		elif evt ==1:
			Option["Armature"]= 1 -Option["Armature"]
		elif evt ==2:
			Option["Image"]= 1 -Option["Image"]
		elif evt ==3:
			Option["Mesh"]= 1 -Option["Mesh"]
		elif evt ==4:
			Option["Node Text"]= 1 -Option["Node Text"]
		elif evt ==5:
			Option["Shader Text"]= 1 -Option["Shader Text"]
		elif evt ==6:
			Option["Material Text"]= 1 -Option["Material Text"]
		
		elif evt ==7:
			def TSOselect(Path):
				global Option
				Option["File"]= Path
			Blender.Window.FileSelector(TSOselect, "Select TSO file", Option["File"])
		elif evt ==8:
			Option["File"]= Blender.sys.join(B["TSOdir"].val, Blender.sys.basename(Option["File"]))
		elif evt ==9:
			Option["File"]= Blender.sys.join(Blender.sys.dirname(Option["File"]), B["TSObase"].val)
		
		else:
			return
		
		Draw.Redraw(1)
		
	####################
	#    GuiDraw()     #
	####################
	def GuiDraw():
		
		####################
		## GuiDraw() sub functions
		
		def Win(rX=0, rY=0, rW=0, rH=0, mX=0, mY=0, mW=0, mH=0):
			WinX=Window.GetAreaSize()[0]; WinY=Window.GetAreaSize()[1]
			X= int( WinX *( rX /100.0 ) +mX )
			Y= int( WinY *( rY /100.0 ) +mY )
			W= int( ( WinX *( rW /100.0 ) +mW ) -X )
			H= int( ( WinY *( rH /100.0 ) +mH ) -Y )
			return {"X":X, "Y":Y, "W":W, "H":H}
		
		def TextSet(rX, rY, mX, mY, R=0, G=0, B=0):
			WinX=Window.GetAreaSize()[0]; WinY=Window.GetAreaSize()[1]
			BGL.glColor3f(R, G, B)
			BGL.glRasterPos2i( int( WinX *( rX /100.0 ) +mX ), int( WinY *( rY /100.0 ) +mY ) )
			return
		##
		####################
		
		global B
		
		#Armature
		W= Win(0,85,50,100, 10,0,-3,-10)
		Draw.Toggle("Armature", 1, W["X"], W["Y"], W["W"], W["H"], Option["Armature"], )
		
		#Image
		W= Win(0,70,50,85, 10,0,-3,-5)
		Draw.Toggle("Image", 2, W["X"], W["Y"], W["W"], W["H"], Option["Image"], )
		
		#Mesh
		W= Win(0,55,50,70, 10,0,-3,-5)
		Draw.Toggle("Mesh", 3, W["X"], W["Y"], W["W"], W["H"], Option["Mesh"], )
		
		#Node Text
		W= Win(50,85,100,100, 3,0,-10,-10)
		Draw.Toggle("Node Text", 4, W["X"], W["Y"], W["W"], W["H"], Option["Node Text"], )
		
		#Shader Text
		W= Win(50,70,100,85, 3,0,-10,-5)
		Draw.Toggle("Shader Text", 5, W["X"], W["Y"], W["W"], W["H"], Option["Shader Text"], )
		
		#Material Text
		W= Win(50,55,100,70, 3,0,-10,-5)
		Draw.Toggle("Material Text", 6, W["X"], W["Y"], W["W"], W["H"], Option["Material Text"], )
		
		
		#TSO Select
		W= Win(0,25,100,55, 10,35,-10,-10)
		Draw.PushButton("TSO select", 7, W["X"], W["Y"], W["W"], W["H"], )
		
		#TSO dir
		W= Win(0,25,50,25, 10,0,-3,30)
		B["TSOdir"]= Draw.String("", 8, W["X"], W["Y"], W["W"], W["H"], Blender.sys.dirname(Option["File"]), 200, "")
		
		#TSO base
		W= Win(50,25,100,25, 3,0,-10,30)
		B["TSObase"]= Draw.String("", 9, W["X"], W["Y"], W["W"], W["H"], Blender.sys.basename(Option["File"]), 200, "")
		
		
		#Import
		W= Win(0,0,100,25, 10,10,-10,-10)
		Draw.PushButton("Import", 0, W["X"], W["Y"], W["W"], W["H"], )
	##
	####################
	
	global Option, B
	
	B= {}
	
	Option= {}
	Option["Armature"]= 0
	Option["Image"]= 1
	Option["Mesh"]= 1
	Option["Node Text"]= 0
	Option["Shader Text"]= 0
	Option["Material Text"]= 0
	Option["File"]= "C:/file.tso"
	
	rdict= Registry.GetKey("tso_import", True)
	if rdict:
		try:
			Option["Armature"]= rdict["Option"]["Armature"]
			Option["Image"]= rdict["Option"]["Image"]
			Option["Mesh"]= rdict["Option"]["Mesh"]
			Option["Node Text"]= rdict["Option"]["Node Text"]
			Option["Shader Text"]= rdict["Option"]["Shader Text"]
			Option["Material Text"]= rdict["Option"]["Material Text"]
			Option["File"]= rdict["Option"]["File"]
		except:
			update_Registry()
	
	Draw.Register(GuiDraw,Event,Button)

if __name__ =="__main__":
	#GUI start
	Gui()

# vim: set sw=4 ts=4:
