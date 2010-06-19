#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

Arc.find(:all, :conditions => "summary is null").each do |arc|
  tah = arc.tahs.first
  if tah && tah.path
    md = %r([^/_]+).match(tah.path)
    folder = md[0]
    folder.sub!(/\.tah$/, '')
    puts [arc.id, folder, arc.origname].join("\t")
    arc.summary = folder
    arc.save!
  end
end
