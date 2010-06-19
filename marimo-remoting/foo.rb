require 'rubygems'
gem 'activeresource'
require 'active_resource'
module Remote
  mattr_accessor :site
  # self.site = 'http://localhost:3000'
  self.site = 'http://3dcustom.ath.cx/rails'
  %w(Arc Tah Tso).each do |cname|
    c = const_set(cname, Class.new(ActiveResource::Base))
    c.site = Remote.site
  end
end
gem 'activerecord'
require 'active_record'
config = YAML.load(IO.read('config/database.yml'))
environment = 'test'
p config[environment]
require 'logger'
ActiveRecord::Base.logger = Logger.new("debug.log")
ActiveRecord::Base.colorize_logging = false
ActiveRecord::Base.establish_connection(config[environment])
module Local
  %w(Arc Tah Tso).each do |cname|
    c = const_set(cname, Class.new(ActiveRecord::Base))
  end
end
def mirror_models(cname)
  remote_class = Remote.const_get(cname)
  local_class = Local.const_get(cname)
  remote_instances = remote_class.find(:all, :from => :recent)
  remote_instances.each do |remote_instance|
    local_instance = local_class.find_or_initialize_by_id(remote_instance.id)
    attributes = remote_instance.attributes
    attributes.delete('id')
    p attributes
    p local_instance.update_attributes(attributes)
  end
end
%w(Arc Tah Tso).each do |cname|
  mirror_models(cname)
end
