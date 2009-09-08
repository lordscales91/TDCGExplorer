class Player < ActiveRecord::Base
  has_many :cards, :order => "position"
end
