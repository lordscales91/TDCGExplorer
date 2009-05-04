class ArcEquip < ActiveRecord::Base
  belongs_to :arc
  belongs_to :equip

  def equip_name
    equip ? equip.name : nil
  end

  def equip_name=(equip_name)
    equip = Equip.find_by_name(equip_name)
    self.equip_id = equip ? equip.id : nil
  end

  def should_destroy?
    equip_id.nil?
  end
end
