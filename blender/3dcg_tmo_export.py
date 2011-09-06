#!BPY
"""
Name: '3DCG tmo file (.tmo)...'
Blender: 249
Group: 'Export'
Tooltip: 'Export 3DCustomGirl tmo (.tmo) file.'
"""

import Blender
from Blender import *
from struct import *
from Blender.Mathutils import *


####################
#    TMOExport()   #
####################
def TMOExport(Option):
	####################
	## TMOExport() sub functions
	
	def GetNode(Option):
		#File
		if Option["Mode"] =="File":
			#Text file
			if Blender.sys.splitext(Option["SourceFile"])[1].lower() ==".txt":
				import re
				File= open(Option["SourceFile"], "r")
				node= []
				for line in File.readlines():
					line= re.compile("[\|\w]+").search(line)
					if line !=None:
						node.append(line.group(0))
				return node
			
			#TSO or TMO
			File= open(Option["SourceFile"], "rb")
			head= File.read(4)
			if head =="TSO1":
				File.seek(4)
			elif head =="TMO1":
				File.seek(20)
			else:
				raise IOError, "Does not support this file (" +Option["SourceFile"] +")"
			node= []
			for line in xrange( unpack("i", File.read(4))[0] ):
				node.append("")
				while True:
					char= File.read(1)
					if char ==chr(0x00): break
					node[-1]+= char
			return node
		
		#Blender Armature
		elif Option["Mode"] =="Armature":
			import string
			Arm= Object.Get(Option["SourceArm"]).getData()
			bak= [[]]
			for f1 in Arm.bones.values():
				if not f1.hasParent():
					bak[0].append(f1)
			node= []
			while len(bak) >0:
				if len(bak[-1]) >0:
					bone= bak[-1][0]
					node.append(bone)
					if bone.hasChildren():
						bak.append(bone.children)
					else:
						del bak[-1][0]
				else:
					del bak[-1]
					if len(bak) >0:
						del bak[-1][0]
			for f1 in xrange(len(node)):
				parents= [node[f1]]
				while parents[0].hasParent():
					parents.insert(0, parents[0].parent)
				node[f1]= ""
				for bone in parents:
					node[f1]+= "|" +bone.name
			return node
		
		#Blender Text
		elif Option["Mode"] =="Text":
			import re
			txt= Text.Get(Option["SourceText"])
			node= []
			for line in txt.asLines():
				line= re.compile("[\|\w]+").search(line)
				if line !=None:
					node.append(line.group(0))
			return node
	
	def GetFrame():
		context= Scene.GetCurrent().getRenderingContext()
		frame2= context.endFrame() -context.startFrame() +1
		frame1= frame2
		if frame1 >1:
			frame1-= 1
		return frame1, frame2
	
	def GetMatrix(Node):
		node= []
		for bones in Node:
			bones= bones.split("|")
			parent= None
			if bones[-2] !="":
				parent= bones[-2]
			node.append({ "Name":bones[-1], "Parent":parent })
		
		context= Scene.GetCurrent().getRenderingContext()
		frame= context.endFrame() -context.startFrame() +1
		BeforeFrame= context.currentFrame()
		
		SelectArm= []
		for ob in Object.GetSelected():
			if ob.type =="Armature":
				SelectArm.append(ob)
		if len(SelectArm) <1:
			raise KeyError, "Armature not selected."
		
		mat= []
		for frm in xrange(frame):
			context.currentFrame(context.startFrame() +frm)
			
			mat.append([])
			for data in node:
				for ob in SelectArm:
					try:
						Arm= ob.getData()
						Pose= ob.getPose()
						ArmMat= ob.matrixWorld
						
						BoneMat= Arm.bones[data["Name"]].matrix["ARMATURESPACE"].copy()
						PoseMat= Pose.bones[data["Name"]].poseMatrix.copy() *ArmMat
						
						if data["Parent"] !=None:
							BonePrtMat= Arm.bones[data["Parent"]].matrix["ARMATURESPACE"].copy()
							PosePrtMat= Pose.bones[data["Parent"]].poseMatrix.copy() *ArmMat
							
							#Scale Matrix
							Smat= PoseMat.copy().scalePart() -PosePrtMat.copy().scalePart() +Vector(1,1,1)
							Smat= Matrix([Smat.x,0,0],[0,Smat.z,0],[0,0,Smat.y]).resize4x4()
							
							#Rotation Matrix
							Rmat= RotationMatrix(90,3,"x") *( BoneMat.copy().rotationPart().invert() *PoseMat.copy().rotationPart() )
							Rmat*= ( RotationMatrix(90,3,"x") *( BonePrtMat.copy().rotationPart().invert() *PosePrtMat.copy().rotationPart() ) ).invert()
							Rmat= Rmat.resize4x4()
							
							#Translation Matrix
							Tmat= ( PoseMat.copy() -PosePrtMat.copy() ).translationPart()
							Tmat*= ( BonePrtMat.copy().invert() *PosePrtMat.copy() ).rotationPart().invert()
							Tmat= TranslationMatrix( Vector(Tmat.x,Tmat.z,-Tmat.y) *2 )
						else:
							#Scale Matrix
							Smat= PoseMat.copy().scalePart()
							Smat= Matrix([Smat.x,0,0],[0,Smat.z,0],[0,0,Smat.y]).resize4x4()
							
							#Rotation Matrix
							Rmat= ( BoneMat.copy().rotationPart().invert() *PoseMat.copy().rotationPart() ).toEuler()
							Rmat= Euler(Rmat.x,Rmat.z,-Rmat.y).toMatrix().resize4x4()
							
							#Translation Matrix
							Tmat= PoseMat.copy().translationPart()
							Tmat= TranslationMatrix( Vector(Tmat.x,Tmat.z,-Tmat.y) *2 )
						
						mat[-1].append( Smat *Rmat *Tmat )
						break
					except KeyError:
						pass
				else:
					raise KeyError, "Boneless in the armature. (" +data["Name"] +")"
		
		context.currentFrame(BeforeFrame)
		return mat
	##
	####################
	
	#Get data
	Node= GetNode(Option)
	frame1, frame2= GetFrame()
	matrix= GetMatrix(Node)
	
	#Binary set
	bin= "TMO1" +pack("5i", 0,0, frame1, 0, len(Node))
	for line in Node:
		bin+= line +chr(0x00)
	bin+= pack("i", frame2)
	for mats in matrix:
		bin+= pack("i", len(mats))
		for mat in mats:
			bin+= pack("16f",\
			mat[0][0],mat[0][1],mat[0][2],mat[0][3],\
			mat[1][0],mat[1][1],mat[1][2],mat[1][3],\
			mat[2][0],mat[2][1],mat[2][2],mat[2][3],\
			mat[3][0],mat[3][1],mat[3][2],mat[3][3] )
	bin+= pack("i", 0)
	
	#File write
	File= open(Option["Output"], "wb")
	File.write(bin)
	File.close

####################
#       Gui()      #
####################
def Gui():
	####################
	## Gui() sub functions
	
	def update_Registry():
		data= {}
		data["Option"]= Option
		Blender.Registry.SetKey("3DCG TMO Exporter", data, True)
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
		
		if evt ==0:
			Draw.Exit()
			update_Registry()
			TMOExport(Option)
			return
		
		
		elif evt ==1:
			def OutputSelector(Path):
				Option["Output"]= Path
			Blender.Window.FileSelector(OutputSelector, "Select Output", Option["Output"])
		
		elif evt ==2:
			Option["Output"]= Blender.sys.join( B["OutputDir"].val, Blender.sys.basename(Option["Output"]) )
		
		elif evt ==3:
			Option["Output"]= Blender.sys.join( Blender.sys.dirname(Option["Output"]), B["OutputBase"].val )
		
		
		elif evt ==10:
			Option["Mode"]= "File"
			
		elif evt ==11:
			Option["Mode"]= "Armature"
			
		elif evt ==12:
			Option["Mode"]= "Text"
			
			
		elif evt ==20:
			def SourceFileSelector(Path):
				Option["SourceFile"]= Path
			Blender.Window.FileSelector(SourceFileSelector, "Select source file", Option["SourceFile"])
		
		elif evt ==21:
			Option["SourceFile"]= Blender.sys.join( B["SourceFileDir"].val, Blender.sys.basename(Option["SourceFile"]) )
		
		elif evt ==22:
			Option["SourceFile"]= Blender.sys.join( Blender.sys.dirname(Option["SourceFile"]), B["SourceFileBase"].val )
		
			
		elif evt ==30:
			Option["SourceArm"]= BlenderArm[B["SourceArm"].val]
			
			
		elif evt ==40:
			Option["SourceText"]= BlenderText[B["SourceText"].val]
			
			
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
		
		#Output select
		W= Win(0,80,100,100, 10,0,-10,-10)
		Draw.PushButton("Output select", 1, W["X"], W["Y"], W["W"], W["H"], )
		
		#Output dir
		W= Win(0,70,50,80, 10,0,-3,0)
		B["OutputDir"]= Draw.String("", 2, W["X"], W["Y"], W["W"], W["H"], Blender.sys.dirname(Option["Output"]), 200, "")
		
		#Output base
		W= Win(50,70,100,80, 3,0,-10,0)
		B["OutputBase"]= Draw.String("", 3, W["X"], W["Y"], W["W"], W["H"], Blender.sys.basename(Option["Output"]), 200, "")
		
		
		#Radio (File)
		W= Win(5,57,5,57, -10,-10,10,10)
		if Option["Mode"] =="File":
			Draw.Toggle("*", 10, W["X"], W["Y"], W["W"], W["H"], 1, )
		else:
			Draw.Toggle(" ", 10, W["X"], W["Y"], W["W"], W["H"], 0, )
		
		#Radio (Armature)
		if len(BlenderArm) >0:
			W= Win(5,35,5,35, -10,-10,10,10)
			if Option["Mode"] =="Armature":
				Draw.Toggle("*", 11, W["X"], W["Y"], W["W"], W["H"], 1, )
			else:
				Draw.Toggle(" ", 11, W["X"], W["Y"], W["W"], W["H"], 0, )
		
		#Radio (Text)
		if len(BlenderText) >0:
			W= Win(55,35,55,35, -10,-10,10,10)
			if Option["Mode"] =="Text":
				Draw.Toggle("*", 12, W["X"], W["Y"], W["W"], W["H"], 1, )
			else:
				Draw.Toggle(" ", 12, W["X"], W["Y"], W["W"], W["H"], 0, )
		
		
		#Text
		if Option["Mode"] =="File":
			TextSet(10,70,5,-15, 0.5,0,0)
		else:
			TextSet(10,70,5,-15, 0,0,0)
		Draw.Text("Source: File", "normal")
		
		#Source file select
		W= Win(10,55,100,70, 0,0,-10,-20)
		Draw.PushButton("Source file select (.tso .tmo .txt)", 20, W["X"], W["Y"], W["W"], W["H"], )
		
		#Source dir
		W= Win(10,45,55,55, 0,0,-3,0)
		B["SourceFileDir"]= Draw.String("", 21, W["X"], W["Y"], W["W"], W["H"], Blender.sys.dirname(Option["SourceFile"]), 200, "")
		
		#Source base
		W= Win(55,45,100,55, 3,0,-10,0)
		B["SourceFileBase"]= Draw.String("", 22, W["X"], W["Y"], W["W"], W["H"], Blender.sys.basename(Option["SourceFile"]), 200, "")
		
		
		#Text
		if Option["Mode"] =="Armature":
			TextSet(10,45,5,-15, 0.5,0,0)
		else:
			TextSet(10,45,5,-15, 0,0,0)
		Draw.Text("Source: Armature", "normal")
		
		#Armature menu
		if len(BlenderArm) >0:
			menu= ""
			for f1 in xrange(len(BlenderArm)):
				menu+= "|" +BlenderArm[f1] +" %x" +str(f1)
			W= Win(10,25,50,45, 0,10,-0,-20)
			B["SourceArm"]= Draw.Menu(menu, 30, W["X"], W["Y"], W["W"], W["H"], BlenderArm.index(Option["SourceArm"]), )
		else:
			TextSet(10,45,5,-20, 0,0,0)
			Draw.Text("Armature not found", "normal")
		
		
		#Text
		if Option["Mode"] =="Text":
			TextSet(60,45,5,-15, 0.5,0,0)
		else:
			TextSet(60,45,5,-15, 0,0,0)
		Draw.Text("Source: Text", "normal")
		
		#Text menu
		if len(BlenderText) >0:
			menu= ""
			for f1 in xrange(len(BlenderText)):
				menu+= "|" +BlenderText[f1] +" %x" +str(f1)
			W= Win(60,25,100,45, 0,10,-10,-20)
			B["SourceText"]= Draw.Menu(menu, 40, W["X"], W["Y"], W["W"], W["H"], BlenderText.index(Option["SourceText"]), )
		else:
			TextSet(60,45,5,-20, 0,0,0)
			Draw.Text("Text not found", "normal")
		
		
		#Export
		W= Win(0,0,100,25, 10,10,-10,-0)
		Draw.PushButton("Export", 0, W["X"], W["Y"], W["W"], W["H"], )
		
	##
	####################
	
	#Get Blender data
	BlenderArm= []
	for ob in Scene.GetCurrent().getChildren():
		if ob.getType() =="Armature":
			BlenderArm.append(ob.name)
	BlenderText= []
	for txt in Text.Get():
		BlenderText.append(txt.name)
	
	#Options
	Option= {}
	Option["Output"]= "C:/file.tmo"
	Option["Mode"]= "File"
	Option["SourceFile"]= "C:/file.tmo"
	Option["SourceArm"]= None
	if len(BlenderArm) >0:
		Option["SourceArm"]= BlenderArm[0]
	Option["SourceText"]= None
	if len(BlenderText) >0:
		Option["SourceText"]= BlenderText[0]
	B= {}
	
	#Registry
	rdict= Registry.GetKey("3DCG TMO Exporter", True)
	if rdict:
		try:
			Option["Output"]= rdict["Option"]["Output"]
			Option["Mode"]= rdict["Option"]["Mode"]
			Option["SourceFile"]= rdict["Option"]["SourceFile"]
			
			try:
				Text.Get(rdict["Option"]["SourceText"])
				Option["SourceText"]= rdict["Option"]["SourceText"]
			except: pass
			
			try:
				Object.Get(rdict["Option"]["SourceArm"])
				Option["SourceArm"]= rdict["Option"]["SourceArm"]
			except: pass
		except:
			update_Registry()
	
	#GUI draw
	Draw.Register(GuiDraw,Event,Button)

if __name__ =="__main__":
	#GUI start
	Gui()
