#!/usr/bin/ruby
require File.dirname(__FILE__) + "/../config/environment"

Character.find(:all).each do |cha|
  good = Good.create(:character => cha, :stock => 10, :price => 100)
end
