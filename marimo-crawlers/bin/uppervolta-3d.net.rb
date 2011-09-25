#!/usr/bin/ruby
# download from uppervolta-3d.net
#
$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + '/../lib')
require 'sn_uploader'

def each_filename(text)
  file_href = './src/'
  file_re = Regexp.new(Regexp.escape(file_href) + '(.+?)\.html')
  text.scan(/href="(.+?)"/) do |href, |
    if md = file_re.match(href)
      yield md[1]
    end
  end
end

name = ARGV.shift || 'kiss'

uploader = SnUploader.new
uploader.host = "uppervolta-3d.net"
uploader.root_path = "/" + name
uploader.base_html = ""
uploader.local_dir = "/Volumes/uploader/arc/uppervolta-3d.net/" + name
body = uploader.get_base
each_filename(body) do |filename|
  exist = uploader.local_file_exist?(filename)
  puts [ filename, exist ? 'exist' : 'download' ].join("\t")
  unless exist
    sleep(1)
    uploader.download_file_through_clicker(filename)
  end
end
