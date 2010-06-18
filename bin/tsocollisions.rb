#!/usr/bin/ruby
require File.dirname(__FILE__) + '/../config/environment'
Tso.find(:all).each do |tso|
  unless tso.collisions.empty?
    tso.collisions.each do |collision|
      puts "#{tso.id}\t#{tso.tah_hash}\t#{tso.path}\t#{collision.path}"
    end
  end
end
