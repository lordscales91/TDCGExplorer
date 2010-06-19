#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

KIND_NAME_TO_ID = {
  'd•¡' => 1,
  'V‹Œ' => 2,
  '‘O’ñ' => 3,
}
def kind_name_to_id(name)
  KIND_NAME_TO_ID[name]
end

while line = gets
  kind_name, from_code, to_code, note = line.chomp.split(/\t/)
  note.strip! unless note.nil?
  puts "creating relationship #{kind_name} #{from_code} #{to_code} #{note}"

  kind = kind_name_to_id(kind_name)
  unless kind
    puts "error: kind not found"
    next
  end

  from = Arc.find_by_code(from_code)
  unless from
    puts "error: arc (from) not found"
    next
  end

  to = Arc.find_by_code(to_code)
  unless to
    puts "error: arc (to) not found"
    next
  end

  Relationship.create!(:kind => kind, :from_id => from.id, :to_id => to.id, :note => note)
end
