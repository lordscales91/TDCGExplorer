class Tag < ActiveRecord::Base
  has_many :arc_tags
  has_many :arcs, :through => :arc_tags

  def before_save
    self.locked = false if locked.nil?
    nil
  end
end
