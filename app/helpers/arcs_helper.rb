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

  def add_arc_tag_link(name)
    link_to_function name do |page|
      page.insert_html :bottom, :arc_tags, :partial => 'arc_tag', :object => ArcTag.new
    end
  end

  def add_arc_tag_function
    url = url_for(:action => 'auto_complete_for_tag_name')
  h <<'EOT'.sub(/url/, url)
    var i = c1();
    new Insertion.Bottom("arc_tags", "<p>\n  <label for=\"arc_tag_"+i+"\">É^ÉO</label><br />\n  <input id=\"arc_tag_"+i+"\" name=\"arc[arc_tag_attributes][][tag_name]\" size=\"30\" type=\"text\" />\n<div class=\"auto_complete\" id=\"arc_tag_ac_"+i+"\"></div>\n</p>\n");
    new Ajax.Autocompleter('arc_tag_'+i, 'arc_tag_ac_'+i, 'url', {frequency:1.0});
    return false;
EOT
  end

  def relationship_kind_collection
    Relationship.kind_collection
  end

  def rev_relationship_kind_collection
    Relationship.rev_kind_collection
  end
end
