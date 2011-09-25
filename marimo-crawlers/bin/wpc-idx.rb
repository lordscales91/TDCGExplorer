#!/usr/bin/ruby
# download from 3DCG Craftsmen's Guild
# http://3dcustom.ath.cx/wordpress/wp-content/uploader/upload.html

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + '/../lib')
require 'sn_uploader'

def each_fileno(text)
  file_href = './upload.cgi?mode=dl&file='
  file_re = Regexp.new(Regexp.escape(file_href) + '(\d+)')
  text.scan(/href="(.+?)"/) do |href, |
    if md = file_re.match(href)
      yield md[1]
    end
  end
end

uploader = SnUploader.new
uploader.host = "3dcustom.ath.cx"
uploader.root_path = "/uploader/mod"
uploader.base_html = "upload.html"
uploader.local_dir = '/Volumes/uploader/arc/wpc'
body = uploader.get_base
each_fileno(body) do |fileno|
  basename_without_extension = "MODS%04d" % fileno.to_i
  filename_re = Regexp.new('\A' + Regexp.escape(basename_without_extension) + '\.')
  exist = uploader.local_file_match?(filename_re)
  puts [ fileno, exist ? 'exist' : 'download' ].join("\t")
  uploader.download(fileno) unless exist
end
