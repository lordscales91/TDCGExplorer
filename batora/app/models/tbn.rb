class Tbn < ActiveRecord::Base
  belongs_to :bmp
  acts_as_list :scope => :bmp

  def basename
    File.basename(name)
  end

  def col
    basename[10,2].to_i
  end
end
