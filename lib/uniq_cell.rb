class UniqCell
  attr :cluster
  attr :x
  attr :y
  attr :z
  attr :contains_zerox
  attr :vertices
  attr_accessor :opposite_cell

  def initialize(cluster, x, y, z, contains_zerox = false)
    @cluster = cluster
    @x = x
    @y = y
    @z = z
    @contains_zerox = contains_zerox
    @vertices = []
  end

  def length_sq(a, b)
    dx = b.x - a.x
    dy = b.y - a.y
    dz = b.z - a.z
    length_sq = dx*dx + dy*dy + dz*dz
  end

  def push(a, sub)
    u = nil
    @vertices.each do |v|
      if length_sq(a.position, v.position) < Float::EPSILON
        v.push(a, sub)
        u = v
        break
      end
    end
    unless u
      u = UniqVertex.new(a, sub)
      @vertices.push u
    end
    u
  end

  def inspect
    "UniqCell(x:#{ x } y:#{ y } z:#{ z } #v:#{ vertices.size })"
  end

  def dump
    puts self.inspect
    v = vertices[0]
    v.dump if v
  end

  def find_vertex_and_len_sq_at(position)
    min_len_sq = 10.0
    found = nil
    @vertices.each do |v|
      len_sq = length_sq(position, v.position)
      if min_len_sq > len_sq then min_len_sq = len_sq; found = v end
    end
    [ found, min_len_sq ]
  end

  def find_vertex(position, cell, found, min_len_sq)
    if cell
      v, len_sq = cell.find_vertex_and_len_sq_at(position)
      if len_sq < min_len_sq then min_len_sq = len_sq; found = v end
    end
    [ found, min_len_sq ]
  end

  def neighbor(dx, dy, dz)
    cluster.get_cell(x + dx, y + dy, z + dz)
  end

  def sign(x)
    d = x - (x+0.5).floor
    d.abs < Float::EPSILON ? 0 : (d < 0 ? -1 : +1)
  end

  def find_opposite_vertex(v)
    opp_p = v.opposite_position
    x = opp_p.x
    y = opp_p.y
    z = opp_p.z

    dx = sign(x)
    dy = sign(y)
    dz = sign(z)

    opp_v = nil
    min_len_sq = 10.0

    opp_v, min_len_sq = find_vertex(opp_p, opposite_cell, opp_v, min_len_sq)

    if dx != 0
      opp_v, min_len_sq = find_vertex(opp_p, opposite_cell.neighbor(dx, 0, 0), opp_v, min_len_sq)
    end

    if dy != 0
      opp_v, min_len_sq = find_vertex(opp_p, opposite_cell.neighbor(0, dy, 0), opp_v, min_len_sq)
    end

    if dz != 0
      opp_v, min_len_sq = find_vertex(opp_p, opposite_cell.neighbor(0, 0, dz), opp_v, min_len_sq)
    end

    if dx != 0 && dy != 0
      opp_v, min_len_sq = find_vertex(opp_p, opposite_cell.neighbor(dx, dy, 0), opp_v, min_len_sq)
    end

    if dy != 0 && dz != 0
      opp_v, min_len_sq = find_vertex(opp_p, opposite_cell.neighbor(0, dy, dz), opp_v, min_len_sq)
    end

    if dz != 0 && dx != 0
      opp_v, min_len_sq = find_vertex(opp_p, opposite_cell.neighbor(dx, 0, dz), opp_v, min_len_sq)
    end

    if dx != 0 && dy != 0 && dz != 0
      opp_v, min_len_sq = find_vertex(opp_p, opposite_cell.neighbor(dx, dy, dz), opp_v, min_len_sq)
    end

    v.opposite_vertex = opp_v
  end

  def assign_opposite_vertices
    if contains_zerox
      @vertices.each do |v|
        if v.position.x.abs < 1.0e-4
          v.opposite_vertex = v
          next
        end
        find_opposite_vertex(v)
      end
    else
      @vertices.each do |v|
        find_opposite_vertex(v)
      end
    end
  end

  def copy_opposite_weights
    if contains_zerox
      @vertices.each do |v|
        next if v.position.x.abs < 1.0e-4
        next if cluster.dir == :LtoR && v.position.x < 0.0
        next if cluster.dir == :RtoL && v.position.x > 0.0
        v.copy_opposite_weights
      end
    else
      @vertices.each do |v|
        v.copy_opposite_weights
      end
    end
  end
end
