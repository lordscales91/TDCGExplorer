require 'rubygems'
gem 'activeresource'
require 'active_resource'
module Remoting
  class Arc < ActiveResource::Base
    # self.site = 'http://localhost:3000'
    self.site = 'http://3dcustom.ath.cx/rails'
  end
end
rarcs = Remoting::Arc.find(:all, :from => :recent)
p rarcs.size
gem 'activerecord'
require 'active_record'
config = YAML.load(IO.read('config/database.yml'))
environment = 'test'
p config[environment]
require 'logger'
ActiveRecord::Base.logger = Logger.new("debug.log")
ActiveRecord::Base.colorize_logging = false
ActiveRecord::Base.establish_connection(config[environment])
class Arc < ActiveRecord::Base
end
rarcs.each do |rarc|
  arc = Arc.find_or_initialize_by_id(rarc.id)
  attributes = rarc.attributes
  attributes.delete('id')
  p attributes
  p arc.update_attributes(attributes)
end
