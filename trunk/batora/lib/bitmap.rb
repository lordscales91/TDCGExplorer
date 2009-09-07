class Bitmap
  def initialize
    @tbn_names = []
  end

  attr_reader :tbn_names

  def read_int
    @reader.read(4).unpack("i").first
  end

  def read_ushort
    @reader.read(2).unpack("S").first
  end

  def read_data
    str = ""
    32.times do
      c = 0x00
      8.times do |i|
        buf = @reader.read(1)
        tmp = buf[0]
        # print "%02X (%1d) " % [ tmp, tmp & 0x01 ]
        c += (tmp & 0x01) << i
      end
      # puts ": %02X " % c
      str.concat c.chr
    end
    str.sub(/\0+$/,'')
  end

  def load(path)
    @reader = open(path, "rb")

    magic = @reader.read(2)
    unless magic == "BM"
      raise ArgumentError.new("magic not match")
    end

    file_size = read_int
    # puts "file_size: #{file_size} bytes"

    @reader.read(4)

    offset = read_int

    header_size = read_int
    unless header_size == 40
      raise ArgumentError.new("header_size is not 40")
    end

    width = read_int
    unless width == 128
      raise ArgumentError.new("width is not 128")
    end

    height = read_int
    unless height == 256
      raise ArgumentError.new("height is not 256")
    end

    nplane = read_ushort
    unless nplane == 1
      raise ArgumentError.new("nplane is not 1")
    end

    nbit = read_ushort
    # puts "#{nplane} plane #{nbit} bits"

    compression_method = read_int
    image_size = read_int
    horizontal_resolution = read_int
    vertical_resolution = read_int
    ncolor = read_int
    nimportant_color = read_int

    @tbn_names.clear
    32.times do
      @tbn_names.push read_data
    end
  ensure
    @reader.close if @reader
  end
end

if __FILE__ == $0
  bmp = Bitmap.new
  bmp.load(ARGV.shift)
  bmp.tbn_names.each { |tbn| puts tbn }
end
