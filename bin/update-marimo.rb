#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

while line = gets
  code, summary, origname = line.chomp.split(/\t/)
  if summary
    summary.strip!
    if /\A"(.+?)"\z/.match(summary)
      summary = $1
    end
  end
  if origname
    origname.strip!
    if /\A"(.+?)"\z/.match(origname)
      origname = $1
    end
  end
  arc = Arc.find_by_code(code)
  if arc
    puts "update arc #{code}"
    arc.summary = summary unless summary.blank?
    arc.origname = origname unless origname.blank?
    arc.save!
  end
end
