class Tag < ActiveRecord::Base
  has_many :arc_tags
  has_many :arcs, :through => :arc_tags
  validates_uniqueness_of :name

  def before_save
    self.locked = false if locked.nil?
    nil
  end

  class Search
    attr_accessor :name

    def initialize(attributes)
      attributes.each do |name, value|
        send("#{name}=", value)
      end if attributes
    end

    def text=(text)
      self.name = text
    end

    def conditions
      @conditions ||= begin
        sql = "1"
        ret = [ sql ]
        unless name.blank?
          sql.concat " and name like ?"
          ret.push "%#{name}%"
        end
        ret
      end
    end

    def find_options
      { :conditions => conditions }
    end
  end
end
