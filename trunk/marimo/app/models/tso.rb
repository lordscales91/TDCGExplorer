begin
  require "tahhash.so"
rescue LoadError
  #
end

class Tso < ActiveRecord::Base
  belongs_to :tah
  acts_as_list :scope => :tah

  has_many :tso_col_zeros, :dependent => :destroy
  has_many :col_zeros, :through => :tso_col_zeros
  after_update :update_col_zeros

  def before_save
    self.tah_hash = '%08X' % TAHHash.calc(path) if defined?(TAHHash)
    self.tah_hash ||= ''
    self.md5 ||= ''
    nil
  end

  def collisions_and_duplicates
    @_collisions_and_duplicates ||=
      begin
        ary = self.class.find(:all, :conditions => ['tah_hash = ?', tah_hash])
        ary.delete(self)
        ary
      end
  end

  def collisions
    path_downcase = path.downcase
    collisions_and_duplicates.reject { |t| t.path.downcase == path_downcase }
  end

  def duplicates
    path_downcase = path.downcase
    collisions_and_duplicates.select { |t| t.path.downcase == path_downcase }
  end

  def row
    case path
    when %r[data/bgmodel/]
      nil
    when %r[data/model/]
      row = File.basename(path, '.tso')[9,1]
      row ? row.upcase : nil
    end
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
    row ? row + ":" + ROW_NAMES[row] : nil
  end

  def row_name
    row ? row + ":" + ROW_NAMES[row] : nil
  end

  def col_zero_path
    case path
    when %r[data/bgmodel/]
      nil
    when %r[data/model/]
      path.sub(/\d{2}\.tso$/, '00.tso')
    end
  end

  def find_col_zeros
    return [ ] unless defined?(TAHHash)
    path = col_zero_path
    return [ ] if path.nil?
    tah_hash = '%08X' % TAHHash.calc(path)
    ary = self.class.find(:all, :conditions => ['tah_hash = ?', tah_hash])
    path_downcase = path.downcase
    ary = ary.select { |t| t.path.downcase == path_downcase }
    ary.delete(self)
    ary
  end

  def update_col_zeros
    old_col_zeros = col_zeros
    new_col_zeros = find_col_zeros
    old_ids = old_col_zeros.map(&:id)
    new_ids = new_col_zeros.map(&:id)
    add_ids = new_ids - old_ids
    sub_ids = old_ids - new_ids
    sub_ids.each do |col_zero_id|
      tso_col_zero = tso_col_zeros.detect { |tc| tc.col_zero_id == col_zero_id }
      tso_col_zero.destroy
    end
    add_ids.each do |col_zero_id|
      tso_col_zero = tso_col_zeros.create(:col_zero_id => col_zero_id)
    end
  end

  class Search
    attr_accessor :path, :tah_hash, :md5

    def initialize(attributes)
      attributes.each do |name, value|
        send("#{name}=", value)
      end if attributes
      self.path ||= ''
      self.tah_hash = '%08X' % TAHHash.calc(path) if defined?(TAHHash) && /\.tso$/.match(path)
      self.tah_hash ||= ''
      self.md5 ||= ''
    end

    def conditions
      @conditions ||= begin
        sql = "1"
        ret = [ sql ]
        unless path.blank?
          sql.concat " and path like ?"
          ret.push "%#{path}%"
        end
        unless md5.blank?
          sql.concat " and md5 = ?"
          ret.push "#{md5}"
        end
        ret
      end
    end

    def find_options
      { :conditions => conditions }
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
