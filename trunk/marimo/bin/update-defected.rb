ent = "arc/xpc-defected.txt"
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
    unless arc
      puts "not found code: #{code}"
      next
    end
    puts "arcs.update code: #{code}"
    arc.summary = summary.gsub(/【(.+?)】/, '')
    arc.origname = origname
    arc.save
  end
end
