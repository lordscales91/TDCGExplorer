#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'
require 'uniq_vertex'
require 'uniq_cell'
require 'cluster'

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../Debug")
require 'TDCG'

if ARGV.size < 1
  puts "tsoweightcopy <tso file>"
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
selected_mesh = tso.meshes[mesh_idx]

nodemap = {}
for node in tso.nodes
  nodemap[node.name.to_s] = node
end

oppnode_idmap = {}
open('flipnodes.txt') do |f|
  while line = f.gets
    tokens = line.chomp.split(/ /)
    op = tokens[0]
    case op
    when 'flip'
      cnode_name = tokens[1]
      cnode_id = nodemap[cnode_name].ID
      oppnode_idmap[cnode_id] = cnode_id
    when 'swap'
      lnode_name = tokens[1]
      rnode_name = tokens[2]
      lnode_id = nodemap[lnode_name].ID
      rnode_id = nodemap[rnode_name].ID
      oppnode_idmap[lnode_id] = rnode_id
      oppnode_idmap[rnode_id] = lnode_id
    end
  end
end

def main(mesh)
  min = Vector3.empty
  max = Vector3.empty
  nvertices = 0

  # p mesh.sub_meshes.size
  for sub in mesh.sub_meshes
    # p sub.bone_indices.size
    # p sub.vertices.size
    for v in sub.vertices
      x = v.position.x
      y = v.position.y
      z = v.position.z

      min.x = x if min.x > x
      min.y = y if min.y > y
      min.z = z if min.z > z

      max.x = x if max.x < x
      max.y = y if max.y < y
      max.z = z if max.z < z

      nvertices += 1
    end
  end
  puts "#vertices:#{ nvertices }"
  puts "min:#{ min.inspect }"
  puts "max:#{ max.inspect }"

  cluster = Cluster.new(min, max)
  for sub in mesh.sub_meshes
    for v in sub.vertices
      cluster.push(v, sub)
    end
  end

  puts "#uniq vertices:#{ cluster.vertices.size }"
  puts

  puts "Copy direction:"
  puts "0 LtoR"
  puts "1 RtoL"
  print "Select copy direction (0-1): "
  line = gets
  copy_dir = line.chomp.to_i
  case copy_dir
  when 0
    cluster.dir = :LtoR
  when 1
    cluster.dir = :RtoL
  end

  cluster.assign_opposite_cells
  cluster.assign_opposite_vertices
  # cluster.dump
  cluster.copy_opposite_weights
end

UniqVertex.nodes = tso.nodes
UniqVertex.oppnode_idmap = oppnode_idmap

main(selected_mesh)
tso.save('out.tso')
