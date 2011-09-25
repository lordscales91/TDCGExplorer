#!/usr/bin/ruby
# download from 3dcustom.net pose
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

uploader = SnUploader.new
uploader.host = "cdn.3dcustom.net"
uploader.root_path = "/TAPuploader"
uploader.base_html = "upload.html"
uploader.local_dir = '/Volumes/uploader/arc/xpp'
body = uploader.get_base
each_filename(body) do |filename|
  exist = uploader.local_file_exist?(filename)
  puts [ filename, exist ? 'exist' : 'download' ].join("\t")
  uploader.download_file(filename) unless exist
end
