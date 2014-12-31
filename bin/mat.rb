#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

# Chichi_Right1	bust=0.225	world
src = <<EOT
0 -0.00455 16.23420 -0.22901
1 -0.00455 17.05820 -1.00901
2 -0.00455 16.23420 -1.00901
3  0.83045 16.23420 -1.00901
EOT

# Chichi_Right1	bust=0	world
src = <<EOT
0 -0.09093 16.22679 -0.12636
1 -0.02582 17.15804 -0.71472
2 -0.04005 16.16644 -0.58614
3  0.79893 16.16644 -0.49329
EOT

# Chichi_Right2	bust=0.225	world
src = <<EOT
0 -0.54656 14.89820 1.13598
1 -0.54656 15.72220 0.35598
2 -0.54656 14.89820 0.35598
3  0.28844 14.89820 0.35598
EOT

# Chichi_Right2	bust=0	world
src = <<EOT
0 -0.46958 15.04515 0.69790
1 -0.45076 16.00269 0.20927
2 -0.46191 15.06842 0.16807
3  0.32789 15.05851 0.17907
EOT

p vertices_to_matrix(create_vertices(src))
