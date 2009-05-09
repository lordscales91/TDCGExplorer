#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

while line = gets
  code, equip_caption, tag_caption, summary = line.chomp.split(/\t/)
  next unless equip_caption

  arc = Arc.find_by_code(code)
  next unless arc

  old_equip_names = arc.equips.map { |e| e.name }

  equip_names = equip_caption.split('/')
  add_names = equip_names - old_equip_names
  sub_names = old_equip_names - equip_names

  add_names.each do |equip_name|
    equip = Equip.find_or_create_by_name(equip_name)
    arc_equip = ArcEquip.create(:arc_id => arc.id, :equip_id => equip.id)
    puts "created arc_equip arc.code=#{code} equip.name=#{equip.name}"
  end
end
