module ArcsHelper
  def add_arc_equip_link(name)
    link_to_function name do |page|
      page.insert_html :bottom, :arc_equips, :partial => 'arc_equip', :object => ArcEquip.new
    end
  end

  def add_relationship_link(name)
    link_to_function name do |page|
      page.insert_html :bottom, :relationships, :partial => 'relationship', :object => Relationship.new
    end
  end
end
