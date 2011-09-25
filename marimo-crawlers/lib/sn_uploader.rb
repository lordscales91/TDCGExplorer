require 'net/http'
require 'erb'
require 'nkf'
require 'user_agent'

class SnUploader
  attr_accessor :host, :root_path, :jcode, :dlkey, :authorization, :local_dir, :base_html

  def initialize
    @jcode = ERB::Util.u("Š¿Žš")
    @dlkey = "custom"
    @authorization = nil
    @local_dir = "."
    @base_html = "upload.html"
  end

  def kcode=(kcode)
    case kcode
    when /^U/i
      @jcode = ERB::Util.u(NKF.nkf('-Sw', "Š¿Žš"))
    end
  end

  def header
    header = UserAgent.default_http_header.dup
    header["Authorization"] = authorization if authorization
    header
  end

  def local_path(path)
    File.join(local_dir, File.basename(path))
  end

  def local_file_exist?(filename)
    File.exist?(local_path(filename))
  end

  def local_file_match?(filename_re)
    Dir.foreach(local_dir) do |ent|
      if md = filename_re.match(ent)
        return md[0]
      end
    end
    false
  end

  def download(fileno, filename_re = nil)
    if filename_re && local_file_match?(filename_re)
      return nil
    end

    Net::HTTP.new(host).start do |http|
      response = http.post("#{root_path}/upload.cgi", "file=#{fileno}&jcode=#{jcode}&mode=dl&dlkey=#{dlkey}", header)
      body = response.body

      if md = /"1;URL=(.+?)"/.match(body)
        path = md[1]
        path.sub!(/\A\./, root_path)
      end
      puts path
      if path
        open(local_path(path), "wb") do |file|
          response = http.get(path, header) do |data|
            file.write data
          end
        end
      end
    end
  end

  def download_file_through_counter(filename)
    if local_file_exist?(filename)
      return nil
    end

    Net::HTTP.new(host).start do |http|
      response = http.get("#{root_path}/src/#{filename}.html", header)
      file_re = Regexp.new(Regexp.escape('count.cgi?') + '(.+)')
      text = response.body
      text.scan(/href="(.+?)"/) do |href, |
        if md = file_re.match(href)
          path = root_path + '/src/' + md[1]
          open(local_path(filename), "wb") do |file|
            response = http.get(path, header) do |data|
              file.write data
            end
          end
        end
      end
    end
  end

  def download_file_through_clicker(filename)
    if local_file_exist?(filename)
      return nil
    end

    http = Net::HTTP.new(host)
    http.open_timeout = 10
    http.read_timeout = 20
    begin
      http.start do
        path = "#{root_path}/src/#{filename}"
        open(local_path(filename), "wb") do |file|
          header = header()
          header["Referer"] = "http://#{host}/#{root_path}/src/#{filename}.html"

          response = http.get(path, header) do |data|
            file.write data
          end
        end
      end
    rescue Timeout::Error
      puts "timeout..."
      sleep(20)
      puts "retry"
      retry
    end
  end

  def download_file(filename)
    if local_file_exist?(filename)
      return nil
    end

    Net::HTTP.new(host).start do |http|
      response = http.get("#{root_path}/src/#{filename}.html", header)
      body = response.body
      if md = /href="(.+?)"/.match(body)
        path = md[1]
        path.sub!(/\A\./, root_path + "/src")
      end
      puts path
      if path
        open(local_path(path), "wb") do |file|
          response = http.get(path, header) do |data|
            file.write data
          end
        end
      end
    end
  end

  def download_file_directly(filename, dir = '/src')
    if local_file_exist?(filename)
      return nil
    end

    path = "#{root_path}#{dir}/#{filename}"
    puts path
    Net::HTTP.new(host).start do |http|
      open(local_path(path), "wb") do |file|
        response = http.get(path, header) do |data|
          file.write data
        end
      end
    end
  end

  def download_file_directly_id(filename)
    if local_file_exist?(filename)
      return nil
    end

    path = "#{root_path}/?id=#{filename}"
    puts path
    http = Net::HTTP.new(host)
    http.open_timeout = 10
    http.read_timeout = 20
    begin
      http.start do
        open(local_path(filename), "wb") do |file|
          response = http.get(path, header) do |data|
            file.write data
          end
        end
      end
    rescue Timeout::Error
      puts "timeout..."
      sleep(20)
      puts "retry"
      retry
    end
  end

  def get_base
    body = nil
    Net::HTTP.new(host).start do |http|
      response = http.get("#{root_path}/#{base_html}", header)
      # response.each do |name, value|
      #   puts [name, value].join(': ')
      # end
      # puts
      body = response.body
    end
    body
  end
end
