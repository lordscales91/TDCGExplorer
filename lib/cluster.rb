class Cluster
  attr_accessor :dir
  attr :min
  attr :max
  attr :cells
  attr :xlen
  attr :ylen
  attr :zlen

  def initialize(min, max)
    if min.x < 0 && max.x < 0 || min.x > 0 && max.x > 0
      raise ArgumentError.new("invalid_range")
    end

    min.x = -max.x if min.x.abs < max.x.abs
    max.x = -min.x if min.x.abs > max.x.abs

    @dir = :LtoR
    @min = min
    @max = max
    @cells = []

    @xlen = (@max.x + 0.5).floor - (@min.x + 0.5).floor + 1
    @ylen = (@max.y + 0.5).floor - (@min.y + 0.5).floor + 1
    @zlen = (@max.z + 0.5).floor - (@min.z + 0.5).floor + 1
  end
  def xidx(x)
    (x + 0.5).floor - (@min.x + 0.5).floor
  end
  def yidx(y)
    (y + 0.5).floor - (@min.y + 0.5).floor
  end
  def zidx(z)
    (z + 0.5).floor - (@min.z + 0.5).floor
  end
  def get_cell(x, y, z)
    cidx = x * ylen * zlen + y * zlen + z
    @cells[cidx] ||= UniqCell.new(self, x, y, z, x == xidx(0.0))
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
      next if dir == :LtoR && cell.x < x
      next if dir == :RtoL && cell.x > x
      cell.copy_opposite_weights
    end
  end
end
