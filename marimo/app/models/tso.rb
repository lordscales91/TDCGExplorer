begin
  require "tahhash.so"
rescue LoadError
  #
end

class Tso < ActiveRecord::Base
  belongs_to :tah

  def before_save
    self.tah_hash = TAHHash.calc(path) if defined? TAHHash
    self.tah_hash ||= ''
    nil
  end
end
