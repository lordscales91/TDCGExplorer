#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../debug")
require 'TDCG'

tso = TDCG::TSOFile.new
tso.load("base/data/model/man.tso")
# # dump nodes
# for node in tso.nodes
#   puts node.name
# end
# # dump textures
# for tex in tso.textures
#   puts "name:#{tex.name} file:#{tex.file_name} size:#{tex.width}x#{tex.height}x#{tex.depth}"
# end
# dump sub_scripts
for sub in tso.sub_scripts
  puts "name:#{sub.name} file:#{sub.file_name}"
  puts sub.lines
end
