require 'rubygems'
gem 'activeresource'
require 'active_resource'
module Remoting
  class Arc < ActiveResource::Base
    # self.site = 'http://localhost:3000'
    self.site = 'http://3dcustom.ath.cx/rails'
  end
  class Tah < ActiveResource::Base
    # self.site = 'http://localhost:3000'
    self.site = 'http://3dcustom.ath.cx/rails'
  end
  class Tso < ActiveResource::Base
    # self.site = 'http://localhost:3000'
    self.site = 'http://3dcustom.ath.cx/rails'
  end
end
rarcs = Remoting::Arc.find(:all, :from => :recent)
p rarcs.size
rtahs = Remoting::Tah.find(:all, :from => :recent)
p rtahs.size
rtsos = Remoting::Tso.find(:all, :from => :recent)
p rtsos.size
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
class Tah < ActiveRecord::Base
end
rtahs.each do |rtah|
  tah = Tah.find_or_initialize_by_id(rtah.id)
  attributes = rtah.attributes
  attributes.delete('id')
  p attributes
  p tah.update_attributes(attributes)
end
class Tso < ActiveRecord::Base
end
rtsos.each do |rtso|
  tso = Tso.find_or_initialize_by_id(rtso.id)
  attributes = rtso.attributes
  attributes.delete('id')
  p attributes
  p tso.update_attributes(attributes)
end
