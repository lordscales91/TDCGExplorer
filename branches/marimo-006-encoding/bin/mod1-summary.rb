#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

while line = gets
  no, equip_caption, summary = line.chomp.split(/\t/)
  code = 'mod%04d' % no.to_i
  arc = Arc.find_by_code(code)
  if arc && summary
    puts "update arc: #{code}"
    arc.summary = summary
    arc.save!
  end
end
