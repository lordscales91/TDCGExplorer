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
uploader.base_html = "all.html"
uploader.local_dir = "/Volumes/uploader/arc/uppervolta-3d.net/" + name

src_path = "/Volumes/uploader/src/uppervolta-3d.net/" + name + "/all.html"

if false
  # read index.html from remote site
  body = uploader.get_base
  # save index.html as local cache
  open(src_path, 'wb') { |f| f.write body }
end

# read index.html from local cache
body = IO.read(src_path)

each_filename(body) do |filename|
  exist = uploader.local_file_exist?(filename)
  puts [ filename, exist ? 'exist' : 'download' ].join("\t")
  unless exist
    sleep(5)
    uploader.download_file_through_clicker(filename)
  end
end
