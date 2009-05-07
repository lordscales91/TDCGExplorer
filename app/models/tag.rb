class Tag < ActiveRecord::Base
  has_many :arc_tags
  has_many :arcs, :through => :arc_tags
end
