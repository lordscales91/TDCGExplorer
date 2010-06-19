#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

def readline
  NKF.nkf('-Ws', gets.chomp)
end

until ARGF.eof?
  code = readline
  summary = readline
  origname = readline
  gets # empty line

  arc = Arc.find_by_code(code)
  unless arc
    puts "arc not found: #{code}"
    next
  end
  puts "update arc: #{code}"
  arc.code = code
  arc.summary = summary
  arc.origname = origname
  arc.save!
end
