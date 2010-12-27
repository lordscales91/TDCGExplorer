class TSOFace
  attr :a
  attr :b
  attr :c
  attr_accessor :spec
  def initialize(a, b, c)
    @a = a
    @b = b
    @c = c
    @spec = a.spec
  end

  def vertices
    [a, b, c]
  end

  def inspect
    "Face(spec:#{ spec })"
  end
end
