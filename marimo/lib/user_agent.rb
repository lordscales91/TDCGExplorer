module UserAgent
module_function
  def default_http_header
    @_default_http_header ||= {
      "User-Agent" => "Mozilla/5.0 (Windows; U; Windows NT 6.0; ja; rv:1.9.0.10) Gecko/2009042316 Firefox/3.0.10 (.NET CLR 3.5.30729)",
      "Accept" => "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
      "Accept-Language" => "ja,en-us;q=0.7,en;q=0.3",
      "Accept-Charset" => "Shift_JIS,utf-8;q=0.7,*;q=0.7",
    }
  end
end
