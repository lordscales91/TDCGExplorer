class TsoColBasis < ActiveRecord::Base
  belongs_to :tso
  belongs_to :col_basis, :class_name => 'Tso'
end
