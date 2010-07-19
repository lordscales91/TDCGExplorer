begin
  require "tahhash.so"
rescue LoadError
  #
end

class Tso < ActiveRecord::Base
  belongs_to :tah
  acts_as_list :scope => :tah

  has_many :tso_col_bases, :dependent => :destroy
  has_many :col_bases, :through => :tso_col_bases
  after_update :update_col_bases

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
A �g��
E ��
D ����(������)
B �O��
C �㔯
U �A�z�ї�
F �u��
H �p���c
G �S�g�����E����
W �^�C�c�E�K�[�^�[
I �C��
J ���(�V���c��)
M ����(�X�J�[�g��)
K �S�g�ߑ�(�i�[�X����)
L �㒅�I�v�V����(�G�v������)
O �C
Q �ዾ
V ���
Y ���{��
P ��������(�X�q��)
N �K��
3 �C�������O��
R ���
S ���
X �r����(��b�Ȃ�)
T �w��(�H�Ȃ�)
0 ����
2 �ق���
1 ���d��
Z �莝���̏���
  EOT
  # Z �w�i
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

  def col_basis_path
    case path
    when %r[data/bgmodel/]
      nil
    when %r[data/model/]
      path.sub(/\d{2}\.tso$/, '00.tso')
    end
  end

  def find_col_bases
    return [ ] unless defined?(TAHHash)
    path = col_basis_path
    return [ ] if path.nil?
    tah_hash = '%08X' % TAHHash.calc(path)
    ary = self.class.find(:all, :conditions => ['tah_hash = ?', tah_hash])
    path_downcase = path.downcase
    ary = ary.select { |t| t.path.downcase == path_downcase }
    ary.delete(self)
    ary
  end

  def update_col_bases
    old_col_bases = col_bases
    new_col_bases = find_col_bases
    old_ids = old_col_bases.map(&:id)
    new_ids = new_col_bases.map(&:id)
    add_ids = new_ids - old_ids
    sub_ids = old_ids - new_ids
    sub_ids.each do |col_basis_id|
      tso_col_basis = tso_col_bases.detect { |tc| tc.col_basis_id == col_basis_id }
      tso_col_basis.destroy
    end
    add_ids.each do |col_basis_id|
      tso_col_basis = tso_col_bases.create(:col_basis_id => col_basis_id)
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