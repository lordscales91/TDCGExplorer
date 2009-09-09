#!/usr/bin/ruby
require File.dirname(__FILE__) + "/../config/environment"
require 'RMagick'

def process(image_path, image_1_path, image_2_path)
  ilist = Magick::ImageList.new(image_path)
  image = ilist.first
  image_1 = image.crop(0, 0, 128, 128)
  image_1.write(image_1_path)
  image_2 = image_1.resize(64, 64)
  image_2.write(image_2_path)
end

Bmp.find(:all).each do |bmp|
  image_path = RAILS_ROOT + "/public/images/bmps/#{bmp.id}.png"
  next unless test ?e, image_path
  puts image_path
  image_1_path = RAILS_ROOT + "/public/images/bmps/128x128/#{bmp.id}.png"
  image_2_path = RAILS_ROOT + "/public/images/bmps/64x64/#{bmp.id}.png"
  process(image_path, image_1_path, image_2_path)
  GC.start
end
