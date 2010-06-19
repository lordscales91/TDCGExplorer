class TsoColZero < ActiveRecord::Base
  belongs_to :tso
  belongs_to :col_zero, :class_name => 'Tso'
end
