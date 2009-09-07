class Bmp < ActiveRecord::Base
  has_many :tbns, :order => "position"
  has_one :character

  def tbn_names=(tbn_names)
    tbn_names.each do |tbn_name|
      tbns.build(:name => tbn_name)
    end
  end
end
