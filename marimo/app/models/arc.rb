class Arc < ActiveRecord::Base
  has_many :tahs, :dependent => :destroy, :order => "position"
  has_many :arc_equips, :dependent => :destroy
  has_many :equips, :through => :arc_equips
  has_many :relationships, :dependent => :destroy, :foreign_key => "from_id"
  has_many :relations, :through => :relationships, :source => "to"
  has_many :rev_relationships, :dependent => :destroy, :class_name => 'Relationship', :foreign_key => "to_id"
  has_many :rev_relations, :through => :rev_relationships, :source => "from"
  has_many :arc_tags
  has_many :tags, :through => :arc_tags

  def collisions
    tahs.map(&:collisions).flatten.uniq.map(&:arc).uniq
  end

  def duplicates
    tahs.map(&:duplicates).flatten.uniq.map(&:arc).uniq
  end

  def rows
    tahs.map(&:rows).flatten.uniq
  end

  def row_names
    rows.map { |row| Tso.row_name(row) }
  end

  def row_caption
    row_names.join('/')
  end

  class Search
    attr_accessor :code, :summary

    def initialize(attributes)
      attributes.each do |name, value|
        send("#{name}=", value)
      end if attributes
    end

    def conditions
      @conditions ||= begin
        sql = "1"
        ret = [ sql ]
        unless code.blank?
          sql.concat " and code like ?"
          ret.push "%#{code}%"
        end
        unless summary.blank?
          sql.concat " and summary like ?"
          ret.push "%#{summary}%"
        end
        ret
      end
    end

    def find_options
      { :conditions => conditions }
    end
  end
end
