class Tah < ActiveRecord::Base
  belongs_to :arc
  has_many :tsos
end
