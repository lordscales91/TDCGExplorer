begin
  require "tahhash.so"
rescue LoadError
  #
end

class Tso < ActiveRecord::Base
  belongs_to :tah

  def before_save
    self.tah_hash = '%08X' % TAHHash.calc(path) if defined? TAHHash
    self.tah_hash ||= ''
    nil
  end

  def collisions
    self.class.find(:all, :conditions => ['tah_hash = ? and id <> ?', tah_hash, id])
  end
end
