class Card < ActiveRecord::Base
  belongs_to :player
  acts_as_list :scope => :player
  belongs_to :character
end
