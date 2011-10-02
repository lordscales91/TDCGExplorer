#!ruby
# encoding: utf-8
require File.dirname(__FILE__) + "/../config/environment"
require 'net/http'

site_code = ARGV.shift || 'xpc'

case site_code
when 'xpc'
  host = "cdn.3dcustom.net"
  root_path = "/TACuploader"
  base_html = "upload.html"
when 'mod'
  host = "www.nijibox5.com"
  root_path = "/futabacustom/mod"
  base_html = ""
end
local_dir = "arc/#{site_code}"

header = UserAgent.default_http_header
response = Net::HTTP.new(host).start do |http|
  http.get("#{root_path}/#{base_html}", header)
end
body = response.body

base_fname = "#{local_dir}/index.html"
open(base_fname, 'w') do |f|
  f.puts body
end

case site_code
when 'xpc'
  location = '3DCUSTOMNET'
when 'mod'
  location = 'futaba'
end

open(base_fname, 'r') do |f|
  scrap = Scrap.find_by_site_code(site_code)
  while line = f.gets
    if scrap.match(line)
      name, summary, size, date, origname, locked = scrap.row
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
end
