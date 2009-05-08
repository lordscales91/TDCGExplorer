class Equip < ActiveRecord::Base
  has_many :arc_equips, :dependent => :destroy
  has_many :arcs, :through => :arc_equips
end
