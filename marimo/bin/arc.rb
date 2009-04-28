#!/usr/bin/ruby -KS

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
  puts [location, code, extname, summary, origname].join("\t")
end

def create_tah(line)
end

def create_tso(line)
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
