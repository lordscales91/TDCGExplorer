#!/usr/bin/env python
from struct import unpack

def read_cstring(file):
	str = ''
	while True:
		c = file.read(1)
		if c == chr(0x00):
			break
		str += c
	return str

def read_int(file):
	return unpack('i', file.read(4))[0]

def read_matrix4(file):
	return unpack('16f', file.read(64))

def read_vector3(file):
	return unpack('3f', file.read(12))

def read_vector2(file):
	return unpack('2f', file.read(8))

class TSONode(object):
	def __init__(self):
		self.name = None
		self.parent_name = None
		self.path = None
		self.transform = None

	def read(self, file):
		path = read_cstring(file)
		# names = path.split('|')
		self.name = path.rpartition('|')[-1]
		self.parent_name = path.rsplit('|')[-2]
		self.path = path

class TSOTex(object):
	def __init__(self):
		self.name = None
		self.path = None
		self.width = 0
		self.height = 0
		self.depth = 0
		self.data = None

	def read(self, file):
		self.name = read_cstring(file)
		self.path = read_cstring(file)
		width, height, depth = unpack('3i', file.read(12))
		self.width = width
		self.height = height
		self.depth = depth
		self.data = file.read(width * height * depth)

class TSOScript(object):
	def __init__(self):
		self.name = None
		self.lines = []

	def read(self, file):
		self.name = read_cstring(file)
		lines_count = read_int(file)
		del self.lines[:]
		for x in xrange(lines_count):
			line = read_cstring(file)
			self.lines.append(line)

class TSOSubScript(object):
	def __init__(self):
		self.name = None
		self.path = None
		self.lines = []

	def read(self, file):
		self.name = read_cstring(file)
		self.path = read_cstring(file)
		lines_count = read_int(file)
		del self.lines[:]
		for x in xrange(lines_count):
			line = read_cstring(file)
			self.lines.append(line)

class Vertex(object):
	def __init__(self):
		self.co = None
		self.no = None
		self.uv = None
		self.skin_weights = []

	def read(self, file):
		self.co = read_vector3(file)
		self.no = read_vector3(file)
		self.uv = read_vector2(file)
		skin_weights_count = read_int(file)
		del self.skin_weights[:]
		for w in xrange(skin_weights_count):
			skin_weight = unpack('if', file.read(8))
			self.skin_weights.append(skin_weight)

class TSOSubMesh(object):
	def __init__(self):
		self.spec = None
		self.bone_indices = []
		self.vertices = []

	def read(self, file):
		self.spec = read_int(file)

		bone_indices_count = read_int(file)
		del self.bone_indices[:]
		for i in xrange(bone_indices_count):
			bone_index = read_int(file)
			self.bone_indices.append(bone_index)

		vertices_count = read_int(file)
		del self.vertices[:]
		for v in xrange(vertices_count):
			vertex = Vertex()
			vertex.read(file)
			self.vertices.append(vertex)

class TSOMesh(object):
	def __init__(self):
		self.name = None
		self.sub_meshes = []

	def read(self, file):
		self.name = read_cstring(file)
		m = read_matrix4(file)
		unknown1 = read_int(file)
		sub_meshes_count = unpack('i', file.read(4))[0]
		del self.sub_meshes[:]
		for sub_mesh_idx in xrange(sub_meshes_count):
			sub_mesh = TSOSubMesh()
			sub_mesh.read(file)
			self.sub_meshes.append(sub_mesh)

class TSOFile(object):
	def __init__(self):
		self.nodes	= []
		self.textures	= []
		self.scripts	= []
		self.sub_scripts	= []
		self.meshes	= []

	def load(self, source_file):
		file = open(source_file, 'rb')
		file.read(4) # 'TSO1'

		nodes_count = read_int(file)
		del self.nodes[:]
		for i in xrange(nodes_count):
			node = TSONode()
			node.read(file)
			self.nodes.append(node)

		matrices_count = read_int(file)
		for i in xrange(matrices_count):
			node = self.nodes[i]
			node.transform = read_matrix4(file)

		textures_count = read_int(file)
		del self.textures[:]
		for i in xrange(textures_count):
			tex = TSOTex()
			tex.read(file)
			self.textures.append(tex)

		scripts_count = read_int(file)
		del self.scripts[:]
		for i in xrange(scripts_count):
			script = TSOScript()
			script.read(file)
			self.scripts.append(script)

		sub_scripts_count = read_int(file)
		del self.sub_scripts[:]
		for i in xrange(sub_scripts_count):
			sub_script = TSOSubScript()
			sub_script.read(file)
			self.sub_scripts.append(sub_script)

		meshes_count = read_int(file)
		del self.meshes[:]
		for mesh_idx in xrange(meshes_count):
			mesh = TSOMesh()
			mesh.read(file)
			self.meshes.append(mesh)

# vim: set sw=4 ts=4:
