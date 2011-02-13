#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../Debug")
require 'TDCG'

tso = TDCG::TSOFile.new
tso.load('base/data/model/N001BODY_A00.tso')

tso.nodes.each_with_index do |node, i|
  puts "#{i} #{node.name}"
end
