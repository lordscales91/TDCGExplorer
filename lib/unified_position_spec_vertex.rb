class UnifiedPositionSpecVertex < TDCG::Vertex
  attr :spec
  def initialize(a, sub)
    self.position = a.position
    self.normal = a.normal
    self.u = a.u
    self.v = a.v
    self.skin_weights = System::Array[TDCG::SkinWeight].new(4)
    4.times do |i|
      self.skin_weights[i] = TDCG::SkinWeight.new(sub.bone_indices[a.skin_weights[i].bone_index], a.skin_weights[i].weight)
    end
    @spec = sub.spec
  end
  def eql?(o)
    position == o.position && spec == o.spec
  end
  def hash
    position.hash ^ spec.hash
  end
end
