#!ruby
# encoding: utf-8
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + '/../lib')
require 'scrap'

site_code = ARGV.shift || 'xpc'
scrap = Scrap.find_by_site_code(site_code)

ent = "/Volumes/uploader/src/#{site_code}/index.html"
open(ent) do |f|
  while line = f.gets
    if scrap.match(line)
      name, summary, size, date, origname, locked = scrap.row
      puts [ name, summary, size, date, origname, locked ].join("\t")
    end
  end
end
