# vim: fileencoding=utf-8
require 'nkf'

class Scrap

  def match(line)
    @md = @row_re.match(line)
  end

  def row
    raise
  end

  def encode(str)
    str && NKF.nkf('-Sw', str)
  end

  def encode_to_sjis(str)
    str && NKF.nkf('-Ws', str)
  end
end

class Scrap_xpc < Scrap
  def initialize
    @row_re = %r(</td><td><a href="(.+?)">(.+?)</a></td><td>(.+?)</td><td>(.+?)</td><td>(.+?)</td><td>(.+?)</td><td>(.+?)</td></tr>)
    @key_re = Regexp.new(Regexp.escape("<font color=\"#FF0000\">[DLKey] </font>"))
    @tag_list_re = Regexp.new("<span class=\"tag_list\">(.+?)</span>")
  end

  def row
    _, href, name, comment, size, date, mime, orig = @md.to_a
    locked = !!comment.sub!(@key_re, '')
    comment.sub!(@tag_list_re, '\\1')
    [ name, encode(comment), size, date, encode(orig), locked ]
  end
end

def Scrap.find_by_site_code(site_code)
  case site_code
  when 'xpc'
    scrap = Scrap_xpc.new
  end
end
