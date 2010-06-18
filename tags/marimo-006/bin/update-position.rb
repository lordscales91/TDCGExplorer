#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

Arc.find(:all).each do |arc|
  arc.tahs.find(:all, :order => "id").each_with_index do |tah, i|
    tah.position = i+1
    tah.save
    puts "tah updated. tah.position=#{tah.position}"
    tah.tsos.find(:all, :order => "id").each_with_index do |tso, j|
      tso.position = j+1
      tso.save
      puts "tso updated. tso.position=#{tso.position}"
    end
  end
end
