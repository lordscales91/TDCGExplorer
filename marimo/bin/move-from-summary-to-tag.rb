#!ruby
# encoding: utf-8
require File.dirname(__FILE__) + "/../config/environment"

Arc.find(:all, :conditions => ["summary like ?", '%【%']).each do |arc|
  tag_names = Cap2tag.summary_to_tag_names(arc.summary)
  if tag_names.empty?
    next
  end
  puts "#{arc.id} #{arc.code} found tag names. #{tag_names.join(' ')}"
  tag_names.each do |tag_name|
    tag = Tag.find_or_create_by_name(tag_name)
    tag.arc_tags.find_or_create_by_arc_id(arc.id)
  end
  arc.summary = arc.summary.gsub(/【(.+?)】/, '')
  arc.save
end
