#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

while line = gets
  code, equip_caption, tag_caption, summary = line.chomp.split(/\t/)
  next unless tag_caption

  unless summary.nil?
    summary.strip!
    if md = /"(.+?)"/.match(summary)
      summary = md[1]
    end
  end

  arc = Arc.find_by_code(code)
  unless arc
    puts "code not found. #{code}"
    next
  end

  # puts "#{arc.code}:#{arc.summary}:#{summary}"
  unless summary.nil?
    arc.summary = summary
    arc.save!
  end

  old_tag_names = arc.tags.map { |t| t.name }

  tag_names = tag_caption.split('/')
  add_names = tag_names - old_tag_names
  sub_names = old_tag_names - tag_names

  add_names.each do |tag_name|
    tag = Tag.find_or_create_by_name(tag_name)
    arc_tag = ArcTag.create(:arc_id => arc.id, :tag_id => tag.id)
    puts "created arc_tag arc.code=#{code} tag.name=#{tag.name}"
  end
end
