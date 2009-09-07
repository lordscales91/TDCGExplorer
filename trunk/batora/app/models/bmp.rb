class Bmp < ActiveRecord::Base
  has_many :tbns, :order => "position"
  has_one :character

  def basename
    File.basename(path)
  end

  def tbn_names=(tbn_names)
    tbn_names.each do |tbn_name|
      tbns.build(:name => tbn_name)
    end
  end

  def tbns_to_oct(offset)
    tbns.to_a[offset,4].inject(0) { |sum, tbn| sum += tbn.col } % 8
  end

  def life
    tbns_to_oct(0) + 2
  end

  def offense
    tbns_to_oct(4) + 1
  end

  def defense
    tbns_to_oct(8) + 1
  end

  def agility
    tbns_to_oct(12) + 1
  end

  def setup_character
    cha = character
    cha.life = life
    cha.off = offense
    cha.def = defense
    cha.agi = agility
    cha
  end

  def build_character_1(attributes)
    build_character(attributes)
    setup_character
    character
  end
end
