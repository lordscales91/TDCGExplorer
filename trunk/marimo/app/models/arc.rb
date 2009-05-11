class Arc < ActiveRecord::Base
  has_many :tahs, :dependent => :destroy, :order => "position"

  has_many :arc_equips, :dependent => :destroy
  has_many :equips, :through => :arc_equips
  after_update :save_arc_equips
  validates_associated :arc_equips

  has_many :relationships, :dependent => :destroy, :foreign_key => "from_id"
  has_many :relations, :through => :relationships, :source => "to"
  has_many :rev_relationships, :dependent => :destroy, :class_name => 'Relationship', :foreign_key => "to_id"
  has_many :rev_relations, :through => :rev_relationships, :source => "from"
  after_update :save_relationships
  validates_associated :relationships

  has_many :arc_tags, :dependent => :destroy
  has_many :tags, :through => :arc_tags
  after_update :save_arc_tags
  validates_associated :arc_tags

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

  def arc_equip_attributes=(arc_equip_attributes)
    arc_equip_attributes.each do |attributes|
      unless id = attributes.delete(:id)
        arc_equips.build(attributes)
      else
        id = id.to_i
        arc_equip = arc_equips.detect { |ae| ae.id == id }
        arc_equip.attributes = attributes
      end
    end
  end

  def save_arc_equips
    arc_equips.each do |ae|
      if ae.should_destroy?
        ae.destroy
      else
        ae.save(false)
      end
    end
  end

  def relationship_attributes=(relationship_attributes)
    relationship_attributes.each do |attributes|
      unless id = attributes.delete(:id)
        relationships.build(attributes)
      else
        id = id.to_i
        relationship = relationships.detect { |ar| ar.id == id }
        relationship.attributes = attributes
      end
    end
  end

  def save_relationships
    relationships.each do |ar|
      if ar.should_destroy?
        ar.destroy
      else
        ar.save(false)
      end
    end
  end

  def arc_tag_attributes=(arc_tag_attributes)
    arc_tag_attributes.each do |attributes|
      unless id = attributes.delete(:id)
        arc_tags.build(attributes)
      else
        id = id.to_i
        arc_tag = arc_tags.detect { |ae| ae.id == id }
        arc_tag.attributes = attributes
      end
    end
  end

  def save_arc_tags
    arc_tags.each do |ae|
      if ae.should_destroy?
        ae.destroy
      else
        ae.save(false)
      end
    end
  end

  class Search
    attr_accessor :text

    def initialize(attributes)
      attributes.each do |name, value|
        send("#{name}=", value)
      end if attributes
    end

    def conditions
      @conditions ||= begin
        sql = "1"
        ret = [ sql ]
        terms = []
        unless text.blank?
          sql.concat " and (code like ? or summary like ?)"
          ret.push "%#{text}%"
          ret.push "%#{text}%"
        end
        ret
      end
    end

    def find_options
      { :conditions => conditions }
    end
  end
end
