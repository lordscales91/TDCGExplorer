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

t1 = Vector3.new(+0.005000, +2.302570, -0.856764)
t2 = Vector3.new(+0.541797, -1.336350, +1.364660)
t3 = Vector3.new(+0.214305, -0.510129, +0.492141)
t4 = Vector3.new(+0.095186, -0.016642, +0.385775)
t5 = Vector3.new(+0.064401, +0.045397, +0.238688)
t5e = Vector3.new(-0.000283, +0.049728, +0.110266)

scaling = Matrix.scaling(1000.0, 1000.0, 1000.0)

# Chichi_Left1 0.0 node ident
src = <<EOT
0  50.923  62.582  459.349
1 -14.188 993.833 -129.008
2   0.040   2.235   -0.434
3 839.021   2.235  -93.280
EOT

p1 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m1 = p1
puts "Chichi_Left1"
dump_matrix m1

# Chichi_Left2 0.0 node ident
src = <<EOT
0   8.129 -22.132 530.147
1 -10.684 935.408  41.512
2   0.462   1.136   0.320
3 790.087  11.042 -10.671
EOT

p2 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m2 = p2 * Matrix.invert(p1)
puts "Chichi_Left2"
dump_matrix m2

# Chichi_Left3 0.0 node ident
src = <<EOT
0  10.597 -29.565 688.564
1  -7.567 688.675  30.892
2   0.641   0.650   0.558
3 689.261   9.289  -9.028
EOT

p3 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m3 = p3 * Matrix.invert(p2)
puts "Chichi_Left3"
dump_matrix m3

# Chichi_Left4 0.0 node ident
src = <<EOT
0   15.165 -43.242 999.752
1  -11.208 999.585  44.865
2    0.711   0.628   0.822
3 1000.532  13.170 -13.095
EOT

p4 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m4 = p4 * Matrix.invert(p3)
puts "Chichi_Left4"
dump_matrix m4

# Chichi_Left5 0.0 node ident
src = <<EOT
0   15.233 -43.206 999.992
1  -11.140 999.621  45.105
2    0.778   0.664   1.061
3 1000.600  13.206 -12.856
EOT

p5 = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m5 = p5 * Matrix.invert(p4)
puts "Chichi_Left5"
dump_matrix m5

# Chichi_Left5_End 0.0 node ident
src = <<EOT
0   15.233 -43.161 1000.104
1  -11.140 999.665   45.217
2    0.779   0.709    1.173
3 1000.600  13.251  -12.743
EOT

p5e = Matrix.invert(scaling) * vertices_to_matrix(create_vertices(src))
m5e = p5e * Matrix.invert(p5)
puts "Chichi_Left5_End"
dump_matrix m5e
