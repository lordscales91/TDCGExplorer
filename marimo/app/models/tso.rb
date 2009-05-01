begin
  require "tahhash.so"
rescue LoadError
  #
end

class Tso < ActiveRecord::Base
  belongs_to :tah

  def before_save
    self.tah_hash = '%08X' % TAHHash.calc(path) if defined? TAHHash
    self.tah_hash ||= ''
    nil
  end

  def collisions_and_duplicates
    @_collisions_and_duplicates ||= self.class.find(:all, :conditions => ['tah_hash = ? and id <> ?', tah_hash, id])
  end

  def collisions
    collisions_and_duplicates.reject { |t| t.path.downcase == path.downcase }
  end

  def duplicates
    collisions_and_duplicates.select { |t| t.path.downcase == path.downcase }
  end

  def row
    File.basename(path, '.tso')[9,1].upcase
  end

  ROW_TBL = <<-EOT
A 身体
E 瞳
D 頭皮(生え際)
B 前髪
C 後髪
U アホ毛類
F ブラ
H パンツ
G 全身下着・水着
W タイツ・ガーター
I 靴下
J 上衣(シャツ等)
M 下衣(スカート等)
K 全身衣装(ナース服等)
L 上着オプション(エプロン等)
O 靴
Q 眼鏡
V 眼帯
Y リボン
P 頭部装備(帽子等)
N 尻尾
3 イヤリング類
R 首輪
S 手首
X 腕装備(手甲など)
T 背中(羽など)
0 眉毛
2 ほくろ
1 八重歯
Z 手持ちの小物
  EOT
  # Z 背景
  ROW_NAMES = {}
  ROW_TBL.each_line do |line|
    row, name = line.chomp.split(/ /)
    ROW_NAMES[row] = name
  end

  def self.row_name(row)
    row + ":" + ROW_NAMES[row]
  end

  def row_name
    row + ":" + ROW_NAMES[row]
  end

  class Search
    attr_accessor :path, :tah_hash

    def initialize(attributes)
      attributes.each do |name, value|
        send("#{name}=", value)
      end if attributes
      self.path ||= ''
      self.tah_hash = '%08X' % TAHHash.calc(path) if defined? TAHHash
      self.tah_hash ||= ''
    end

    def collisions_and_duplicates
      @_collisions_and_duplicates ||= Tso.find(:all, :conditions => ['tah_hash = ?', tah_hash])
    end

    def collisions
      collisions_and_duplicates.reject { |t| t.path.downcase == path.downcase }
    end

    def duplicates
      collisions_and_duplicates.select { |t| t.path.downcase == path.downcase }
    end
  end
end
