site_code = ARGV.shift || 'xpc'
location =
  case site_code
  when 'xpc'
    '3DCUSTOMNET'
  when 'mod'
    'futaba'
  end
ent = "arc/#{site_code}.txt"
open(ent) do |f|
  while line = f.gets
    name, summary, size, date, origname, locked = line.chomp.split("\t")
    if md = /\.(.+)\z/.match(name)
      extname = md[1]
      code = File.basename(name, '.' + extname)
    else
      extname = nil
      code = name.dup
    end
    arc = Arc.find_by_code(code)
    if arc
      puts "found code: #{code}"
      next
    end
    puts "arcs.create code: #{code}"
    arc = Arc.new
    arc.code = code
    arc.extname = extname
    arc.location = location
    arc.summary = summary
    arc.origname = origname
    arc.save
  end
end
