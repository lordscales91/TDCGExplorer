#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

def combined_matrix(node)
  m = Matrix.identity
  while node != nil
    m = m * node.transformation_matrix
    node = node.parent
  end
  m
end

def dump_matrix(m)
  for row in 0..3
    for col in 0..3
      method_name = "M#{row+1}#{col+1}"
      puts "m.%s = %fF;" % [ method_name, m.send(method_name) ]
    end
  end
end

# Chichi_Right1 position
world_0 = Vector3.new( -0.00455, 16.23420, -1.00901 ) # bust=0.225 world
world_1 = Vector3.new( -0.04005, 16.16644, -0.58614 ) # bust=0 world
local_0 = Vector3.new( 0, 0, 0 ) # bust=0.225 local
local_1 = Vector3.new( -0.04049, +2.23481, -0.43389 ) # bust=0 local

# bust 0.225 .. 0
local_d = local_1 - local_0
world_d = world_1 - world_0
t1 = local_d - world_d

# Chichi_Right1 0.0 node ident
src = <<EOT
0 -0.09138 +2.29516 +0.02589
1 -0.02626 +3.22641 -0.56247
2 -0.04049 +2.23481 -0.43389
3 +0.79849 +2.23481 -0.34104
EOT

p1 = vertices_to_matrix(create_vertices(src))
m1 = Matrix.invert(Matrix.translation(t1)) * p1
puts "Chichi_Right1"
dump_matrix m1

# Chichi_Right2 position
world_0 = Vector3.new( -0.54656, 14.89820, +0.35598 ) # bust=0.225 world
world_1 = Vector3.new( -0.46191, 15.06842, +0.16807 ) # bust=0 world
local_0 = Vector3.new( 0, 0, 0 ) # bust=0.225 local
local_1 = Vector3.new( -0.46235, +1.13679, +0.32032 ) # bust=0 local

# bust 0.225 .. 0
local_d = local_1 - local_0
world_d = world_1 - world_0
t2 = local_d - world_d - t1

# Chichi_Right2 0.0 node ident
src = <<EOT
0 -0.47002 1.11352 0.85015
1 -0.45121 2.07106 0.36152
2 -0.46235 1.13679 0.32032
3  0.32744 1.12688 0.33132
EOT

p2 = vertices_to_matrix(create_vertices(src))
m2 = Matrix.invert(Matrix.translation(t2)) * p2 * Matrix.invert(p1)
puts "Chichi_Right2"
dump_matrix m2
