#!/usr/bin/ruby
require File.dirname(__FILE__) + '/../config/environment'
Tso.find(:all).each do |tso|
  unless tso.collisions.empty?
    puts "# #{tso.path}\t#{tso.tah_hash}"
    tso.collisions.each do |collision|
      puts "#{collision.path}\t#{collision.tah_hash}"
    end
  end
end
