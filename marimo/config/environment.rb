# Be sure to restart your server when you modify this file

# Uncomment below to force Rails into production mode when
# you don't control web/app server and can't set it the proper way
# ENV['RAILS_ENV'] ||= 'production'

# Specifies gem version of Rails to use when vendor/rails is not present
RAILS_GEM_VERSION = '2.3.18' unless defined? RAILS_GEM_VERSION

# Bootstrap the Rails environment, frameworks, and default configuration
require File.join(File.dirname(__FILE__), 'boot')

Rails::Initializer.run do |config|
  # Settings in config/environments/* take precedence over those specified here.
  # Application configuration should go into files in config/initializers
  # -- all .rb files in that directory are automatically loaded.
  # See Rails::Configuration for more options.

  # Skip frameworks you're not going to use. To use Rails without a database
  # you must remove the Active Record framework.
  config.frameworks -= [ :active_resource, :action_mailer ]

  # Specify gems that this application depends on. 
  # They can then be installed with "rake gems:install" on new installations.
  config.gem "will_paginate", :lib => "will_paginate", :version => "2.3.15"
  config.gem "locale"
  config.gem "locale_rails", :version => "2.0.5"
  config.gem "gettext"
  config.gem "gettext_activerecord", :version => "2.1.0"
  config.gem "gettext_rails", :version => "2.1.0"

  # Only load the plugins named here, in the order given. By default, all plugins 
  # in vendor/plugins are loaded in alphabetical order.
  # :all can be used as a placeholder for all plugins not explicitly named
  # config.plugins = [ :exception_notification, :ssl_requirement, :all ]

  # Add additional load paths for your own custom dirs
  # config.load_paths += %W( #{RAILS_ROOT}/extras )

  # Force all environments to use the same logger level
  # (by default production uses :info, the others :debug)
  # config.log_level = :debug

  # Make Time.zone default to the specified zone, and make Active Record store time values
  # in the database in UTC, and return them converted to the specified local zone.
  # Run "rake -D time" for a list of tasks for finding time zone names. Uncomment to use default local time.
  # config.time_zone = 'UTC'

  # Your secret key for verifying cookie session data integrity.
  # If you change this key, all old sessions will become invalid!
  # Make sure the secret is at least 30 characters and all random, 
  # no regular words or you'll be exposed to dictionary attacks.
  config.action_controller.session = {
    :session_key => '_marimo_session',
    :secret      => '52a4164af5d9a609337a8b5e67b4e73df5386d4eb1a37b26204558236f9fa78c2afe8a7c51560853d79af1970ef73d0cbd7ac74d386a88aad541d4594b08c638'
  }

  # Use the database for sessions instead of the cookie-based default,
  # which shouldn't be used to store highly confidential information
  # (create the session table with "rake db:sessions:create")
  # config.action_controller.session_store = :active_record_store

  # Use SQL instead of Active Record's schema dumper when creating the test database.
  # This is necessary if your schema can't be completely dumped by the schema dumper,
  # like if you have constraints or database-specific column types
  # config.active_record.schema_format = :sql

  # Activate observers that should always be running
  # config.active_record.observers = :cacher, :garbage_collector
end
$KCODE = 'UTF8'

ActiveRecord::Base.colorize_logging = false
ActionController::Base.default_charset = 'UTF-8'

WillPaginate::ViewHelpers.pagination_options[:previous_label] = '&laquo; 前へ'
WillPaginate::ViewHelpers.pagination_options[:next_label] = '次へ &raquo;'

class String
  def to_json(options = nil) #:nodoc:
    '"' + gsub(ActiveSupport::JSON::Encoding.escape_regex) { |s|
      ActiveSupport::JSON::Encoding::ESCAPED_CHARS[s]
    } + '"'
  end

  def to_xs
    gsub(/[&<>]/) { Builder::XChar::PREDEFINED[$&[0]] }
  end
end
