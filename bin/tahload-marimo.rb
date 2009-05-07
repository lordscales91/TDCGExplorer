#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

def create_arc(line)
  md = /^# ([a-z]{3}) (.+)/.match(line)
  _, extname, tmp = md.to_a
  _, location, basename = tmp.split("\\")
  case location
  when '3d'
    location = '3DCUSTOMNET'
  end

  basename.sub!(Regexp.new(Regexp.escape("." + extname) + "$"), '')
  case basename
  when /_rar$/
    basename.sub!(/_rar$/, '')
    extname = 'rar'
  when /_lzh$/
    basename.sub!(/_lzh$/, '')
    extname = 'lzh'
  end

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
  # puts [code, extname, location, summary, origname].join("\t")

  # arc = Arc.find_by_code(code)
  # if arc
  #   puts "arc dup code #{code}"
  # end

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
