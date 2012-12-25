# Managed DirectX
$LOAD_PATH.unshift "E:/SharpDX/bin/Standard-net20"
require 'SharpDX'
require 'SharpDX.Direct3D9'
include SharpDX
include SharpDX::Direct3D

class Matrix
  def inspect
    "[ [ %f %f %f %f ]\n" % [ m11, m12, m13, m14 ] +
    "  [ %f %f %f %f ]\n" % [ m21, m22, m23, m24 ] +
    "  [ %f %f %f %f ]\n" % [ m31, m32, m33, m34 ] +
    "  [ %f %f %f %f ] ]" % [ m41, m42, m43, m44 ]
  end
end

class Vector3
  def inspect
    "[ %f %f %f ]" % [ x, y, z ]
  end
end

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
  m.M11 = x[0]; m.M12 = x[1]; m.M13 = x[2];
  m.M21 = y[0]; m.M22 = y[1]; m.M23 = y[2];
  m.M31 = z[0]; m.M32 = z[1]; m.M33 = z[2];
  m.M41 = t[0]; m.M42 = t[1]; m.M43 = t[2];
  m
end
