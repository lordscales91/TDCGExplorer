class Tbn < ActiveRecord::Base
  belongs_to :bmp
  acts_as_list :scope => :bmp
end
