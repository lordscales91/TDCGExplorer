class Tah < ActiveRecord::Base
  belongs_to :arc
  acts_as_list :scope => :arc
  has_many :tsos, :dependent => :destroy

  def collisions
    tsos.map(&:collisions).flatten.uniq.map(&:tah).uniq
  end

  def duplicates
    tsos.map(&:duplicates).flatten.uniq.map(&:tah).uniq
  end

  def rows
    tsos.map(&:row).compact.uniq
  end

  def row_names
    rows.map { |row| Tso.row_name(row) }
  end

  def row_caption
    row_names.join('/')
  end
end
