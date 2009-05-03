class ArcTag < ActiveRecord::Base
  belongs_to :arc
  belongs_to :tag
end
