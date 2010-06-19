#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

while line = gets
  no, equip_caption, summary = line.chomp.split(/\t/)
  code = 'mod%04d' % no.to_i
  arc = Arc.find_by_code(code)
  if arc && equip_caption
    equip_names = equip_caption.split('/')
    equip_names.each do |equip_name|
      equip = Equip.find_or_create_by_name(equip_name)
      ArcEquip.create(:arc_id => arc.id, :equip_id => equip.id)
      puts "created arc_equip arc.code=#{code} equip.name=#{equip.name}"
    end
  end
end
