#!/usr/bin/ruby
# download from futabacustom mod
# http://www.nijibox5.com/futabacustom/mod/

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
uploader.host = "www.nijibox5.com"
uploader.root_path = "/futabacustom/mod"
uploader.base_html = ""
uploader.local_dir = '/Volumes/uploader/arc/mod'
body = uploader.get_base
open('/Volumes/uploader/src/mod/index.html', 'w'){ |f| f.puts body }
each_filename(body) do |filename|
  exist = uploader.local_file_exist?(filename)
  puts [ filename, exist ? 'exist' : 'download' ].join("\t")
  uploader.download_file(filename) unless exist
end
