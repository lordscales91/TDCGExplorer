class Tah < ActiveRecord::Base
  belongs_to :arc
  has_many :tsos

  def collisions
    tsos.map(&:collisions).flatten.uniq.map(&:tah).uniq
  end
end
