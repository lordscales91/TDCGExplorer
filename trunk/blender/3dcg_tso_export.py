#!BPY
"""
Name: '3DCG tso file (.tso)...'
Blender: 249
Group: 'Export'
Tooltip: 'Export 3DCustomGirl tso (.tso) file.'
"""

import Blender
import re
import string
import pprint
from Blender import *
from struct import *
from Blender.Mathutils import *
from StringIO import StringIO

import subprocess

# NvTriStrip binary location
# Defaults to Blender's scripts directory
NvTriStripPath = Blender.sys.join(Blender.Get("scriptsdir"), "NvTriStripper-cli.exe")

# Check the NvTriStrip binary
if not Blender.sys.exists(NvTriStripPath):
	raise Exception("Can't find NvTriStrip binary (%s)" % NvTriStripPath)

####################
#      Export      #
####################
def Export(Option):
	####################
	## Export() sub functions
	
	#NvTriStrip
	def optimize(vert_indices):
		# Stripifying mesh
		NvTriStrip = subprocess.Popen([NvTriStripPath], 
			stdin = subprocess.PIPE, stdout = subprocess.PIPE)
		for vi in vert_indices:
			NvTriStrip.stdin.write(str(vi) + " ")
		NvTriStrip.stdin.write("-1\n")
		stripcount = int(NvTriStrip.stdout.readline())
		if stripcount < 1:
			raise Exception("NvTriStrip returned 0 strips. Aborting")
		nativelist = []
		striptype = int(NvTriStrip.stdout.readline())
		nativelength = int(NvTriStrip.stdout.readline())
		nativelist.extend(map(int, NvTriStrip.stdout.readline().split()))
		return nativelist

	#Node
	def GetNodeBin(option):
		if option["Mode"] =="File":
			file= open( option["File"], "rb" )
			if file.read(4) =="TSO1":
				for f1 in xrange(unpack("i", file.read(4))[0]):
					path= ""
					while True:
						path+= file.read(1)
						if path[-1] ==chr(0x00):
							break
					TSOnode.append( path[:-1].rpartition("|")[2] )
				index= unpack("i", file.read(4))[0]
				file.seek( index*64, 1 )
				end= file.tell()
				file.seek(4)
				bin= file.read( end-4 )
				file.close()
				return bin
			else:
				raise KeyError, "NodeSourceFile error"
		elif option["Mode"] =="Armature":
			ob= Object.Get(option["Armature"])
			arm= ob.getData(False, False)
			pose= ob.getPose()
			
			bone_queue= [[]]
			#Get first parent bone
			for f1 in arm.bones.values():
				if not f1.hasParent():
					bone_queue[0].append(f1.name)
			bone_queue.sort( lambda x, y: cmp(string.lower(x), string.lower(y)) )
			#Get children
			while True:
				if len(bone_queue[0]) >0:
					if len(bone_queue[-1]) >0:
						TSOnode.append(bone_queue[-1][0])
						if arm.bones[bone_queue[-1][0]].hasChildren():
							bone_queue.append([])
							for f1 in arm.bones[bone_queue[-2][0]].children:
								bone_queue[-1].append(f1.name)
							bone_queue[-1].sort( lambda x, y: cmp(string.lower(x), string.lower(y)) )
						else:
							del bone_queue[-1][0]
					else:
						del bone_queue[-1]
						del bone_queue[-1][0]
				else:
					break
			
			#Get bone index
			bin= pack("i", len(TSOnode))
			
			#Get bone path binary
			for f1 in TSOnode:
				path= [f1]
				while True:
					if arm.bones[path[-1]].hasParent():
						path.append( arm.bones[path[-1]].parent.name )
					else:
						break
				for f2 in reversed(path):
					bin+= "|" +f2
				bin+= chr(0x00)
			
			#Get bone index
			bin+= pack("i", len(TSOnode))
			
			#Get bone matrix
			for f1 in TSOnode:
				pose_bone= pose.bones[f1]
				
				#Scale matrix
				Smat= pose_bone.localMatrix.copy().scalePart()
				Smat= Matrix( [Smat.x, 0, 0], [0, Smat.z, 0], [0, 0, Smat.y] ).resize4x4()
				
				#Rotation matrix
				Rmat= pose_bone.localMatrix.copy().toEuler()
				Rmat= Euler(Rmat.x, Rmat.z, -Rmat.y).toMatrix().resize4x4()
				
				#Translation matrix
				Tmat= arm.bones[f1].head["ARMATURESPACE"].copy()
				if arm.bones[f1].hasParent():
					Tmat-= arm.bones[f1].parent.head["ARMATURESPACE"].copy()
				Tmat= TranslationMatrix( Vector(Tmat.x, Tmat.z, -Tmat.y) *2 )
				
				m= Smat *Rmat *Tmat
				bin+= pack("16f",\
					m[0][0], m[0][1], m[0][2], m[0][3],\
					m[1][0], m[1][1], m[1][2], m[1][3],\
					m[2][0], m[2][1], m[2][2], m[2][3],\
					m[3][0], m[3][1], m[3][2], m[3][3])
			
			#print len(TSOnode)
			#raise KeyError, "NodeMode Armature"
			return bin
		else:
			raise KeyError, "NodeMode error"
	
	#Texture
	def GetTextureBin(option):
		bin= pack("<i", len(option))
		for tex in option:
			bin+= tex["Name"] +chr(0x00)
			bin+= '"' +tex["FileName"] +tex["Ext"] +'"' +chr(0x00)
			img= Image.Get(tex["Image"])
			width, height= img.getMaxXY()
			bin+= pack( "<3i", width, height, 4 )
			for y in xrange(height):
				for x in xrange(width):
					px= img.getPixelI(x, y)
					for d in xrange(4):
						bin+= chr(px[d])
		return bin
	
	#Shader
	def GetShaderBin(option):
		bin= pack("<i", len(option))
		for shd in option:
			bin+= shd["Name"] +chr(0x00)
			
			if shd["Mode"] =="File":
				file= open( shd["File"], "r" )
				txt= file.readlines()
				for line in range( len(txt)-1, -1, -1 ):
					if txt[line][-1] =="\n":
						txt[line]= txt[line][:-1]
			elif shd["Mode"] =="Text":
				txt= Text.Get(shd["Text"]).asLines()
			else:
				raise KeyError, "ShaderMode error"
			
			if txt[-1] ==txt[-2] =="":
				del txt[-1]
			bin+= pack( "<i", len(txt) )
			for line in txt:
				bin+= line +chr(0x00)
		return bin
	
	#Material
	def GetMaterialBin(option):
		bin= pack("<i", len(option))
		for name, mate in option.iteritems():
			TSOmaterial.append(name)
			bin+= mate["Name"] +chr(0x00)
			bin+= "cgfxShader" +chr(0x00)
			
			if mate["Mode"] =="File":
				file= open( mate["File"], "r" )
				txt= file.readlines()
				for line in range( len(txt)-1, -1, -1 ):
					if txt[line][-1] =="\n":
						txt[line]= txt[line][:-1]
			elif mate["Mode"] =="Text":
				txt= Text.Get(mate["Text"]).asLines()
			else:
				raise KeyError, "ShaderMode error"
			
			for line in range( len(txt)-1, -1, -1 ):
				if None ==re.compile("\S+").match(txt[line], 1):
					del txt[line]
			bin+= pack( "<i", len(txt) )
			for line in txt:
				bin+= line +chr(0x00)
		return bin
	
	#Mesh
	def GetMeshBin( option, TSOnode, TSOmaterial ):
		WEIGHT_EPSILON = 1.0e-4

		class Vertex:
			def __init__(self):
				self.co = None
				self.no = None
				self.uv = None
				self.skin_weights = []

			def __eq__(self, v):
				return self.co == v.co and self.uv == v.uv

			def __hash__(self):
				return hash(self.co) ^ hash(self.uv)

		def create_vertex(me, face, index):
			a = Vertex()
			v = face.verts[index]
			co = Vector(v.co.x, v.co.z, -v.co.y) *2 *transform
			no = Vector(v.no.x, v.no.z, -v.no.y)
			uv = face.uv[index]
			a.co = (co.x, co.y, co.z)
			a.no = (no.x, no.y, no.z)
			a.uv = (uv.x, uv.y)

			for name, w in me.getVertexInfluences(v.index):
				if w < WEIGHT_EPSILON:
					continue
				if name not in TSOnode:
					continue
				a.skin_weights.append([ name, w ])

			a.skin_weights.sort(lambda a, b: cmp(b[1], a[1]))

			del a.skin_weights[4:]

			total= 0.0
			for name, w in a.skin_weights:
				total+= w
			for sw in a.skin_weights:
				sw[1] = sw[1]/total

			return a

		class TriangleFace(object):
			pass

		def CreateTriangleFace(me, face, a, b, c):
			f = TriangleFace()
			f.verts= [ face.verts[a], face.verts[b], face.verts[c] ]
			f.uv= [ face.uv[a], face.uv[b], face.uv[c] ]
			f.mat= face.mat

			return f

		def write_cstring(writer, str):
			writer.write(str + chr(0x00))

		def write_int(writer, i):
			writer.write(pack('<i', i))

		def write_float(writer, i):
			writer.write(pack('<f', i))

		def write_matrix4(writer, m):
			writer.write(pack('<16f',\
				m[0][0], m[0][1], m[0][2], m[0][3],\
				m[1][0], m[1][1], m[1][2], m[1][3],\
				m[2][0], m[2][1], m[2][2], m[2][3],\
				m[3][0], m[3][1], m[3][2], m[3][3]))

		def write_vector3(writer, v):
			writer.write(pack('<3f', v[0], v[1], v[2]))

		def write_vector2(writer, v):
			writer.write(pack('<2f', v[0], v[1]))

		class SubMesh:
			def __init__(self):
				self.spec = None
				self.bone_indices = None
				self.vertices = None

			def write(self, writer):
				write_int(writer, self.spec)

				write_int(writer, len(self.bone_indices))
				for name in self.bone_indices:
					write_int(writer, TSOnode.index(name))
					
				bone_index_map = {}
				for name in self.bone_indices:
					bone_index_map[name] = self.bone_indices.index(name)

				write_int(writer, len(self.vertices))

				for v in self.vertices:
					write_vector3(writer, v.co)
					write_vector3(writer, v.no)
					write_vector2(writer, v.uv)
					if len(v.skin_weights) != 0:
						write_int(writer, len(v.skin_weights))
						for name, w in v.skin_weights:
							write_int(writer, bone_index_map[name])
							write_float(writer, w)
					else:
						write_int(writer, 1)
						write_int(writer, 0)
						write_float(writer, 1.0)
					
		def create_transform(ob_mat):
			#Scale
			Smat= ob_mat.copy().scalePart()
			Smat= Matrix( [Smat.x, 0, 0], [0, Smat.z, 0], [0, 0, Smat.y] ).resize4x4()
			#Rotation
			Rmat= ob_mat.copy().toEuler()
			Rmat= Euler(Rmat.x, Rmat.z, -Rmat.y).toMatrix().resize4x4()
			#Translation
			Tmat= ob_mat.copy().translationPart()
			Tmat= TranslationMatrix( Vector(Tmat.x, Tmat.z, -Tmat.y) *2 )
			#Mix
			m= Smat *Rmat *Tmat
			return m

		def create_tri_faces(me_faces):
			ret = []
			for face in me_faces:
				if len(face.verts) ==4:
					mid_co_13 = MidpointVecs(face.verts[1].co, face.verts[3].co)
					mid_co_02 = MidpointVecs(face.verts[0].co, face.verts[2].co)
					i1= (face.verts[0].co-mid_co_13).length +(face.verts[2].co-mid_co_13).length
					i2= (face.verts[1].co-mid_co_02).length +(face.verts[3].co-mid_co_02).length
					if i1 >=i2:
						ret.append( CreateTriangleFace(me, face, 0, 1, 3) )
						ret.append( CreateTriangleFace(me, face, 1, 2, 3) )
					else:
						ret.append( CreateTriangleFace(me, face, 0, 1, 2) )
						ret.append( CreateTriangleFace(me, face, 0, 2, 3) )
				else:
					ret.append( CreateTriangleFace(me, face, 0, 1, 2) )
			return ret
			
		def create_sub_meshes(me, tri_faces, max_palettes):
			faces_1 = tri_faces
			faces_2 = []

			subs = []

			print "  vertices bone_indices"
			print "  -------- ------------"

			while len(faces_1) != 0:
				mat = faces_1[0].mat
				bmap = {}
				bone_indices = []
				vmap = {}
				vertices = []
				vert_indices = []

				for f in faces_1:
					if f.mat != mat:
						faces_2.append(f)
						continue

					adding_bone_names = set()
					for v in f.verts:
						for name, w in me.getVertexInfluences(v.index):
							if w < WEIGHT_EPSILON:
								continue
							if name in bmap:
								continue
							adding_bone_names.add(name)

					if len(bmap) + len(adding_bone_names) > max_palettes:
						faces_2.append(f)
						continue

					for name in adding_bone_names:
						bmap[name] = len(bone_indices)
						bone_indices.append(name)

					for i in xrange(len(f.verts)):
						a = create_vertex(me, f, i)
						if a not in vmap:
							vmap[a] = len(vertices)
							vertices.append(a)
						vert_indices.append(vmap[a])
					
				# print '#vert_indices', len(vert_indices)
				optimized_indices = optimize(vert_indices)
				# print '#optimized_indices', len(optimized_indices)

				sub = SubMesh()
				sub.spec = mat_spec_map[mat]

				# print '#bone_indices', len(bone_indices)
				sub.bone_indices = bone_indices
				sub.vertices = [ vertices[vidx] for vidx in optimized_indices ]

				print "  %8d %12d" % (len(sub.vertices), len(sub.bone_indices))

				subs.append(sub)

				t = faces_1
				faces_1 = faces_2
				faces_2 = t
				del t[:]

			return subs

		name_spec_map = {}
		for i, name in enumerate(TSOmaterial):
			name_spec_map[name] = i

		writer = StringIO()

		write_int(writer, len(option))

		for mesh in option:
			ob= Object.Get(mesh["Object"])
			me= ob.getData(False, True)
			
			mat_spec_map = {}
			for i, material in enumerate(me.materials):
				mat_spec_map[i] = name_spec_map[material.name]

			write_cstring(writer, mesh["Name"])

			transform = create_transform( ob.getMatrix("worldspace") )
			write_matrix4(writer, transform)
			
			write_int(writer, 1)
			
			subs = create_sub_meshes(me, create_tri_faces(me.faces), 16)

			write_int(writer, len(subs))
			
			for sub in subs:
				sub.write(writer)
		
		bin = writer.getvalue()
		writer.close()
		return bin
	
	##
	####################
	
	global TSOnode, TSOmaterial
	
	pp = pprint.PrettyPrinter(indent=2)
	pp.pprint(Option)
	
	start_time = Blender.sys.time()

	#TSO need variable
	TSOnode= []
	TSOmaterial= []
	
	#Get binary
	bin= "TSO1"
	bin+= GetNodeBin(Option["Node"])
	bin+= GetTextureBin(Option["Texture"])
	bin+= GetShaderBin(Option["Shader"])
	bin+= GetMaterialBin(Option["Material"])
	bin+= GetMeshBin( Option["Mesh"], TSOnode, TSOmaterial )
	
	#Write file
	file= open( Option["Export"]["Path"], "wb" )
	file.write( bin )
	file.close()
	
	end_time = Blender.sys.time()
	print 'tso export time:', end_time - start_time

####################
#       Gui        #
####################
def Gui():
	####################
	## Gui() sub functions
	
	def GetData():
		
		Scn= Scene.GetCurrent()
		BlenderData= {}
		BlenderData["Armature"]= []
		BlenderData["Mesh"]= []
		BlenderData["Image"]= []
		BlenderData["Text"]= []
		
		for ob in Scn.getChildren():
			if ob.getType() =="Armature":
				BlenderData["Armature"].append(ob.name)
			elif ob.getType() =="Mesh":
				BlenderData["Mesh"].append(ob.name)
		
		for img in Image.Get():
			BlenderData["Image"].append(img.name)
		
		for txt in Text.Get():
			BlenderData["Text"].append(txt.name)
		
		return BlenderData
	
	def update_Registry():
		data= {}
		data["Tab"]= Tab
		data["Option"]= Option
		data["GuiState"]= GuiState
		Blender.Registry.SetKey("tso_export", data, True)
	
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
		
		global Tab
		
		#pass
		if False: pass
		
		#Export button
		elif evt==0:
			if Tab =="Export":
				#Error check
				GuiState["ERROR"]= ""
				if Option["Node"]["Mode"]=="File" and sys.exists(Option["Node"]["File"])!=1:
					GuiState["ERROR"]= "Node: Node source file is not found."
				elif Option["Node"]["Mode"]=="Armature" and Option["Node"]["Armature"]==None:
						GuiState["ERROR"]= "Node: Blender armature not found."
				elif not len(Option["Mesh"]):
					GuiState["ERROR"]= "Mesh: Make one or more mesh."
				elif not len(Option["Texture"]):
					GuiState["ERROR"]= "Texture: Make one or more texture."
				elif not len(Option["Shader"]):
					GuiState["ERROR"]= "Shader: Make one or more shader."
				elif not len(Option["Material"]):
					GuiState["ERROR"]= "Material: Assign material to mesh."
				for f1 in Option["Mesh"]:
					if f1["Object"] ==None:
						GuiState["ERROR"]= "Mesh: Blender mesh object is not found."
						break
				for f1 in Option["Texture"]:
					if f1["Image"] ==None:
						GuiState["ERROR"]= "Texture: Blender image is not found."
						break
				for f1 in Option["Shader"]:
					if f1["Mode"]=="File" and sys.exists(f1["File"])!=1:
						GuiState["ERROR"]= "Shader: Shader source file is not found."
						break
					if f1["Mode"]=="Text" and f1["Text"]==None:
						GuiState["ERROR"]= "Shader: Blender text is not found."
						break
				for name, f1 in Option["Material"].iteritems():
					if f1["Mode"]=="File" and sys.exists(f1["File"])!=1:
						GuiState["ERROR"]= "Material: Material source file is not found."
						break
					if f1["Mode"]=="Text" and f1["Text"]==None:
						GuiState["ERROR"]= "Material: Blender text is not found."
						break
				
				if GuiState["ERROR"] =="":
					Export(Option)
					
					update_Registry()
					Draw.Exit()
					return
			
			Tab="Export"
		
		#Tab
		elif evt==1: Tab="Node"
		elif evt==2: Tab="Mesh"
		elif evt==3: Tab="Texture"
		elif evt==4: Tab="Shader"
		elif evt==5: Tab="Material"
		
		
		#Node
		elif evt==100: Option["Node"]["Mode"]= "File"
		elif evt==101: Option["Node"]["Mode"]= "Armature"
		
		elif evt==110:
			def nodefile_selector(file): Option["Node"]["File"]=file
			Window.FileSelector( nodefile_selector, "File select", Option["Node"]["File"] )
		elif evt==111:
			file= sys.basename( Option["Node"]["File"] )
			Option["Node"]["File"]= sys.join( B["NodeSourceFileDir"].val, file )
		elif evt==112:
			dir= sys.dirname( Option["Node"]["File"] )
			Option["Node"]["File"]= sys.join( dir, B["NodeSourceFileBase"].val )
		elif evt==120:
			if len(BlenderData["Armature"]) >0:
				Option["Node"]["Armature"]= BlenderData["Armature"][ B["SelectArmature"].val ]
		elif evt==121:
			act= Scene.GetCurrent().objects.active
			if act!=None and act.type=="Armature" and act.name in BlenderData["Armature"]:
				Option["Node"]["Armature"]= act.name
		
		#Mesh
		elif evt==200:
			if len(Option["Mesh"]) >0:
				GuiState["ActiveMesh"]= Option["Mesh"][B["ActiveMesh"].val]["Name"]
				GuiState["ActiveMeshName"]= Option["Mesh"][B["ActiveMesh"].val]["Name"]
				GuiState["ActiveMeshObject"]= Option["Mesh"][B["ActiveMesh"].val]["Object"]
		elif evt==201:
			Option["Mesh"].append({ "Name":GuiState["ActiveMeshName"], "Object":GuiState["ActiveMeshObject"] })
			GuiState["ActiveMesh"]= GuiState["ActiveMeshName"]
		elif evt==202:
			if len(Option["Mesh"]) >0:
				for f1 in xrange(len(Option["Mesh"])):
					if Option["Mesh"][f1]["Name"] ==GuiState["ActiveMesh"]:
						index= f1
						break
				Option["Mesh"][index]["Name"]= GuiState["ActiveMeshName"]
				Option["Mesh"][index]["Object"]= GuiState["ActiveMeshObject"]
				GuiState["ActiveMesh"]= GuiState["ActiveMeshName"]
		elif evt==203:
			if len(Option["Mesh"]) >0:
				for f1 in xrange(len(Option["Mesh"])):
					if Option["Mesh"][f1]["Name"] ==GuiState["ActiveMesh"]:
						index= f1
						break
				del Option["Mesh"][index]
				if len(Option["Mesh"]) ==index:
					index-= 1
				if len(Option["Mesh"]) >0:
					GuiState["ActiveMeshName"]= Option["Mesh"][index]["Name"]
					GuiState["ActiveMeshObject"]= Option["Mesh"][index]["Object"]
					GuiState["ActiveMesh"]= GuiState["ActiveMeshName"]
		elif evt==210:
			GuiState["ActiveMeshName"]= B["MeshName"].val
		elif evt==211:
			GuiState["ObjectNameCopy"]= 1 -GuiState["ObjectNameCopy"]
		elif evt==220:
			if len(BlenderData["Mesh"]) >0:
				GuiState["ActiveMeshObject"]= BlenderData["Mesh"][B["MeshObject"].val]
				if GuiState["ObjectNameCopy"]:
					GuiState["ActiveMeshName"]= GuiState["ActiveMeshObject"]
		elif evt==221:
			ob= Scene.GetCurrent().objects.active
			if ob.type =="Mesh":
				GuiState["ActiveMeshObject"]= ob.name
				if GuiState["ObjectNameCopy"]:
					GuiState["ActiveMeshName"]= GuiState["ActiveMeshObject"]
		
		#Texture
		elif evt==300:
			if len(Option["Texture"]) >0:
				GuiState["ActiveTexture"]= Option["Texture"][B["ActiveTexture"].val]["Name"]
				GuiState["ActiveTextureName"]= Option["Texture"][B["ActiveTexture"].val]["Name"]
				GuiState["ActiveTextureFileName"]= Option["Texture"][B["ActiveTexture"].val]["FileName"]
				GuiState["ActiveTextureImage"]= Option["Texture"][B["ActiveTexture"].val]["Image"]
				GuiState["ActiveImageExt"]= Option["Texture"][B["ActiveTexture"].val]["Ext"]
		elif evt==301:
			Option["Texture"].append({\
				"Name":GuiState["ActiveTextureName"],\
				"FileName":GuiState["ActiveTextureFileName"],\
				"Image":GuiState["ActiveTextureImage"],\
				"Ext":GuiState["ActiveImageExt"] })
			GuiState["ActiveTexture"]= GuiState["ActiveTextureName"]
		elif evt==302:
			if len(Option["Texture"]) >0:
				for f1 in xrange(len(Option["Texture"])):
					if Option["Texture"][f1]["Name"] ==GuiState["ActiveTexture"]:
						index= f1
						break
				Option["Texture"][index]["Name"]= GuiState["ActiveTextureName"]
				Option["Texture"][index]["FileName"]= GuiState["ActiveTextureFileName"]
				Option["Texture"][index]["Image"]= GuiState["ActiveTextureImage"]
				Option["Texture"][index]["Ext"]= GuiState["ActiveImageExt"]
				GuiState["ActiveTexture"]= GuiState["ActiveTextureName"]
		elif evt==303:
			if len(Option["Texture"]) >0:
				for f1 in xrange(len(Option["Texture"])):
					if Option["Texture"][f1]["Name"] ==GuiState["ActiveTexture"]:
						index= f1
						break
				del Option["Texture"][index]
				if len(Option["Texture"]) ==index:
					index-= 1
				if len(Option["Texture"]) >0:
					GuiState["ActiveTexture"]= Option["Texture"][index]["Name"]
					GuiState["ActiveTextureName"]= Option["Texture"][index]["Name"]
					GuiState["ActiveTextureFileName"]= Option["Texture"][index]["FileName"]
					GuiState["ActiveTextureImage"]= Option["Texture"][index]["Image"]
					GuiState["ActiveImageExt"]= Option["Texture"][index]["Ext"]
		elif evt==310:
			if len(BlenderData["Image"]) >0:
				GuiState["ActiveTextureImage"]= BlenderData["Image"][B["TextureImage"].val]
				if GuiState["ImageNameCopy"]:
					GuiState["ActiveTextureName"]= GuiState["ActiveTextureImage"]
					if GuiState["TextureNameCopy"]:
						GuiState["ActiveTextureFileName"]= GuiState["ActiveTextureName"]
		elif evt==320:
			GuiState["ActiveTextureName"]= B["TextureName"].val
			if GuiState["TextureNameCopy"]:
				GuiState["ActiveTextureFileName"]= GuiState["ActiveTextureName"]
		elif evt==321:
			GuiState["ImageNameCopy"]= 1 -GuiState["ImageNameCopy"]
		elif evt==330:
			GuiState["ActiveTextureFileName"]= B["TextureFileName"].val
		elif evt==331:
			GuiState["ActiveImageExt"]= ImageExt[B["TextureFileExt"].val]
		elif evt==332:
			GuiState["TextureNameCopy"]= 1 -GuiState["TextureNameCopy"]
		
		#Shader
		elif evt==400:
			if len(Option["Shader"]) >0:
				GuiState["ActiveShader"]= Option["Shader"][B["ActiveShader"].val]["Name"]
				GuiState["ActiveShaderName"]= Option["Shader"][B["ActiveShader"].val]["Name"]
				GuiState["ActiveShaderMode"]= Option["Shader"][B["ActiveShader"].val]["Mode"]
				GuiState["ActiveShaderFile"]= Option["Shader"][B["ActiveShader"].val]["File"]
				GuiState["ActiveShaderText"]= Option["Shader"][B["ActiveShader"].val]["Text"]
		elif evt==401:
			Option["Shader"].append({\
				"Name":GuiState["ActiveShaderName"],\
				"Mode":GuiState["ActiveShaderMode"],\
				"File":GuiState["ActiveShaderFile"],\
				"Text":GuiState["ActiveShaderText"]})
			GuiState["ActiveShader"]= GuiState["ActiveShaderName"]
		elif evt==402:
			if len(Option["Shader"]) >0:
				for f1 in xrange(len(Option["Shader"])):
					if Option["Shader"][f1]["Name"] ==GuiState["ActiveShader"]:
						index= f1
						break
				Option["Shader"][index]["Name"]= GuiState["ActiveShaderName"]
				Option["Shader"][index]["Mode"]= GuiState["ActiveShaderMode"]
				Option["Shader"][index]["File"]= GuiState["ActiveShaderFile"]
				Option["Shader"][index]["Text"]= GuiState["ActiveShaderText"]
				GuiState["ActiveShader"]= GuiState["ActiveShaderName"]
		elif evt==403:
			if len(Option["Shader"]) >0:
				for f1 in xrange(len(Option["Shader"])):
					if Option["Shader"][f1]["Name"] ==GuiState["ActiveShader"]:
						index= f1
						break
				del Option["Shader"][index]
				if len(Option["Shader"]) ==index:
					index-= 1
				if len(Option["Shader"]) >0:
					GuiState["ActiveShader"]= Option["Shader"][index]["Name"]
					GuiState["ActiveShaderName"]= Option["Shader"][index]["Name"]
					GuiState["ActiveShaderMode"]= Option["Shader"][index]["Mode"]
					GuiState["ActiveShaderFile"]= Option["Shader"][index]["File"]
					GuiState["ActiveShaderText"]= Option["Shader"][index]["Text"]
		elif evt==404:
			GuiState["ActiveShaderName"]= B["ShaderName"].val
		elif evt==410:
			GuiState["ActiveShaderMode"]= "File"
		elif evt==411:
			GuiState["ActiveShaderMode"]= "Text"
		elif evt==420:
			def shaderfile_selector(file): GuiState["ActiveShaderFile"]=file
			Window.FileSelector( shaderfile_selector, "File select", GuiState["ActiveShaderFile"] )
		elif evt==421:
			GuiState["ActiveShaderFile"]= sys.join( B["ShaderFileDir"].val, sys.basename( GuiState["ActiveShaderFile"] ) )
		elif evt==422:
			GuiState["ActiveShaderFile"]= sys.join( sys.dirname( GuiState["ActiveShaderFile"] ), B["ShaderFileBase"].val )
		elif evt==430:
			if len(BlenderData["Text"]) >0:
				GuiState["ActiveShaderText"]= BlenderData["Text"][B["ActiveShaderText"].val]
		
		#Material
		elif evt==500:
			if len( Option["Material"] ) >0:
				name= list(Option["Material"])[B["ActiveMaterial"].val]
				GuiState["ActiveMaterial"]= name
		elif evt==501:
			Option["Material"][GuiState["ActiveMaterial"]]["Name"]= B["MaterialName"].val
		elif evt==510:
			Option["Material"][GuiState["ActiveMaterial"]]["Mode"]= "File"
		elif evt==511:
			Option["Material"][GuiState["ActiveMaterial"]]["Mode"]= "Text"
		elif evt==520:
			def materialfile_selector(file): Option["Material"][GuiState["ActiveMaterial"]]["File"]=file
			Window.FileSelector( materialfile_selector, "File select", Option["Material"][GuiState["ActiveMaterial"]]["File"] )
		elif evt==521:
			Option["Material"][GuiState["ActiveMaterial"]]["File"]= sys.join( B["MaterialFileDir"].val, sys.basename( GuiState["ActiveMaterialFile"] ) )
		elif evt==522:
			Option["Material"][GuiState["ActiveMaterial"]]["File"]= sys.join( sys.dirname( GuiState["ActiveMaterialFile"] ), B["MaterialFileBase"].val )
		elif evt==530:
			if len(BlenderData["Text"]) >0:
				Option["Material"][GuiState["ActiveMaterial"]]["Text"]= BlenderData["Text"][B["MaterialText"].val]
			
		#Export
		elif evt==600:
			def output_selector(file): Option["Export"]["Path"]=file
			Window.FileSelector( output_selector, "Output select", Option["Export"]["Path"] )
		elif evt==601:
			Option["Export"]["Path"]= sys.join( B["OutputDir"].val, sys.basename( Option["Export"]["Path"] ) )
		elif evt==602:
			Option["Export"]["Path"]= sys.join( sys.dirname( Option["Export"]["Path"] ), B["OutputBase"].val )
		
		
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
		
		
		#Tab set
		Step= [0,0,0,0,0]
		if   Tab=="Node":     Step[0]=1
		elif Tab=="Mesh":     Step[1]=1
		elif Tab=="Texture":  Step[2]=1
		elif Tab=="Shader":   Step[3]=1
		elif Tab=="Material": Step[4]=1
		
		#[Text]: Tab text
		TextSet(0,12,5,0 ,0,0,0)
		Draw.Text("TSO export setting (Esc:Quit)", "normal")
		
		#[Toggle]: Node
		W= Win(0,0,15,10, 0,0,-0,-0)
		Draw.Toggle("Node", 1, W["X"], W["Y"], W["W"], W["H"], Step[0], )
		
		#[Toggle]: Mesh
		W= Win(15,0,30,10, 0,0,-0,-0)
		Draw.Toggle("Mesh", 2, W["X"], W["Y"], W["W"], W["H"], Step[1], )
		
		#[Toggle]: Texture
		W= Win(30,0,45,10, 0,0,-0,-0)
		Draw.Toggle("Texture", 3, W["X"], W["Y"], W["W"], W["H"], Step[2], )
		
		#[Toggle]: Shader
		W= Win(45,0,60,10, 0,0,-0,-0)
		Draw.Toggle("Shaders", 4, W["X"], W["Y"], W["W"], W["H"], Step[3], )
		
		#[Toggle]: Material
		W= Win(60,0,75,10, 0,0,-0,-0)
		Draw.Toggle("Material", 5, W["X"], W["Y"], W["W"], W["H"], Step[4], )
		
		#[Push]: Export
		W= Win(75,0,100,10, 0,0,-0,-0)
		Draw.PushButton("Export", 0, W["X"], W["Y"], W["W"], W["H"], )
		
		
		####################
		#     Node Tab     #
		####################
		if Tab=="Node":
			op= Option["Node"]
			
			#[Text]: Source type text
			TextSet(5,92,0,0 ,0,0,0)
			Draw.Text("Select node source type", "large")
			
			#[Toggle]: File
			W= Win(10,80,50,90, -0,-0,0,0)
			W["Bool"]=0
			if op["Mode"]=="File": W["Bool"]=1
			Draw.Toggle("File", 100, W["X"], W["Y"], W["W"], W["H"], W["Bool"], )
			
			#[Toggle]: Armature
			W= Win(50,80,90,90, -0,-0,0,0)
			W["Bool"]=0
			if op["Mode"]=="Armature": W["Bool"]=1
			Draw.Toggle("Armature", 101, W["X"], W["Y"], W["W"], W["H"], W["Bool"], )
			
			
			if op["Mode"] =="File":
				#[Text]: Select file
				TextSet(0,72,15,0 ,0,0,0)
				Draw.Text("Select file", "normal")
				
				#[Push]: Source file select
				W= Win(0,40,100,70, 10,0,-10,-0)
				Draw.PushButton("Source file select (tso)", 110, W["X"], W["Y"], W["W"], W["H"], )
				
				#[String]: Dir text box
				W= Win(0,20,50,40, 10,0,-1,-2)
				B["NodeSourceFileDir"]= Draw.String("", 111, W["X"], W["Y"], W["W"], W["H"], sys.dirname(op["File"] ), 200, )
				
				#[String]: Base text box
				W= Win(50,20,100,40, 1,0,-10,-2)
				B["NodeSourceFileBase"]= Draw.String("", 112, W["X"], W["Y"], W["W"], W["H"], sys.basename( op["File"] ), 200, )
			
			elif op["Mode"] =="Armature":
				#[Text]: Select armature
				TextSet(0,72,15,0 ,0,0,0)
				Draw.Text("Select armature", "normal")
				
				#[Menu]: Select Armature
				menu= "None %x-1"
				if len( BlenderData["Armature"] ) >0:
					menu= ""
					for f1 in xrange(len(BlenderData["Armature"])):
						menu+= "|" +BlenderData["Armature"][f1] +" %x" +str(f1)
					num= BlenderData["Armature"].index( op["Armature"] )
				else:
					num= -1
				W= Win(0,40,100,70, 10,0,-10,-0)
				B["SelectArmature"]= Draw.Menu(menu, 120, W["X"], W["Y"], W["W"], W["H"], num, )
				
				#[Push]: Quick select
				W= Win(0,20,100,40, 10,0,-10,-2)
				Draw.PushButton("Quick select", 121, W["X"], W["Y"], W["W"], W["H"], )
				
		####################
		#     Mesh Tab     #
		####################
		elif Tab=="Mesh":
			op= Option["Mesh"]
			
			#[Text]: Mesh menu
			TextSet(0,96,15,0 ,0,0,0)
			Draw.Text("Mesh menu", "large")
			
			#[Menu]: Mesh menu
			if len( op ) <1:
				menu= "None %x-1"
				index= -1
			else:
				menu= ""
				for f1 in xrange(len(op)):
					menu+= "|" +op[f1]["Name"] +" %x" +str(f1)
					if op[f1]["Name"] ==GuiState["ActiveMesh"]:
						index= f1
			W= Win(0,75,100,95, 10,0,-10,-0)
			B["ActiveMesh"]= Draw.Menu(menu, 200, W["X"], W["Y"], W["W"], W["H"], index, )
			
			#[Push]: New
			W= Win(0,65,34,75, 10,0,-0,-2)
			Draw.PushButton("New", 201, W["X"], W["Y"], W["W"], W["H"], )
			
			#[Push]: Replace
			W= Win(34,65,66,75, 0,0,-0,-2)
			Draw.PushButton("Replace", 202, W["X"], W["Y"], W["W"], W["H"], )
			
			#[Push]: Delete
			W= Win(66,65,100,75, 0,0,-10,-2)
			Draw.PushButton("Delete", 203, W["X"], W["Y"], W["W"], W["H"], )
			
			
			#[Text]: Mesh name
			TextSet(0,61,15,0 ,0,0,0)
			Draw.Text("Name", "normal")
			
			#[String]: MeshName text box
			W= Win(0,45,100,60, 10,0,-10,-0)
			B["MeshName"]= Draw.String("", 210, W["X"], W["Y"], W["W"], W["H"], GuiState["ActiveMeshName"], 200, )
			
			#[Toggle]: Object name copy
			W= Win(50,35,100,45, 0,2,-10,-2)
			Draw.Toggle("Object name copy", 211, W["X"], W["Y"], W["W"], W["H"], GuiState["ObjectNameCopy"], )
			
			
			#[Text]: Source object menu
			TextSet(0,36,15,0 ,0,0,0)
			Draw.Text("Source object menu", "normal")
			
			#[Menu]: Mesh object menu
			if len(BlenderData["Mesh"]) >0:
				menu= ""
				for f1 in xrange(len(BlenderData["Mesh"])):
					menu+= "|" +BlenderData["Mesh"][f1] +" %x" +str(f1)
				index= BlenderData["Mesh"].index( GuiState["ActiveMeshObject"] )
			else:
				menu= "None %x-1"
				index= -1
			W= Win(0,20,75,35, 10,0,-5,-0)
			B["MeshObject"]= Draw.Menu(menu, 220, W["X"], W["Y"], W["W"], W["H"], index, )
			
			#[Push]: Quick select
			W= Win(75,20,100,35, 0,0,-10,-0)
			Draw.PushButton("Quick select", 221, W["X"], W["Y"], W["W"], W["H"], )
			
		####################
		#   Texture Tab    #
		####################
		elif Tab=="Texture":
			op= Option["Texture"]
			
			#[Text]: Texture menu
			TextSet(0,96,15,0 ,0,0,0)
			Draw.Text("Texture menu", "large")
			
			#[Menu]: Texture menu
			if len( op ) <1:
				menu= "None %x-1"
				index= -1
			else:
				menu= ""
				for f1 in xrange(len(op)):
					menu+= "|" +op[f1]["Name"] +" %x" +str(f1)
					if op[f1]["Name"] ==GuiState["ActiveTexture"]:
						index= f1
			W= Win(0,75,100,95, 10,0,-10,-0)
			B["ActiveTexture"]= Draw.Menu(menu, 300, W["X"], W["Y"], W["W"], W["H"], index, )
			
			#[Push]: New
			W= Win(0,65,34,75, 10,0,-0,-2)
			Draw.PushButton("New", 301, W["X"], W["Y"], W["W"], W["H"], )
			
			#[Push]: Replace
			W= Win(34,65,66,75, 0,0,-0,-2)
			Draw.PushButton("Replace", 302, W["X"], W["Y"], W["W"], W["H"], )
			
			#[Push]: Delete
			W= Win(66,65,100,75, 0,0,-10,-2)
			Draw.PushButton("Delete", 303, W["X"], W["Y"], W["W"], W["H"], )
			
			
			#Image viewer
			if GuiState["ActiveTextureImage"] in BlenderData["Image"]:
				img= {}
				img["Image"]= Image.Get(GuiState["ActiveTextureImage"])
				img["W"], img["H"]= img["Image"].getMaxXY()
				W= Win(0,20,50,65, 10,0,-5,-5)
				if W["W"]/float(img["W"]) >=W["H"]/float(img["H"]):
					zoom= W["H"]/float(img["H"])
					W["X"]+= (-img["W"] *zoom +W["W"]) /2.0
				else:
					zoom= W["W"]/float(img["W"])
					W["Y"]+= (-img["H"] *zoom +W["H"]) /2.0
				Draw.Image(img["Image"], W["X"], W["Y"], zoom, zoom)
			
			
			#[Text]: Image menu
			TextSet(50,61,5,0 ,0,0,0)
			Draw.Text("Image menu", "normal")
			
			#[Menu]: Image menu
			if len(BlenderData["Image"]) >0:
				menu= ""
				for f1 in xrange(len(BlenderData["Image"])):
					menu+= "|" +BlenderData["Image"][f1] +" %x" +str(f1)
				index= BlenderData["Image"].index( GuiState["ActiveTextureImage"] )
			else:
				menu= "Node %x-1"
				index= -1
			W= Win(50,50,100,60, 0,0,-10,-0)
			B["TextureImage"]= Draw.Menu(menu, 310, W["X"], W["Y"], W["W"], W["H"], index, )
			
			
			#[Text]: Texture Name
			TextSet(50,46,5,0 ,0,0,0)
			Draw.Text("Texture Name", "normal")
			
			#[String]: TextureName text box
			W= Win(50,35,100,45, 0,0,-10,-0)
			B["TextureName"]= Draw.String("", 320, W["X"], W["Y"], W["W"], W["H"], GuiState["ActiveTextureName"], 200, )
			
			#[Toggle]: Image name copy
			W= Win(75,45,100,50, 0,2,-10,-2)
			Draw.Toggle("Image name copy", 321, W["X"], W["Y"], W["W"], W["H"], GuiState["ImageNameCopy"], )
			
			
			#[Text]: File Name
			TextSet(50,31,5,0 ,0,0,0)
			Draw.Text("File Name", "normal")
			
			#[String]: TextureFileName text box
			W= Win(50,20,80,30, 0,0,-2,-0)
			B["TextureFileName"]= Draw.String("", 330, W["X"], W["Y"], W["W"], W["H"], GuiState["ActiveTextureFileName"], 200, )
			
			#[Menu]: Ext menu
			menu= ""
			for f1 in xrange(len(ImageExt)):
				menu+= "|" +ImageExt[f1] +" %x" +str(f1)
			W= Win(80,20,100,30, 0,0,-10,-0)
			B["TextureFileExt"]= Draw.Menu(menu, 331, W["X"], W["Y"], W["W"], W["H"], ImageExt.index( GuiState["ActiveImageExt"] ), )
			
			#[Toggle]: Texture name copy
			W= Win(75,30,100,35, 0,2,-10,-2)
			Draw.Toggle("Texture name copy", 332, W["X"], W["Y"], W["W"], W["H"], GuiState["TextureNameCopy"], )
			
		####################
		#    Shader Tab    #
		####################
		elif Tab=="Shader":
			op= Option["Shader"]
			
			
			#[Text]: Shader menu
			TextSet(0,96,15,0 ,0,0,0)
			Draw.Text("Shader menu", "large")
			
			#[Menu]: Shader menu
			if len( op ) <1:
				menu= "None %x-1"
				index= -1
			else:
				menu= ""
				for f1 in xrange(len(op)):
					menu+= "|" +op[f1]["Name"] +" %x" +str(f1)
					if op[f1]["Name"] ==GuiState["ActiveShader"]:
						index= f1
			W= Win(0,75,50,95, 10,0,-1,-0)
			B["ActiveShader"]= Draw.Menu(menu, 400, W["X"], W["Y"], W["W"], W["H"], index, )
			
			
			#[Push]: New
			W= Win(0,65,34,75, 10,0,-0,-2)
			Draw.PushButton("New", 401, W["X"], W["Y"], W["W"], W["H"], )
			
			#[Push]: Replace
			W= Win(34,65,66,75, 0,0,-0,-2)
			Draw.PushButton("Replace", 402, W["X"], W["Y"], W["W"], W["H"], )
			
			#[Push]: Delete
			W= Win(66,65,100,75, 0,0,-10,-2)
			Draw.PushButton("Delete", 403, W["X"], W["Y"], W["W"], W["H"], )
			
			
			#[Text]: Shader name
			TextSet(50,96,5,0 ,0,0,0)
			Draw.Text("Shader name", "normal")
			
			#[String]: Shader Name
			W= Win(50,75,100,95, 1,0,-10,-0)
			B["ShaderName"]= Draw.String("", 404, W["X"], W["Y"], W["W"], W["H"], GuiState["ActiveShaderName"], 200, )
			
			
			#[Text]: Select source type
			TextSet(10,61,5,0 ,0,0,0)
			Draw.Text("Select source type", "normal")
			
			#[Toggle]: File
			W= Win(10,50,50,60, 0,0,-0,-0)
			W["Bool"]= 0
			if GuiState["ActiveShaderMode"]=="File": W["Bool"]=1
			Draw.Toggle("File", 410, W["X"], W["Y"], W["W"], W["H"], W["Bool"], )
			
			#[Toggle]: Blender Text
			W= Win(50,50,90,60, 0,0,-0,-0)
			W["Bool"]= 0
			if GuiState["ActiveShaderMode"]=="Text": W["Bool"]=1
			Draw.Toggle("Blender Text", 411, W["X"], W["Y"], W["W"], W["H"], W["Bool"], )
			
			
			if GuiState["ActiveShaderMode"]=="File":
				
				#[Text]: Select file
				TextSet(0,46,15,0 ,0,0,0)
				Draw.Text("Select file", "normal")
				
				#[Push]: File selector
				W= Win(0,30,100,45, 10,0,-10,-0)
				Draw.PushButton("File selector ( Text file )", 420, W["X"], W["Y"], W["W"], W["H"], )
				
				#[String]: Dir text box
				W= Win(0,20,50,30, 10,0,-1,-2)
				B["ShaderFileDir"]= Draw.String("", 421, W["X"], W["Y"], W["W"], W["H"], sys.dirname( GuiState["ActiveShaderFile"] ), 200, )
				
				#[String]: Base text box
				W= Win(50,20,100,30, 1,0,-10,-2)
				B["ShaderFileBase"]= Draw.String("", 422, W["X"], W["Y"], W["W"], W["H"], sys.basename( GuiState["ActiveShaderFile"] ), 200, )
			
			elif GuiState["ActiveShaderMode"]=="Text":
				
				#[Text]: Text menu
				TextSet(0,46,15,0 ,0,0,0)
				Draw.Text("Text menu", "normal")
				
				#[Menu]: Text menu
				menu= "None %x-1"
				if len( BlenderData["Text"] ) >0:
					menu= ""
					for f1 in xrange(len(BlenderData["Text"])):
						menu+= "|" +BlenderData["Text"][f1] +" %x" +str(f1)
					sel= BlenderData["Text"].index( GuiState["ActiveShaderText"] )
				else: sel= -1
				W= Win(0,20,100,45, 10,0,-10,-0)
				B["ActiveShaderText"]= Draw.Menu(menu, 430, W["X"], W["Y"], W["W"], W["H"], sel, )
				
				
		####################
		#   Material Tab   #
		####################
		elif Tab=="Material":
			op= Option["Material"]
			
			mesh_mate= []
			for f1 in Option["Mesh"]:
				mesh= Object.Get( f1["Name"] ).getData(False, True)
				for f2 in mesh.materials:
					if not f2 in mesh_mate:
						mesh_mate.append(f2)
			for f1 in Option["Material"].keys():
				for f2 in mesh_mate:
					if f2.name ==f1:
						break
				else:
					del Option["Material"][f1]
			for f1 in mesh_mate:
				if not Option["Material"].has_key(f1.name):
					Option["Material"][f1.name]= { "Name":f1.name, "Mode":"Text", "File":"C:/file.txt", "Text":None }
					if len(BlenderData["Text"]) >0:
						Option["Material"][f1.name]["Text"]= BlenderData["Text"][0]
			
			#[Text]: Material menu
			TextSet(0,96,15,0 ,0,0,0)
			Draw.Text("Material menu", "large")
			
			if len( Option["Material"] ) <1:
				menu= "None %x-1"
				W= Win(0,75,100,95, 10,0,-10,-0)
				Draw.Menu(menu, 1000, W["X"], W["Y"], W["W"], W["H"], -1, )
			else:
				if not Option["Material"].has_key( GuiState["ActiveMaterial"] ):
					GuiState["ActiveMaterial"]= list( Option["Material"] )[0]
				menu= ""
				for f1 in xrange(len(Option["Material"])):
					menu+= "|" +list(Option["Material"])[f1] +" %x" +str(f1)
				W= Win(0,75,100,95, 10,0,-10,-0)
				B["ActiveMaterial"]= Draw.Menu(menu, 500, W["X"], W["Y"], W["W"], W["H"], list(Option["Material"]).index(GuiState["ActiveMaterial"]), )
				
				
				#[String]: Shader Name
				W= Win(0,65,100,75, 10,0,-10,-2)
				B["MaterialName"]= Draw.String("ShaderName: ", 501, W["X"], W["Y"], W["W"], W["H"], Option["Material"][GuiState["ActiveMaterial"]]["Name"], 200, )
				
				
				#[Text]: Select source type
				TextSet(10,61,5,0 ,0,0,0)
				Draw.Text("Select source type", "normal")
				
				#[Toggle]: File
				W= Win(10,50,50,60, 0,0,-0,-0)
				W["Bool"]= 0
				if Option["Material"][GuiState["ActiveMaterial"]]["Mode"]=="File": W["Bool"]=1
				Draw.Toggle("File", 510, W["X"], W["Y"], W["W"], W["H"], W["Bool"], )
				
				#[Toggle]: Blender Text
				W= Win(50,50,90,60, 0,0,-0,-0)
				W["Bool"]= 0
				if Option["Material"][GuiState["ActiveMaterial"]]["Mode"]=="Text": W["Bool"]=1
				Draw.Toggle("Blender Text", 511, W["X"], W["Y"], W["W"], W["H"], W["Bool"], )
				
				
				if Option["Material"][GuiState["ActiveMaterial"]]["Mode"]=="File":
					
					#[Text]: Select file
					TextSet(0,46,15,0 ,0,0,0)
					Draw.Text("Select file", "normal")
					
					#[Push]: File selector
					W= Win(0,30,100,45, 10,0,-10,-0)
					Draw.PushButton("File selector ( Text file )", 520, W["X"], W["Y"], W["W"], W["H"], )
					
					#[String]: Dir text box
					W= Win(0,20,50,30, 10,0,-1,-2)
					B["MaterialFileDir"]= Draw.String("", 521, W["X"], W["Y"], W["W"], W["H"], sys.dirname( Option["Material"][GuiState["ActiveMaterial"]]["File"] ), 200, )
					
					#[String]: Base text box
					W= Win(50,20,100,30, 1,0,-10,-2)
					B["MaterialFileBase"]= Draw.String("", 522, W["X"], W["Y"], W["W"], W["H"], sys.basename( Option["Material"][GuiState["ActiveMaterial"]]["File"] ), 200, )
				
				elif Option["Material"][GuiState["ActiveMaterial"]]["Mode"]=="Text":
					
					#[Text]: Text menu
					TextSet(0,46,15,0 ,0,0,0)
					Draw.Text("Text menu", "normal")
					
					#[Menu]: Text menu
					menu= "None %x-1"
					if len( BlenderData["Text"] ) >0:
						menu= ""
						for f1 in xrange(len(BlenderData["Text"])):
							menu+= "|" +BlenderData["Text"][f1] +" %x" +str(f1)
						sel= BlenderData["Text"].index( Option["Material"][GuiState["ActiveMaterial"]]["Text"] )
					else: sel= -1
					W= Win(0,20,100,45, 10,0,-10,-0)
					B["MaterialText"]= Draw.Menu(menu, 530, W["X"], W["Y"], W["W"], W["H"], sel, )
					
		####################
		#    Export Tab    #
		####################
		elif Tab=="Export":
			
			#[Text]: Select file
			TextSet(0,96,15,0 ,0,0,0)
			Draw.Text("Select Output", "large")
			
			#[Push]: File selector
			W= Win(0,80,100,95, 10,0,-10,-0)
			Draw.PushButton("File selector ( Output )", 600, W["X"], W["Y"], W["W"], W["H"], )
			
			#[String]: Dir text box
			W= Win(0,70,50,80, 10,0,-1,-2)
			B["OutputDir"]= Draw.String("", 601, W["X"], W["Y"], W["W"], W["H"], sys.dirname( Option["Export"]["Path"] ), 200, )
			
			#[String]: Base text box
			W= Win(50,70,100,80, 1,0,-10,-2)
			B["OutputBase"]= Draw.String("", 602, W["X"], W["Y"], W["W"], W["H"], sys.basename( Option["Export"]["Path"] ), 200, )
			
			
			#[String]: Error text box
			if GuiState["ERROR"] !="":
				#[Text]: Error
				TextSet(0,31,15,0 ,0.8,0,0)
				Draw.Text("ERROR", "large")
				
				W= Win(0,20,100,30, 10,0,-10,-0)
				Draw.String("", 1000, W["X"], W["Y"], W["W"], W["H"], GuiState["ERROR"], 200, )
			
			
			
			#[Text]: One more V
			TextSet(88,11,-65,0 ,0.8,0,0)
			Draw.Text("One more V", "normal")
		
	##
	####################
	
	global Option, Tab, GuiState
	
	BlenderData= GetData()
	
	Tab= "Node"
	ImageExt= [ ".bmp", ".tga", "" ]
	
	## GuiState reset
	GuiState= {}
	#Mesh
	GuiState["ActiveMesh"]= ""
	GuiState["ActiveMeshName"]= ""
	GuiState["ActiveMeshObject"]= None
	if len( BlenderData["Mesh"] ) >0:
		GuiState["ActiveMeshName"]= BlenderData["Mesh"][0]
		GuiState["ActiveMeshObject"]= BlenderData["Mesh"][0]
	GuiState["ObjectNameCopy"]= 1
	#Texture
	GuiState["ActiveTexture"]= ""
	GuiState["ActiveTextureName"]= ""
	GuiState["ActiveTextureFileName"]= ""
	GuiState["ActiveTextureImage"]= None
	if len( BlenderData["Image"] ) >0:
		GuiState["ActiveTextureName"]= BlenderData["Image"][0]
		GuiState["ActiveTextureFileName"]= BlenderData["Image"][0]
		GuiState["ActiveTextureImage"]= BlenderData["Image"][0]
	GuiState["ActiveImageExt"]= ImageExt[0]
	GuiState["ImageNameCopy"]= 1
	GuiState["TextureNameCopy"]= 1
	#Shader
	GuiState["ActiveShader"]= ""
	GuiState["ActiveShaderName"]= "TAToonshade_050.cgfx"
	GuiState["ActiveShaderMode"]= "Text"
	GuiState["ActiveShaderFile"]= "C:/file.txt"
	GuiState["ActiveShaderText"]= None
	if len( BlenderData["Text"] ) >0:
		GuiState["ActiveShaderText"]= BlenderData["Text"][0]
	#Material
	GuiState["ActiveMaterial"]= ""
	GuiState["ActiveMaterialName"]= ""
	GuiState["ActiveMaterialMode"]= "Text"
	GuiState["ActiveMaterialFile"]= "C:/file.txt"
	if len( BlenderData["Text"] ) >0:
		GuiState["ActiveMaterialText"]= BlenderData["Text"][0]
	
	GuiState["ERROR"]= ""
	
	# Button dict
	B= {}
	
	
	#Option set
	Option= {}
	Option["Node"]= { "Mode":"File", "File":"C:/file.tso", "Armature":None, }
	if len( BlenderData["Armature"] ) >0: Option["Node"]["Armature"]= BlenderData["Armature"][0]
	Option["Mesh"]= []
	Option["Texture"]= []
	Option["Shader"]= []
	Option["Material"]= {}
	Option["Export"]= { "Path":"C:/output.tso" }
	
	
	#Registry read
	rdict= Registry.GetKey("tso_export", True)
	if rdict:
		try:
			if rdict["Tab"] in ["Node", "Mesh", "Texture", "Shader", "Material", "Export"]:
				Tab= rdict["Tab"]
			Opt= rdict["Option"]
			state= rdict["GuiState"]
		except:
			update_Registry()
	
	
	#######################
	# Registry data check #
	#######################
	#Node
	try:
		op= Opt["Node"]
	except: pass
	try:
		if op["Mode"] in ["File", "Armature"]:
			Option["Node"]["Mode"]= op["Mode"]
		if isinstance(op["File"], str):
			Option["Node"]["File"]= op["File"]
	except: pass
	try:
		if op["Armature"] in BlenderData["Armature"]:
			Option["Node"]["Armature"]= op["Armature"]
	except: pass
	
	#Mesh
	try:
		op= Opt["Mesh"]
	except: pass
	try:
		for f1 in op:
			try:
				if isinstance(f1["Name"], str):
					name= f1["Name"]
					if isinstance(f1["Object"], str):
						obj= f1["Object"]
						if obj in BlenderData["Mesh"]:
							Option["Mesh"].append({ "Name":name, "Object":obj })
			except: pass
		
		for f1 in Option["Mesh"]:
			if f1["Name"] ==state["ActiveMesh"]:
				GuiState["ActiveMesh"]= f1["Name"]
				break
		else:
			GuiState["ActiveMesh"]= Option["Mesh"][-1]["Name"]
	except: pass
	try:
		if isinstance(state["ObjectNameCopy"], int):
			GuiState["ObjectNameCopy"]= state["ObjectNameCopy"]
		if isinstance(state["ActiveMeshName"], str):
			GuiState["ActiveMeshName"]= state["ActiveMeshName"]
		if state["ActiveMeshObject"] in BlenderData["Mesh"]:
			GuiState["ActiveMeshObject"]= state["ActiveMeshObject"]
	except: pass
	
	#Texture
	try:
		op= Opt["Texture"]
	except: pass
	try:
		for f1 in op:
			try:
				if isinstance(f1["Name"], str):
					name= f1["Name"]
					if isinstance(f1["FileName"], str):
						filename= f1["FileName"]
						if f1["Image"] in BlenderData["Image"]:
							image= f1["Image"]
							if f1["Ext"] in ImageExt:
								ext= f1["Ext"]
								Option["Texture"].append({ "Name":name, "FileName":name, "Image":image, "Ext":ext })
			except: pass
		for f1 in Option["Texture"]:
			if f1["Name"] ==state["ActiveTexture"]:
				GuiState["ActiveTexture"]= f1["Name"]
				break
		else:
			GuiState["ActiveTexture"]= Option["Texture"][-1]["Name"]
	except: pass
	try:
		if isinstance(state["ActiveTextureName"], str):
			GuiState["ActiveTextureName"]= state["ActiveTextureName"]
		if isinstance(state["ActiveTextureFileName"], str):
			GuiState["ActiveTextureFileName"]= state["ActiveTextureFileName"]
		if state["ActiveTextureImage"] in BlenderData["Image"]:
			GuiState["ActiveTextureImage"]= state["ActiveTextureImage"]
		if state["ActiveImageExt"] in ImageExt:
			GuiState["ActiveImageExt"]= state["ActiveImageExt"]
		if isinstance(state["ImageNameCopy"], int):
			GuiState["ImageNameCopy"]= state["ImageNameCopy"]
		if isinstance(state["TextureNameCopy"], int):
			GuiState["TextureNameCopy"]= state["TextureNameCopy"]
	except: pass
	
	#Shader
	try:
		op= Opt["Shader"]
	except: pass
	try:
		for f1 in op:
			try:
				if isinstance(f1["Name"], str):
					name= f1["Name"]
					if f1["Mode"] in ["File", "Text"]:
						mode= f1["Mode"]
						if isinstance(f1["File"], str):
							file= f1["File"]
							if f1["Text"] in BlenderData["Text"]:
								text= f1["Text"]
								Option["Shader"].append({ "Name":name, "Mode":mode, "File":file, "Text":text })
			except: pass
		for f1 in Option["Shader"]:
			if f1["Name"] ==state["ActiveShader"]:
				GuiState["ActiveShader"]= f1["Name"]
				break
		else:
			GuiState["ActiveShader"]= Option["Shader"][-1]["Name"]
	except: pass
	try:
		if isinstance(state["ActiveShaderName"], str):
			GuiState["ActiveShaderName"]= state["ActiveShaderName"]
		if state["ActiveShaderMode"] in ["File", "Text"]:
			GuiState["ActiveShaderMode"]= state["ActiveShaderMode"]
		if isinstance(state["ActiveShaderFile"], str):
			GuiState["ActiveShaderFile"]= state["ActiveShaderFile"]
		if state["ActiveShaderText"] in BlenderData["Text"]:
			GuiState["ActiveShaderText"]= state["ActiveShaderText"]
	except: pass
	
	#Material
	try:
		op= Opt["Material"]
		mesh_mate= []
		for f1 in Option["Mesh"]:
			mesh= Object.Get( f1["Name"] ).getData(False, True)
			for f2 in mesh.materials:
				if not f2 in mesh_mate:
					mesh_mate.append(f2.name)
		for f1, f2 in op.iteritems():
			try:
				if f1 in mesh_mate:
					if isinstance(f2["Name"], str):
						name= f2["Name"]
						if f2["Mode"] in ["File", "Text"]:
							mode= f2["Mode"]
							if isinstance(f2["File"], str):
								file= f2["File"]
								if f2["Text"] in BlenderData["Text"]:
									text= f2["Text"]
									Option["Material"][f1]= { "Name":name, "Mode":mode, "File":file, "Text":text }
			except: pass
		for f1 in Option["Material"].keys():
			if f1 ==state["ActiveMaterial"]:
				GuiState["ActiveMaterial"]= f1
				break
		else:
			GuiState["ActiveMaterial"]= Option["Material"].keys()[0]
	except: pass
	
	#Export
	try:
		if isinstance(Opt["Export"]["Path"], str):
			Option["Export"]["Path"]= Opt["Export"]["Path"]
	except: pass
	
	
	# Draw start
	Draw.Register(GuiDraw,Event,Button)
	

if __name__ =="__main__":
	#GUI start
	Gui()

# vim: set sw=4 ts=4:
