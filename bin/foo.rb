#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

p Float::EPSILON
p EPSILON = 1.4e-45

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../Debug")
require 'TDCG'

tso = TDCG::TSOFile.new
tso.load("TETRIS_01/1/1.tso")

p tso.nodes.size
# for node in tso.nodes
#   p node.name
# end
p tso.meshes.size
for mesh in tso.meshes
  p mesh.name
  p mesh.sub_meshes.size
  for sub in mesh.sub_meshes
    p sub.bone_indices.size
    p sub.vertices.size
    v0 = sub.vertices[0]
    v1 = sub.vertices[1]
    p0 = v0.position
    p1 = v1.position
    if p1 - p0 == Vector3.empty
    end
  end
end
