class UnifiedPositionTexcoordVertex < TDCG::Vertex
  WEIGHT_EPSILON = Float::EPSILON # or 1.0e-4

  def initialize(v, bone_indices_map)
    self.position = v.position
    self.normal = v.normal
    self.u = v.u
    self.v = v.v
    self.skin_weights = System::Array[TDCG::SkinWeight].new(4)
    4.times do |i|
      self.skin_weights[i] =
	if v.skin_weights[i].weight < WEIGHT_EPSILON
	  TDCG::SkinWeight.new(0, 0.0)
	else
	  TDCG::SkinWeight.new(bone_indices_map[v.skin_weights[i].bone_index], v.skin_weights[i].weight)
	end
    end
    self.generate_bone_indices
  end
  def eql?(o)
    position == o.position && u == o.u && v == o.v
  end
  def hash
    position.hash ^ u.hash ^ v.hash
  end
end
