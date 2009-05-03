require 'stringio'

class Tahdump

  class Arc
    attr_accessor :code, :extname, :location, :summary, :origname

    def initialize
      @tahs = []
    end

    def add_tah(tah)
      tah.arc = self
      @tahs.push tah
    end

    def commit
      arc = ::Arc.find_or_create_by_code(code)
      arc.extname = extname
      arc.location = location
      arc.summary = summary
      arc.origname = origname
      arc.save

      ( arc.tahs.size-1 ).downto( @tahs.size ) do |i|
        arc.tahs[i].destroy
      end if arc.tahs.size > @tahs.size

      @tahs.each_with_index do |tah, i|
        tah.commit(arc, i)
      end
    end
  end

  class Tah
    attr_accessor :arc, :path

    def initialize
      @tsos = []
    end

    def add_tso(tso)
      tso.tah = self
      @tsos.push tso
    end

    def commit(arc, i)
      tah = ::Tah.find_or_initialize_by_position(i+1)
      tah.arc = arc
      tah.path = path
      tah.save

      ( tah.tsos.size-1 ).downto( @tsos.size ) do |i|
        tah.tsos[i].destroy
      end if tah.tsos.size > @tsos.size

      @tsos.each_with_index do |tso, i|
        tso.commit(tah, i)
      end
    end
  end

  class Tso
    attr_accessor :tah, :path

    def commit(tah, i)
      tso = ::Tso.find_or_initialize_by_position(i+1)
      tso.tah = tah
      tso.path = path
      tso.save
    end
  end

  def initialize(data)
    @io = StringIO.new(data)
  end

  def commit
    process
  end

  def process
    arc = nil
    tah = nil
    tso = nil
    while line = @io.gets
      line.chomp!
      case line
      when /^# zip /
        arc.commit if arc
        arc = create_arc(line)
      when /^# rar /
        arc.commit if arc
        arc = create_arc(line)
      when /^# lzh /
        arc.commit if arc
        arc = create_arc(line)
      when /^# TAH in archive /
        tah = create_tah(line)
        arc.add_tah tah
      when /^# TAH /
        # .tah ÇÕë∂ç›ÇµÇ»Ç¢ÇÕÇ∏
        raise "raw TAH found."
      when /^Error:/
        next
      when /^   at /
        next
      when /\.tso$/
        tso = create_tso(line)
        tah.add_tso tso
      end
    end
    arc.commit if arc
  end

  def create_arc(line)
    md = /^# ([a-z]{3}) (.+)/.match(line)
    _, extname, tmp = md.to_a
    location, basename = tmp.split("\\")
    basename.sub!(Regexp.new(Regexp.escape("." + extname) + "$"), '')
    if md = /^([A-Za-z0-9]{6,9})$/.match(basename)
      _, code = md.to_a
      summary = nil
      origname = nil
    elsif md = /^([A-Za-z0-9]{6,9})_(.+)/.match(basename)
      _, code, tmp = md.to_a
      summary, origname = tmp.split("_", 2)
    else
      raise "pattern not matched. " + line
    end
    arc = Arc.new
    arc.code = code
    arc.extname = extname
    arc.location = location
    arc.summary = summary
    arc.origname = origname
    arc
  end

  def create_tah(line)
    md = /^# TAH in archive (.+)/.match(line)
    _, path = md.to_a
    tah = Tah.new
    tah.path = path
    tah
  end

  def create_tso(line)
    path = line
    tso = Tso.new
    tso.path = path
    tso
  end
end
