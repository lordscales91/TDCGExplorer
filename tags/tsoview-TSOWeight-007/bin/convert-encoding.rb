require 'nkf'

sjis_entries = []

Dir.glob("**/*.cs") do |ent|
  str = IO.read(ent)
  guess = NKF.guess(str)
  case guess
  when NKF::SJIS
    sjis_entries.push ent
  end
end

sjis_entries.each do |ent|
  p ent
  str = IO.read(ent)
  str = NKF.nkf('-Sw -m0', str)
  open(ent, "w") do |o|
    o.puts str
  end
end
