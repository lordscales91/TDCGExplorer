#!BPY
"""
Name: '3DCG tmo file (.tmo)...'
Blender: 249
Group: 'Import'
Tooltip: 'Import 3DCustomGirl tmo (.tmo) file.'
"""

import Blender
from Blender import *
from struct import *
from Blender.Mathutils import *

####################
#      Import      #
####################
def Import(tmo_node, tmo_mat):
	####################
	## Import() sub functions
	
	##
	####################
	
	ob= Scene.GetCurrent().objects.active
	
	if ob == None: return
	if ob.type != "Armature": return
	
	arm= ob.getData(False, False)
	pose= ob.getPose()
	
	for f1_1, f1_2 in zip(tmo_node, tmo_mat):
		bone_name= f1_1.rpartition("|")[2]
		
		if bone_name in arm.bones.keys():
			
			#Scale
			Smat= f1_2.copy().scalePart()
			Smat= Matrix([Smat.x,0,0],[0,Smat.z,0],[0,0,Smat.y]).resize4x4()
			#Rotation
			Rmat2= arm.bones[bone_name].matrix["ARMATURESPACE"].copy().toQuat().toMatrix() *RotationMatrix(90, 3, "x")
			Rmat2.resize4x4()
			Rmat= f1_2.copy().toEuler()
			Rmat= Euler(Rmat.x, -Rmat.y, -Rmat.z).toMatrix().resize4x4()
			Rmat= Rmat2 *Rmat *Rmat2.invert()
			#Translation
			path= f1_1.split("|")
			Tmat= arm.bones[bone_name].head["ARMATURESPACE"].copy()
			if len(path)>2 and path[-2] in arm.bones.keys():
				Tmat= arm.bones[bone_name].head["ARMATURESPACE"].copy() -arm.bones[path[-2]].head["ARMATURESPACE"].copy()
			Tmat2= f1_2.copy().translationPart()
			Tmat2= Vector(Tmat2.x, -Tmat2.z, Tmat2.y) *0.5
			Tmat= Tmat -Tmat2
			if len(path)>2 and path[-2] in arm.bones.keys():
				Tmat= Tmat *arm.bones[bone_name].matrix["ARMATURESPACE"].copy().toQuat().inverse()
			
			pose.bones[bone_name].localMatrix= Smat *Rmat
			pose.bones[bone_name].loc= Tmat
	
	pose.update()
	for f1 in Object.Get():
		if f1.type == "Armature":
			f1.getPose().update()
		if f1.type == "Mesh":
			f1.getData(False, True).update()
	Blender.Redraw()

####################
#       Gui        #
####################
def Gui():
	####################
	## Gui() sub functions
	
	def update_Registry():
		data= {}
		data["tmo_file"]= tmo_file
		Blender.Registry.SetKey("3DCG TMO Importer", data, True)
	##
	####################
	
	####################
	## Draw.Register
	
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
		
		global tmo_file, tmo_node, tmo_mat, frame
		
		if evt==0:
			def tmofile_selector(file): global tmo_file;tmo_file=file
			Window.FileSelector( tmofile_selector, "File select", tmo_file )
		elif evt==1:
			path= sys.basename( tmo_file )
			tmo_file= sys.join( B["TmoFileDir"].val, path )
		elif evt==2:
			path= sys.dirname( tmo_file )
			tmo_file= sys.join( path, B["TmoFileBase"].val )
		
		elif evt==10:
			if Blender.sys.exists(tmo_file) == 1:
				frame= 1
				
				file= open(tmo_file, "rb")
				if file.read(4) == "TMO1":
					
					file.seek(20)
					tmo_node= []
					for f1 in xrange( unpack("<i", file.read(4))[0] ):
						path= " "
						while path[-1] != chr(0x00):
							path+= file.read(1)
						tmo_node.append( path[1:-1] )
					
					tmo_mat= []
					for f1 in xrange( unpack("<i", file.read(4))[0] ):
						tmo_mat.append([])
						for f2 in xrange( unpack("<i", file.read(4))[0] ):
							floats= unpack("<16f", file.read(64))
							mat= Matrix(\
								[floats[ 0],floats[ 1],floats[ 2],floats[ 3]],
								[floats[ 4],floats[ 5],floats[ 6],floats[ 7]],
								[floats[ 8],floats[ 9],floats[10],floats[11]],
								[floats[12],floats[13],floats[14],floats[15]])
							tmo_mat[-1].append(mat)
				
				file.close()
				
		elif evt==20:
			frame= B["TimeLine"].val
		
		elif evt==30:
			Import(tmo_node, tmo_mat[frame-1])
		
		else: return
		
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
		
		#[Push]: TMO file select
		W= Win(0,80,100,100, 10,0,-10,-10)
		Draw.PushButton("TMO file select", 0, W["X"], W["Y"], W["W"], W["H"], )
		
		#[String]: Dir text box
		W= Win(0,70,50,80, 10,0,-1,-2)
		B["TmoFileDir"]= Draw.String("", 1, W["X"], W["Y"], W["W"], W["H"], sys.dirname(tmo_file), 200, )
		
		#[String]: Base text box
		W= Win(50,70,100,80, 1,0,-10,-2)
		B["TmoFileBase"]= Draw.String("", 2, W["X"], W["Y"], W["W"], W["H"], sys.basename(tmo_file), 200, )
		
		
		#[Push]: TMO Lead
		W= Win(0,50,100,65, 10,0,-10,-0)
		Draw.PushButton("TMO Lead", 10, W["X"], W["Y"], W["W"], W["H"], )
		
		
		if tmo_node:
			
			#[Slider]: TimeLine
			W= Win(0,25,100,45, 10,0,-10,-0)
			B["TimeLine"]= Draw.Slider("TimeLine :", 20, W["X"], W["Y"], W["W"], W["H"], frame, 1, len(tmo_mat), 1, )
			
			#[Push]: Apply
			W= Win(0,0,100,20, 10,10,-10,-0)
			Draw.PushButton("Pose Apply", 30, W["X"], W["Y"], W["W"], W["H"], )
			
	
	##
	####################
	
	global tmo_file, tmo_node, tmo_mat, frame
	
	B= {}
	
	tmo_file= "C:/file.tmo"
	tmo_node= []
	tmo_mat= []
	frame= 1
	
	#Registry read
	rdict= Registry.GetKey("3DCG TMO Importer", True)
	if rdict:
		try:
			if isinstance(rdict["tmo_file"], str):
				tmo_file= rdict["tmo_file"]
		except:
			update_Registry()
	
	# Draw start
	Draw.Register(GuiDraw,Event,Button)

if __name__ =="__main__":
	#GUI start
	Gui()
