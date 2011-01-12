#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../Debug")
require 'TDCG'
require 'TDCG.NvTriStrip'

require 'unified_position_texcoord_vertex'
require 'unified_position_spec_vertex'
require 'tsoface'

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
        f = TSOFace.new(a, b, c)
        faces.push f
      end
    end
  end
  faces
end

WEIGHT_EPSILON = Float::EPSILON # or 1.0e-4

puts "Sub meshes:"
puts "  vertices bone_indices"
puts "  -------- ------------"
for sub in selected_mesh.sub_meshes
  puts sprintf("  %8d %12d", sub.vertices.size, sub.bone_indices.size)
end

print "Set max palettes: "
line = gets
max_palettes = line.chomp.to_i
max_palettes = 16 if max_palettes == 0

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

def create_sub_meshes(faces, max_palettes)
  faces_1 = faces
  faces_2 = []

  subs = []

  puts "  vertices bone_indices"
  puts "  -------- ------------"

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
          if bmap.size == max_palettes
            valid = false
            break
          end
          tmap[sw.bone_index] = true
          if bmap.size + tmap.size > max_palettes
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

    # puts "#vert_indices:#{ vert_indices.size }"
    vert_indices_ary = System::Array[System::UInt16].new(vert_indices.size)
    vert_indices.each_with_index do |vidx, i|
      vert_indices_ary[i] = vidx
    end

    optimized_indices = TDCG::NvTriStrip.optimize(vert_indices_ary)
    # puts "#optimized_indices:#{ optimized_indices.size }"

    sub = TDCG::TSOSubMesh.new
    sub.spec = spec

    # puts "#bone_indices:#{ bone_indices.size }"
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

    puts sprintf("  %8d %12d", sub.vertices.size, sub.bone_indices.size)

    subs.push(sub)

    t = faces_1
    faces_1 = faces_2
    faces_2 = t
    t.clear
  end
  subs
end

def rebuild_mesh(mesh, max_palettes)
  faces = create_faces(mesh)
  # puts "#uniq faces:#{ faces.size }"

  subs = create_sub_meshes(faces, max_palettes)
  # puts "#subs:#{ subs.size }"

  subs_ary = System::Array[TDCG::TSOSubMesh].new(subs.size)
  subs.each_with_index do |sub, i|
    subs_ary[i] = sub
  end
  mesh.sub_meshes = subs_ary
end

rebuild_mesh(selected_mesh, max_palettes)
tso.save('out.tso')
