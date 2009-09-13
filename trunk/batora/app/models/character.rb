class Character < ActiveRecord::Base
  belongs_to :bmp
  has_one :good

end
