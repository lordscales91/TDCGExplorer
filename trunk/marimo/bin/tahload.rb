#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

def create_arc(line)
  md = /^# ([a-z]{3}) (.+)/.match(line)
  _, extname, tmp = md.to_a
  location = File.dirname(tmp)
  basename = File.basename(tmp, "." + extname)
  if md = /^([A-Za-z0-9]{6,9})$/.match(basename)
    _, code = md.to_a
    summary = nil
    origname = nil
  elsif md = /^([A-Za-z0-9]{6,9}) (.+)/.match(basename)
    _, code, tmp = md.to_a
    summary, origname = tmp.split("_", 2)
  elsif md = /^([A-Za-z0-9]{6,9})_(.+)/.match(basename)
    _, code, tmp = md.to_a
    summary, origname = tmp.split("_", 2)
  elsif md = /^([A-Za-z0-9]{6,9})(Åy.+)/.match(basename)
    _, code, tmp = md.to_a
    summary, origname = tmp.split("_", 2)
  else
    raise "pattern not matched. " + line
  end
  arc = Arc.new
  arc.code = code
  arc.extname = extname
  arc.location = location
  arc.summary = summary
  arc.origname = origname
  arc.save!
  arc
end

def create_tah(line)
  md = /^# TAH in archive (.+)/.match(line)
  _, path = md.to_a
  tah = Tah.new
  tah.arc = @arc
  tah.path = path
  tah.save!
  tah
end

def create_tso(line)
  path = line
  tso = Tso.new
  tso.tah = @tah
  tso.path = path
  tso.save!
  tso
end

while line = gets
  line.chomp!
  case line
  when /^# zip /
    @arc = create_arc(line)
  when /^# rar /
    @arc = create_arc(line)
  when /^# lzh /
    @arc = create_arc(line)
  when /^# TAH in archive /
    @tah = create_tah(line)
  when /^# TAH /
    # .tah ÇÕë∂ç›ÇµÇ»Ç¢ÇÕÇ∏
    raise "raw TAH found."
  when /^Error:/
    next
  when /^   at /
    next
  when /\.tso$/
    @tso = create_tso(line)
  end
end
