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
