#!ir
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../lib")
require 'directx'

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../Debug")
require 'TDCG'

tso = TDCG::TSOFile.new
tso.load('base/data/model/N001BODY_A00.tso')

def tso.find_mesh(name)
  # p meshes.size
  found_mesh = nil
  for mesh in meshes
    # p mesh.name
    if mesh.name == name
      found_mesh = mesh
      break
    end
  end
  found_mesh
end

selected_mesh = tso.find_mesh('W_BODY_Nurin_M01')

def length_sq(a, b)
  dx = b.x - a.x
  dy = b.y - a.y
  dz = b.z - a.z
  length_sq = dx*dx + dy*dy + dz*dz
end

class UniqVertex
  attr :vertices
  attr :position
  attr :skin_weights
  attr_accessor :opposite_vertex

  @@warn_count = 0

  def self.warn_count
    @@warn_count
  end

  def initialize(a, sub)
    @vertices = {}
    @vertices[a] = sub
    @position = a.position
    @skin_weights = []
    for skin_weight in a.skin_weights
      @skin_weights.push TDCG::SkinWeight.new(sub.bone_indices[skin_weight.bone_index], skin_weight.weight)
    end
  end
  def push(a, sub)
    @vertices[a] = sub
  end
  def opposite_position
    Vector3.new( -position.x, position.y, position.z )
  end
  def inspect
    "UniqVertex(p:#{ position.inspect } #w:#{ skin_weights.size } #v:#{ vertices.size })"
  end
  def dump
    puts self.inspect
    puts "opp " + opposite_vertex.inspect
  end
  def warn_opposite_weights
    dump
    opposite_vertex.skin_weights.each_with_index do |opp_sw, i|
      sw = skin_weights[i]
      puts sprintf("%d sw(%d %f) opp sw(%d %f)", i, sw.bone_index, sw.weight, opp_sw.bone_index, opp_sw.weight)
    end
    puts
    @@warn_count += 1
  end
  def copy_opposite_weights
    found = nil
    opposite_vertex.skin_weights.each_with_index do |opp_sw, i|
      sw = skin_weights[i]
      unless (sw.weight - opp_sw.weight).abs < 0.01
        found = i
        break
      end
    end
    warn_opposite_weights if found
  end
end

class UniqCell
  attr :x
  attr :y
  attr :z
  attr :vertices
  attr_accessor :opposite_cell
  def initialize(x, y, z)
    @x = x
    @y = y
    @z = z
    @vertices = []
  end
  def push(a, sub)
    found = nil
    @vertices.each do |v|
      if length_sq(a.position, v.position) < Float::EPSILON
        v.push(a, sub)
        found = v
        break
      end
    end
    unless found
      @vertices.push UniqVertex.new(a, sub)
    end
  end
  def inspect
    "UniqCell(x:#{ x } y:#{ y } z:#{ z } #v:#{ vertices.size })"
  end
  def dump
    puts self.inspect
    v = vertices[0]
    v.dump if v
  end
  def find_vertex_at(position)
    min_len_sq = 10.0
    found = nil
    @vertices.each do |v|
      len_sq = length_sq(position, v.position)
      if min_len_sq > len_sq then min_len_sq = len_sq; found = v end
    end
    found
  end
  def assign_opposite_vertices
    @vertices.each do |v|
      v.opposite_vertex = opposite_cell.find_vertex_at(v.opposite_position)
    end
  end
  def copy_opposite_weights
    @vertices.each do |v|
      v.copy_opposite_weights
    end
  end
end

class Cluster
  def initialize(min, max)
    @min = min
    @max = max
    @cells = []
  end
  def xidx(x)
    x.floor - @min.x.floor
  end
  def yidx(y)
    y.floor - @min.y.floor
  end
  def zidx(z)
    z.floor - @min.z.floor
  end
  def get_cell(x, y, z)
    xlen = @max.x.floor - @min.x.floor + 1
    ylen = @max.y.floor - @min.y.floor + 1
    zlen = @max.z.floor - @min.z.floor + 1
    cidx = x * ylen * zlen + y * zlen + z
    @cells[cidx] ||= UniqCell.new(x, y, z)
  end
  def push(v, sub)
    x = xidx(v.position.x)
    y = yidx(v.position.y)
    z = zidx(v.position.z)
    cell = get_cell(x, y, z)
    cell.push(v, sub)
  end
  def dump
    @cells.compact.each do |cell|
      cell.dump
    end
  end
  def vertices
    ary = []
    @cells.compact.each do |cell|
      ary.concat cell.vertices
    end
    ary
  end
  def oppositex(x)
    xlen = @max.x.floor - @min.x.floor + 1
    xend = xlen - 1
    xend - x
  end
  def assign_opposite_cells
    @cells.compact.each do |cell|
      x = oppositex(cell.x)
      y = cell.y
      z = cell.z
      cell.opposite_cell = get_cell(x, y, z)
    end
  end
  def assign_opposite_vertices
    @cells.compact.each do |cell|
      cell.assign_opposite_vertices
    end
  end
  def copy_opposite_weights
    x = xidx(0.0)
    @cells.compact.each do |cell|
      cell.copy_opposite_weights if cell.x < x
    end
  end
end

def main(mesh)
  ary = []
  min = Vector3.empty
  max = Vector3.empty

  # p mesh.sub_meshes.size
  for sub in mesh.sub_meshes
    # p sub.bone_indices.size
    # p sub.vertices.size
    for v in sub.vertices
      x = v.position.x
      y = v.position.y
      z = v.position.z

      min.x = x if min.x > x
      min.y = y if min.y > y
      min.z = z if min.z > z

      max.x = x if max.x < x
      max.y = y if max.y < y
      max.z = z if max.z < z

      ary.push(v)
    end
  end
  puts "#vertices:#{ ary.size }"
  puts "min:#{ min.inspect }"
  puts "max:#{ max.inspect }"

  cluster = Cluster.new(min, max)
  for sub in mesh.sub_meshes
    for v in sub.vertices
      cluster.push(v, sub)
    end
  end

  puts "#uniq vertices:#{ cluster.vertices.size }"
  puts
  cluster.assign_opposite_cells
  cluster.assign_opposite_vertices
  # cluster.dump
  cluster.copy_opposite_weights
end

# p Float::EPSILON
main(selected_mesh)

puts "#warn:#{ UniqVertex.warn_count }"
