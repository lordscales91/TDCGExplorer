#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../debug")
require 'TDCG'

tmo = TDCG::TMOFile.new
tmo.load("base/data/motion/back01.tmo")
for node in tmo.nodes
  puts node.name
end
