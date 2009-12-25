#!/usr/bin/ir

$LOAD_PATH.unshift "C:/Windows/Microsoft.NET/DirectX for Managed Code/1.0.2902.0"
require 'Microsoft.DirectX'
require 'Microsoft.DirectX.Direct3D'
require 'Microsoft.DirectX.Direct3DX'
include Microsoft::DirectX
include Microsoft::DirectX::Direct3D
# p Matrix.identity

require 'stringio'

def create_vertices(src)
  io = StringIO.new(src)
  vertices = []
  while line = io.gets
    idx, *vec = line.chomp.split(/ +/)
    vertices[idx.to_i] = vec.map { |x| x.to_f }
  end
  vertices
end

def vertices_to_matrix(vertices)
  z,y,t,x = vertices
  3.times { |i| x[i] -= t[i] }
  3.times { |i| y[i] -= t[i] }
  3.times { |i| z[i] -= t[i] }
  m = Matrix.identity
  m.M11 = x[0]
  m.M12 = x[1]
  m.M13 = x[2]
  m.M21 = y[0]
  m.M22 = y[1]
  m.M23 = y[2]
  m.M31 = z[0]
  m.M32 = z[1]
  m.M33 = z[2]
  m.M41 = t[0]
  m.M42 = t[1]
  m.M43 = t[2]
  m
end

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

scaling = Matrix.scaling(1000.0, 1000.0, 1000.0)

# Chichi_Left1 0.0 node ident
src = <<EOT
0  50.923  62.582  459.349
1 -14.188 993.833 -129.008
2   0.040   2.235   -0.434
3 839.021   2.235  -93.280
EOT

p1 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
puts "Chichi_Left1"
dump_matrix p1

# Chichi_Left2 0.0 node ident
src = <<EOT
0   8.129 -22.132 530.147
1 -10.684 935.408  41.512
2   0.462   1.136   0.320
3 790.087  11.042 -10.671
EOT

p2 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
puts "Chichi_Left2"
dump_matrix p2

# Chichi_Left3 0.0 node ident
src = <<EOT
0  10.597 -29.565 688.564
1  -7.567 688.675  30.892
2   0.641   0.650   0.558
3 689.261   9.289  -9.028
EOT

p3 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
puts "Chichi_Left3"
dump_matrix p3

# Chichi_Left4 0.0 node ident
src = <<EOT
0   15.165 -43.242 999.752
1  -11.208 999.585  44.865
2    0.711   0.628   0.822
3 1000.532  13.170 -13.095
EOT

p4 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
puts "Chichi_Left4"
dump_matrix p4

# Chichi_Left5 0.0 node ident
src = <<EOT
0   15.233 -43.206 999.992
1  -11.140 999.621  45.105
2    0.778   0.664   1.061
3 1000.600  13.206 -12.856
EOT

p5 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
puts "Chichi_Left5"
dump_matrix p5

# Chichi_Left5_End 0.0 node ident
src = <<EOT
0   15.233 -43.161 1000.104
1  -11.140 999.665   45.217
2    0.779   0.709    1.173
3 1000.600  13.251  -12.743
EOT

p5e = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
puts "Chichi_Left5_End"
dump_matrix p5e
