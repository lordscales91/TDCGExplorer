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


####################
#     Import()     #
####################
def Import(Option):
	
	####################
	## Import() sub functions
	
	def ImpoetArmature(op):
		global Seek
		
		File.seek(Seek)
		index= unpack("i", File.read(4))[0]
		
		Node= []
		for line in xrange(index):
			Node.append("")
			while True:
				char= File.read(1)
				if char ==chr(0x00):
					break
				Node[-1]+= char
		
		if op:
			File.seek(4, 1)
			BoneData= []
			for path in Node:
				BoneData.append({})
				BoneData[-1]["Name"]= path.rsplit("|")[-1]
				BoneData[-1]["Parent"]= path.rsplit("|")[-2]
				mat= unpack("16f", File.read(64))
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
		else:
			File.seek(4 +(64 *index), 1)
		
		Seek= File.tell()
		return Node
	
	def ImportNode(op, node):
		if op:
			txt= Text.New("N: " +Blender.sys.makename(Option["File"],"",1))
			for line in node:
				txt.write(line +"\n")
	
	def ImportImage(op):
		global Seek
		File.seek(Seek)
		for f1 in xrange( unpack("i", File.read(4))[0] ):
			
			name= ""
			while True:
				char= File.read(1)
				if char ==chr(0x00): break
				name+= char
			
			while True:
				if File.read(1) ==chr(0x00): break
			
			imgX, imgY, imgC= unpack("3i", File.read(12))
			if op:
				data= []
				for f2 in xrange(imgX *imgY):
					if imgC ==4:
						bin= File.read(4)
						data.append([ ord(bin[0]), ord(bin[1]), ord(bin[2]), ord(bin[3]) ])
					else:
						bin= File.read(3)
						data.append([ ord(bin[0]), ord(bin[1]), ord(bin[2]), 255 ])
				
				img= Image.New(name, imgX, imgY, 8*imgC)
				count= 0
				for y in xrange(imgY):
					for x in xrange(imgX):
						img.setPixelI(x, y, data[count])
						count+= 1
				img.pack()
			else:
				File.seek(imgX *imgY *imgC, 1)
		
		Seek= File.tell()
	
	def ImportShader(op):
		global Seek
		File.seek(Seek)
		for f1 in xrange( unpack("i", File.read(4))[0] ):
			name= ""
			while True:
				char= File.read(1)
				if char ==chr(0x00): break
				name+= char
			LineIndex= unpack("i", File.read(4))[0]
			if op:
				txt= Text.New("S: " +name)
				for f2 in xrange(LineIndex):
					line= ""
					while True:
						char= File.read(1)
						if char ==chr(0x00): break
						line+= char
					txt.write(line +"\n")
			else:
				for f2 in xrange(LineIndex):
					while True:
						if File.read(1) ==chr(0x00): break
		Seek= File.tell()
	
	def ImportMaterial(op):
		global Seek
		File.seek(Seek)
		mate= []
		for f1 in xrange( unpack("i", File.read(4))[0] ):
			name= ""
			while True:
				char= File.read(1)
				if char ==chr(0x00): break
				name+= char
			mate.append(name)
			while True:
				if File.read(1) ==chr(0x00): break
			LineIndex= unpack("i", File.read(4))[0]
			if op:
				txt= Text.New("M: " +name)
				for f2 in xrange(LineIndex):
					line= ""
					while True:
						char= File.read(1)
						if char ==chr(0x00): break
						line+= char
					txt.write(line +"\n")
			else:
				for f2 in xrange(LineIndex):
					while True:
						if File.read(1) ==chr(0x00): break
		Seek= File.tell()
		return mate
	
	def ImportMesh(op, node, mate):
		if op:
			File.seek(Seek)
			for f1 in xrange(len(node)):
				node[f1]= node[f1].rpartition("|")[-1]
			
			UseMate= []
			for m in mate:
				UseMate.append( Blender.Material.New(m) )
			
			for ObIndex in xrange( unpack("i", File.read(4))[0] ):
				name= ""
				while True:
					char= File.read(1)
					if char ==chr(0x00): break
					name+= char
				
				mat= unpack("16i", File.read(64))
				mat= Matrix( mat[0:4], mat[4:8], mat[8:12], mat[12:16] )
				
				File.read(4)
				VertData= []
				BinData= []
				LocalUseMate= []
				for vg in xrange( unpack("i", File.read(4))[0] ):
					LocalMateIndex= unpack("i", File.read(4))[0]
					LocalMate= mate[LocalMateIndex]
					LocalBone= []
					
					if not LocalUseMate.count(UseMate[LocalMateIndex]):
						LocalUseMate.append(UseMate[LocalMateIndex])
					
					for bone in xrange( unpack("i", File.read(4))[0] ):
						LocalBone.append( node[ unpack("i", File.read(4))[0] ] )
					
					VertData.append({"Mate":UseMate[LocalMateIndex], "Vert":[]})
					BinData.append([])
					for vert in xrange( unpack("i", File.read(4))[0] ):
						data= {}
						bin= chr(LocalMateIndex)
						data["mate"]= LocalMateIndex
						bin+= File.read(12)
						data["co"]= unpack("3f", bin[1:13])
						bin+= File.read(12)
						data["no"]= unpack("3f", bin[13:25])
						data["uv"]= unpack("2f", File.read(8))
						data["weight"]= []
						for bone in xrange( unpack("i", File.read(4))[0] ):
							weight= unpack("if", File.read(8))
							data["weight"].append([ LocalBone[weight[0]], weight[1] ])
						VertData[-1]["Vert"].append(data)
						BinData[-1].append(bin)
				
				UseVert= []
				VertVector= []
				UseBin= []
				UseFace= []
				UseUV= []
				Mate= []
				sort_face= []
				for f1 in range(len(BinData)):
					
					skip= 2
					bak= []
					
					for f2 in range(len(BinData[f1])):
						if len(bak)==3: del bak[0]
						bak.append(BinData[f1][f2])
						
						if not bak[-1] in UseBin:
							UseVert.append(VertData[f1]["Vert"][f2])
							co= UseVert[-1]["co"]
							VertVector.append( Vector(co[0], -co[2], co[1]) *0.5 )
							UseBin.append(bak[-1])
						
						if skip == 0:
							if bak[0] != bak[1] != bak[2] != bak[0]:
								num= [ UseBin.index(bak[0]), UseBin.index(bak[1]), UseBin.index(bak[2]) ]
								
								if f2%2 == 0:
									UseFace.append(num)
									UseUV.append((\
										Vector( VertData[f1]["Vert"][f2-2]["uv"] ),\
										Vector( VertData[f1]["Vert"][f2-1]["uv"] ),\
										Vector( VertData[f1]["Vert"][f2-0]["uv"] )))
								else:
									num.reverse()
									UseFace.append(num)
									UseUV.append((\
										Vector( VertData[f1]["Vert"][f2-0]["uv"] ),\
										Vector( VertData[f1]["Vert"][f2-1]["uv"] ),\
										Vector( VertData[f1]["Vert"][f2-2]["uv"] )))
								
								Mate.append(VertData[f1]["Mate"])
								
								if sorted(num) in sort_face:
									del UseFace[sort_face.index(sorted(num))]
									del UseUV[sort_face.index(sorted(num))]
									del Mate[sort_face.index(sorted(num))]
									del sort_face[sort_face.index(sorted(num))]
								sort_face.append(sorted(num))
						else:
							skip-= 1
				
				me= Mesh.New(name)
				me.materials= LocalUseMate
				ob= Scene.GetCurrent().objects.new(me, name)
				
				me.verts.extend(VertVector)
				for f1 in xrange(len(UseVert)):
					for f2 in UseVert[f1]["weight"]:
						if not f2[0] in me.getVertGroupNames():
							me.addVertGroup(f2[0])
						me.assignVertsToGroup(f2[0], [f1], f2[1], 1)
				
				me.faces.extend(UseFace)
				for f1 in range(len(me.faces)):
					me.faces[f1].mat= me.materials.index(Mate[f1])
					me.faces[f1].uv= UseUV[f1]
				
	##
	####################
	
	global Seek
	
	File= open(Option["File"], "rb")
	
	Seek= 4
	Node= ImpoetArmature(Option["Armature"])
	ImportNode(Option["Node Text"], Node)
	ImportImage(Option["Image"])
	ImportShader(Option["Shader Text"])
	Material= ImportMaterial(Option["Material Text"])
	ImportMesh(Option["Mesh"], Node, Material)

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
