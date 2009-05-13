class Tag < ActiveRecord::Base
  has_many :arc_tags
  has_many :arcs, :through => :arc_tags
  validates_uniqueness_of :name

  def before_save
    self.locked = false if locked.nil?
    nil
  end
end
