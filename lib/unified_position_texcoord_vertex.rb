class UnifiedPositionTexcoordVertex < TDCG::Vertex
  def eql?(o)
    position == o.position && u == o.u && v == o.v
  end
  def hash
    position.hash ^ u.hash ^ v.hash
  end
end
