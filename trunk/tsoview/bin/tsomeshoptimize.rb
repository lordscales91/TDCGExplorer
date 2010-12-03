#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../Debug")
require 'TDCG'
require 'TDCG.NvTriStrip'

if ARGV.size < 1
  puts "tsomeshoptimize <tso file>"
  exit
end
source_file = ARGV[0]

tso = TDCG::TSOFile.new
tso.load(source_file)

puts "Meshes:"
tso.meshes.each_with_index do |mesh, i|
  puts sprintf("%d %s", i, mesh.name)
end

print "Select mesh (0-#{tso.meshes.size - 1}): "
line = gets
mesh_idx = line.chomp.to_i
mesh_idx = 0
selected_mesh = tso.meshes[mesh_idx]

nodemap = {}
for node in tso.nodes
  nodemap[node.name.to_s] = node
end

class UnifiedPositionTexcoordVertex < TDCG::Vertex
  def eql?(o)
    position == o.position && u == o.u && v == o.v
  end
  def hash
    position.hash ^ u.hash ^ v.hash
  end
end

class UnifiedPositionSpecVertex < TDCG::Vertex
  attr :spec
  def initialize(a, sub)
    self.position = a.position
    self.normal = a.normal
    self.u = a.u
    self.v = a.v
    self.skin_weights = System::Array[TDCG::SkinWeight].new(4)
    4.times do |i|
      self.skin_weights[i] = TDCG::SkinWeight.new(sub.bone_indices[a.skin_weights[i].bone_index], a.skin_weights[i].weight)
    end
    @spec = sub.spec
  end
  def eql?(o)
    # position == o.position && spec == o.spec
    position.x == o.position.x && position.y == o.position.y && position.z == o.position.z && spec == o.spec
  end
  def hash
    # position.hash ^ spec.hash
    position.x.hash ^ position.y.hash ^ position.z.hash ^ spec.hash
  end
end

class Face
  attr :a
  attr :b
  attr :c
  attr_accessor :spec
  def initialize(a, b, c, spec)
    @a = a
    @b = b
    @c = c
    @spec = spec
  end

  def vertices
    [a, b, c]
  end

  def inspect
    "Face(spec:#{ spec })"
  end
end

# tso2mqo
def create_faces(mesh)
  faces = []
  for sub in mesh.sub_meshes
    vertices = []
    for a in sub.vertices
      v = UnifiedPositionSpecVertex.new(a, sub)
      vertices.push(v)
    end
    for i in 2...vertices.size
      if i % 2 != 0
        a = vertices[i-2]
        b = vertices[i-0]
        c = vertices[i-1]
      else
        a = vertices[i-2]
        b = vertices[i-1]
        c = vertices[i-0]
      end
      if !a.eql?(b) && !b.eql?(c) && !c.eql?(a)
        f = Face.new(a, b, c, sub.spec)
        faces.push f
      end
    end
  end
  faces
end

WEIGHT_EPSILON = Float::EPSILON # or 1.0e-4
MAX_PALETTES = 16

def create_vertex(v, bmap)
  a = UnifiedPositionTexcoordVertex.new
  a.position = v.position
  a.skin_weights = System::Array[TDCG::SkinWeight].new(4)
  4.times do |i|
    a.skin_weights[i] =
      if v.skin_weights[i].weight < WEIGHT_EPSILON
        TDCG::SkinWeight.new(0, 0.0)
      else
        TDCG::SkinWeight.new(bmap[v.skin_weights[i].bone_index], v.skin_weights[i].weight)
      end
  end
  a.generate_bone_indices
  a.normal = v.normal
  a.u = v.u
  a.v = v.v
  a
end

# mqo2tso
def create_sub_meshes(faces)
  faces_1 = faces
  faces_2 = []

  subs = []

  until faces_1.empty?
    spec = faces_1[0].spec
    bmap = {}
    bone_indices = []
    vmap = {}
    vertices = []
    vert_indices = []

    for f in faces_1
      if f.spec != spec
        faces_2.push(f)
        next
      end

      valid = true
      tmap = {}
      for v in f.vertices
        for sw in v.skin_weights
          next if sw.weight < WEIGHT_EPSILON
          next if bmap[sw.bone_index]
          if bmap.size == MAX_PALETTES
            valid = false
            break
          end
          tmap[sw.bone_index] = true
          if bmap.size + tmap.size > MAX_PALETTES
            valid = false
            break
          end
        end
      end

      unless valid
        faces_2.push(f)
        next
      end

      for bone_index in tmap.keys
        bmap[bone_index] = bone_indices.size
        bone_indices.push(bone_index)
      end

      for v in f.vertices
        a = create_vertex(v, bmap)
        unless vmap[a]
          vmap[a] = vertices.size
          vertices.push(a)
        end
        vert_indices.push(vmap[a])
      end
    end

    puts "#vert_indices:#{ vert_indices.size }"
    vert_indices_ary = System::Array[System::UInt16].new(vert_indices.size)
    vert_indices.each_with_index do |vidx, i|
      vert_indices_ary[i] = vidx
    end

    optimized_indices = TDCG::NvTriStrip.optimize(vert_indices_ary)
    puts "#optimized_indices:#{ optimized_indices.size }"

    sub = TDCG::TSOSubMesh.new
    sub.spec = spec

    puts "#bone_indices:#{ bone_indices.size }"
    bone_indices_ary = System::Array[System::Int32].new(bone_indices.size)
    bone_indices.each_with_index do |bidx, i|
      bone_indices_ary[i] = bidx
    end
    sub.bone_indices = bone_indices_ary

    vertices_ary = System::Array[UnifiedPositionTexcoordVertex].new(optimized_indices.size)
    optimized_indices.each_with_index do |vidx, i|
      vertices_ary[i] = vertices[vidx]
    end
    sub.vertices = vertices_ary

    subs.push(sub)

    t = faces_1
    faces_1 = faces_2
    faces_2 = t
    t.clear
  end
  subs
end

def main(mesh)
  puts "#sub_meshes:#{ mesh.sub_meshes.size }"
  faces = create_faces(mesh)
  # faces.sort!

  puts "#uniq faces:#{ faces.size }"
  puts

  subs = create_sub_meshes(faces)

  puts "#subs:#{ subs.size }"
  subs_ary = System::Array[TDCG::TSOSubMesh].new(subs.size)
  subs.each_with_index do |sub, i|
    subs_ary[i] = sub
  end
  mesh.sub_meshes = subs_ary
end

main(selected_mesh)
tso.save('out.tso')
