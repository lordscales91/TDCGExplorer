class UniqVertex
  attr :vertices
  attr :position
  attr :skin_weights
  attr_accessor :opposite_vertex

  @@nodes = nil
  def self.nodes
    @@nodes
  end
  def self.nodes=(nodes)
    @@nodes= nodes
  end

  def node_at(index)
    @@nodes[index]
  end

  @@oppnode_idmap = nil
  def self.oppnode_idmap
    @@oppnode_idmap
  end
  def self.oppnode_idmap=(oppnode_idmap)
    @@oppnode_idmap= oppnode_idmap
  end

  def opposite_bone_index(bone_index)
    @@oppnode_idmap[bone_index]
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

    4.times do |i|
      sw = skin_weights[i]
      a_sw = a.skin_weights[i]
      if sw.weight != 0.0
        if sw.bone_index != sub.bone_indices[a_sw.bone_index]
          puts "### warn: bone_index not match"
          dump
          puts sprintf("%d sw(%d %f) a sw(%d %f)", i, sw.bone_index, sw.weight, sub.bone_indices[a_sw.bone_index], a_sw.weight)
        end
      end
    end
  end

  def opposite_position
    Vector3.new( -position.x, position.y, position.z )
  end

  def inspect
    "UniqVertex(p:#{ position.inspect } #v:#{ vertices.size }"
  end

  def dump
    puts self.inspect
    puts "opp " + opposite_vertex.inspect
  end

  def warn_opposite_weights
    puts "### warn: weights gap found"
    dump
    4.times do |i|
      sw = skin_weights[i]
      opp_sw = opposite_vertex.skin_weights[i]
      puts sprintf("%d sw(%d %f) opp sw(%d %f)", i, sw.bone_index, sw.weight, opp_sw.bone_index, opp_sw.weight)
    end
    puts
  end

  def warn_bone_index_not_found(a, sub)
    puts "### warn: a_sw.bone_index not found in sub.bone_indices"
    dump
    4.times do |i|
      sw = skin_weights[i]
      a_sw = a.skin_weights[i]
      puts sprintf("%d sw(%d %f) a sw(%d %f)", i, sw.bone_index, sw.weight, sub.bone_indices[a_sw.bone_index], a_sw.weight)
    end
    puts
  end

  def copy_opposite_weights
    return if opposite_vertex.nil?
    return if opposite_vertex == self

    weights_gap_found = nil
    4.times do |i|
      sw = skin_weights[i]
      opp_sw = opposite_vertex.skin_weights[i]
      unless (sw.weight - opp_sw.weight).abs < 1.0e-2
        weights_gap_found = i
        break
      end
    end
    warn_opposite_weights if weights_gap_found

    4.times do |i|
      sw = skin_weights[i]
      opp_sw = opposite_vertex.skin_weights[i]
      sw.bone_index = opposite_bone_index(opp_sw.bone_index)
      sw.weight = opp_sw.weight
    end

    vertices.each do |a, sub|
      copy_weights(a, sub)
    end
  end

  def copy_weights(a, sub)
    bone_index_not_found = nil
    4.times do |i|
      sw = skin_weights[i]
      a_sw = a.skin_weights[i]
      a_bone_idx = sub.bone_indices.index(sw.bone_index) || -1
      if a_bone_idx == -1
        puts "### warn: add bone index:#{ sw.bone_index }"
        a_bone_idx = sub.add_bone(node_at(sw.bone_index))
      end
      if a_bone_idx == -1
        if sw.weight == 0.0
          a_sw.bone_index = 0
          a_sw.weight = 0.0
        else
          bone_index_not_found = true
        end
      else
        a_sw.bone_index = a_bone_idx
        a_sw.weight = sw.weight
      end
    end
    warn_bone_index_not_found(a, sub) if bone_index_not_found
  end
end
