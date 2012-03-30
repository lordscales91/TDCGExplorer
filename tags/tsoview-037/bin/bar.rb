#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../Debug")
require 'TDCG'

tso = TDCG::TSOFile.new
tso.load("TETRIS_01/1/1.tso")

p tso.nodes.size
nodemap = {}
for node in tso.nodes
  nodemap[node.name] = node
end
p nodemap[System::String.new("Chichi_Right1")].parent.transformation_matrix
