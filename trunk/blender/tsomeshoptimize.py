#!/usr/bin/env python
import nvtristrip
from tdcg import TSOFile, TSOSubMesh
import sys

if len(sys.argv) < 2:
	print "tsomeshoptimize <tso file>"
	quit()

source_file = sys.argv[1]

tso = TSOFile()
tso.load(source_file)

print "Meshes:"
for i, mesh in enumerate(tso.meshes):
	print i, mesh.name

mesh_idx = 5
selected_mesh = tso.meshes[mesh_idx]

max_palettes = 12

print "Sub meshes:"
print "  vertices bone_indices"
print "  -------- ------------"
for sub in selected_mesh.sub_meshes:
	print "  %8d %12d" % (len(sub.vertices), len(sub.bone_indices))

class UnifiedPositionSpecVertex(object):
	def __init__(self, a, bone_indices, spec):
		self.co = a.co
		self.no = a.no
		self.uv = a.uv
		self.skin_weights = []
		for bone_index, weight in a.skin_weights:
			self.skin_weights.append(( bone_indices[bone_index], weight ))
		self.spec = spec

	def __eq__(self, v):
		return self.co == v.co and self.spec == v.spec

	def __hash__(self):
		return hash(self.co) ^ hash(self.spec)

class UnifiedPositionTexcoordVertex(object):
	WEIGHT_EPSILON = 1.0e-4

	def __init__(self, v, bone_indices_map):
		self.co = v.co
		self.no = v.no
		self.uv = v.uv
		self.skin_weights = []
		for bone_index, weight in v.skin_weights:
			if weight < WEIGHT_EPSILON:
				self.skin_weights.append(( 0, 0.0 ))
			else:
				self.skin_weights.append(( bone_indices_map[bone_index], weight ))

	def __eq__(self, v):
		return self.co == v.co and self.uv == v.uv

	def __hash__(self):
		return hash(self.co) ^ hash(self.uv)

class TSOFace(object):
	def __init__(self, a, b, c):
		self.a = a
		self.b = b
		self.c = c
		self.spec = a.spec
	
	def vertices(self):
		return ( self.a, self.b, self.c )

def create_faces(mesh):
	faces = []
	for sub in mesh.sub_meshes:
		vertices = [ UnifiedPositionSpecVertex(a, sub.bone_indices, sub.spec) for a in sub.vertices ]
		for i in xrange(2, len(vertices)):
			if i % 2 != 0:
				a = vertices[i-2]
				b = vertices[i-0]
				c = vertices[i-1]
			else:
				a = vertices[i-2]
				b = vertices[i-1]
				c = vertices[i-0]
			if a == b or b == c or c == a:
				continue
			faces.append( TSOFace(a, b, c) )

	return faces

WEIGHT_EPSILON = 1.0e-4

def create_sub_meshes(faces, max_palettes):
	faces_1 = faces
	faces_2 = []

	subs = []

	print "  vertices bone_indices"
	print "  -------- ------------"

	while len(faces_1) != 0:
		spec = faces_1[0].spec
		bmap = {}
		bone_indices = []
		vmap = {}
		vertices = []
		vert_indices = []

		for f in faces_1:
			if f.spec != spec:
				faces_2.append(f)
				continue

			valid = True
			bset = set()
			for v in f.vertices():
				for bone_index, weight in v.skin_weights:
					if weight < WEIGHT_EPSILON:
						continue
					if bone_index in bmap:
						continue
					if len(bmap) == max_palettes:
						valid = False
						break
					bset.add(bone_index)
					if len(bmap) + len(bset) > max_palettes:
						valid = False
						break

			if not valid:
				faces_2.append(f)
				continue

			for bone_index in bset:
				bmap[bone_index] = len(bone_indices)
				bone_indices.append(bone_index)

			for v in f.vertices():
				a = UnifiedPositionTexcoordVertex(v, bmap)
				if a not in vmap:
					vmap[a] = len(vertices)
					vertices.append(a)
				vert_indices.append(vmap[a])

		# print '#vert_indices', len(vert_indices)
		optimized_indices = nvtristrip.optimize(vert_indices)
		# print '#optimized_indices', len(optimized_indices)

		sub = TSOSubMesh()
		sub.spec = spec

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

def rebuild_mesh(mesh, max_palettes):
	faces = create_faces(mesh)
	print '#uniq faces', len(faces)

	subs = create_sub_meshes(faces, max_palettes)
	print '#subs', len(subs)

	mesh.sub_meshes = subs

rebuild_mesh(selected_mesh, max_palettes)

# vim: set sw=4 ts=4:
