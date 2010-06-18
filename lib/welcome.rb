class Welcome
  class Search
    attr_accessor :text

    def initialize(attributes)
      attributes.each do |name, value|
        send("#{name}=", value)
      end if attributes
    end
  end
end
