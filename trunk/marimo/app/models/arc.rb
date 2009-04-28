class Arc < ActiveRecord::Base
  has_many :tahs

  class Search
    attr_accessor :code

    def initialize(attributes)
      attributes.each do |name, value|
        send("#{name}=", value)
      end if attributes
    end

    def conditions
      @conditions ||= begin
        sql = "1"
        ret = [ sql ]
        if code
          sql.concat " and code like ?"
          ret.push "%#{code}%"
        end
        ret
      end
    end

    def find_options
      { :conditions => conditions }
    end
  end
end
