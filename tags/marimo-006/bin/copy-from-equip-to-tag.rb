#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

Equip.find(:all).each do |equip|
  puts "#{equip.name}"
  tag = Tag.find_or_create_by_name(equip.name)
  equip.arc_equips.each do |ae|
    puts "#{ae.arc_id}"
    tag.arc_tags.find_or_create_by_arc_id(ae.arc_id)
  end
end
