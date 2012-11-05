class UnifiedPositionVertex < TDCG::Vertex
  attr :spec
  def initialize(a, bone_indices, spec)
    self.position = a.position
    self.normal = a.normal
    self.u = a.u
    self.v = a.v
    self.skin_weights = System::Array[TDCG::SkinWeight].new(4)
    4.times do |i|
      self.skin_weights[i] = TDCG::SkinWeight.new(bone_indices[a.skin_weights[i].bone_index], a.skin_weights[i].weight)
    end
    @spec = spec
  end
  def eql?(o)
    position == o.position
  end
  def hash
    position.hash
  end
end
