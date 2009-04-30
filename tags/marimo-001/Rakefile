# Add your own tasks in files placed in lib/tasks ending in .rake,
# for example lib/tasks/capistrano.rake, and they will automatically be available to Rake.

require(File.join(File.dirname(__FILE__), 'config', 'boot'))
# Rails::Initializer.run(:add_gem_load_paths)
Gem.activate("gettext", "= 1.93.0")
require 'rake'
require 'rake/testtask'
require 'rake/rdoctask'

require 'tasks/rails'
