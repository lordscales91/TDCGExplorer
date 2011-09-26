#!/usr/bin/ruby
# download from 3ch
# http://www.esc-j.net/tech-arts/ta3dc/t1a931d9c1s9.html

$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + '/../lib')
require 'sn_uploader'

def each_fileno(text)
  file_href = 'http://www.esc-j.net/tech-arts/ta3dc/upload.cgi?mode=dl&file='
  file_re = Regexp.new(Regexp.escape(file_href) + '(\d+)')
  text.scan(/href="(.+?)"/) do |href, |
    if md = file_re.match(href)
      yield md[1]
    end
  end
end

uploader = SnUploader.new
uploader.host = "www.esc-j.net"
uploader.root_path = "/tech-arts/ta3dc"
uploader.base_html = "t1a931d9c1s9.html"
uploader.kcode = 'U'
uploader.authorization = "Basic " + ["tech:mybride"].pack('m').chomp
uploader.local_dir = '/Volumes/uploader/arc/3ch'
body = uploader.get_base
each_fileno(body) do |fileno|
  prefix = (fileno.to_i > 735) ? "3DCH" : "TA3CH"
  basename_without_extension = "#{prefix}%04d" % fileno.to_i
  filename_re = Regexp.new('\A' + Regexp.escape(basename_without_extension) + '\.')
  exist = uploader.local_file_match?(filename_re)
  puts [ fileno, exist ? 'exist' : 'download' ].join("\t")
  uploader.download(fileno) unless exist
end
