require "tahhash.so"
key = TAHHash.calc(ARGV.shift || "script/items/N000KASU_E00.tbn")
puts "%08X" % key
