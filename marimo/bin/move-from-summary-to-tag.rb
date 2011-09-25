#!ruby
# encoding: utf-8
require File.dirname(__FILE__) + "/../config/environment"

CAP2TAG_NAMES = {}

open("arc/summary-tag-uniq-mapping.txt") do |f|
  while line = f.gets
    row = line.chomp.split(/\t+/)
    caption = row.shift
    CAP2TAG_NAMES[caption] = row
  end
end

def caption_to_tag_names(caption)
  CAP2TAG_NAMES[caption]
end

Arc.find(:all, :conditions => ["summary like ?", '%【%']).each do |arc|
  arc.summary.scan(/【(.+?)】/) do |caption, |
    tag_names = caption_to_tag_names(caption)
    if tag_names.nil?
      puts "#{arc.id} #{arc.code} not found tag_names. #{caption}"
      next
    end
    puts "#{arc.id} #{arc.code} converted. #{tag_names.join(' ')}"
    tag_names.each do |tag_name|
      tag = Tag.find_or_create_by_name(tag_name)
      tag.arc_tags.find_or_create_by_arc_id(arc.id)
    end
    arc.summary = arc.summary.gsub(/【(.+?)】/, '')
    arc.save
  end
end
