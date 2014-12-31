#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../Debug")
require 'TDCG'

fig = TDCG::Figure.new

tso = TDCG::TSOFile.new
tso.load("TETRIS_01/1/1.tso")

# fig.slide_matrices.WaistRatio = 1.0
fig.slide_matrices.BustRatio = 0.0

tso_nodemap = {}
for node in tso.nodes
  tso_nodemap[node.name] = node
end

# # transform_matrix を 1. に変更
# for node in tso.nodes
#   node.transformation_matrix = Matrix.identity
# end
# 
# # transform_matrix を変更したら offset_matrix を再計算する
# for node in tso.nodes
#   node.compute_offset_matrix
# end

fig.AddTSO(tso)
fig.UpdateNodeMapAndBoneMatrices()

tso_node = tso_nodemap[System::String.new('Chichi_Right2')]
tmo_node = fig.nodemap[tso_node]

m = tso_node.get_world_coordinate
p m
p tso_node.offset_matrix
p tmo_node.combined_matrix

px = Vector3.new(m.m11 + m.m41, m.m12 + m.m42, m.m13 + m.m43)
p Vector3.transform_coordinate(px, tso_node.offset_matrix * tmo_node.combined_matrix)

py = Vector3.new(m.m21 + m.m41, m.m22 + m.m42, m.m23 + m.m43)
p Vector3.transform_coordinate(py, tso_node.offset_matrix * tmo_node.combined_matrix)

pz = Vector3.new(m.m31 + m.m41, m.m32 + m.m42, m.m33 + m.m43)
p Vector3.transform_coordinate(pz, tso_node.offset_matrix * tmo_node.combined_matrix)
