#!/usr/bin/ruby -KS
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

Arc.find(:all, :conditions => ["summary like ?", '%Åy%']).each do |arc|
=begin
  arc.summary.scan(/Åy(.+?)Åz/) do |caption, |
    tag_names = caption_to_tag_names(caption)
    if tag_names.nil?
      puts "not found tag_names. #{caption}"
    end
    tag_names.each do |tag_name|
      tag = Tag.find_or_create_by_name(tag_name)
      tag.arc_tags.find_or_create_by_arc_id(arc.id)
    end
  end
=end
  arc.summary = arc.summary.gsub(/Åy(.+?)Åz/, '')
  arc.save
end
