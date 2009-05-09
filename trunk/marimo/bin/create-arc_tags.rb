#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

while line = gets
  code, equip_caption, tag_caption, summary = line.chomp.split(/\t/)
  unless summary.nil?
    summary.strip!
    if md = /"(.+?)"/.match(summary)
      summary = md[1]
    end
  end
  arc = Arc.find_by_code(code)
  if arc
    # puts "#{arc.code}:#{arc.summary}:#{summary}"
    arc.summary = summary
    arc.save!
  end
  if arc && tag_caption
    tag_names = tag_caption.split('/')
    tag_names.each do |tag_name|
      tag = Tag.find_or_create_by_name(tag_name)
      ArcTag.create(:arc_id => arc.id, :tag_id => tag.id)
      puts "created arc_tag arc.code=#{code} tag.name=#{tag.name}"
    end
  end
end
