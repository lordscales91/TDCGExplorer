require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tahdump do

  it "指定 code を持つ既存の arc がない場合 arc を作成する" do
    data = <<'EOT'
# zip 3ch\TA0002.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc = Arc.find_by_code("TA0002")
    arc.should_not be_nil
  end

  it "指定 code を持つ既存の arc がある場合 arc を更新する" do
    arc = Arc.create(:code => "TA0002")

    data = <<'EOT'
# zip 3ch\TA0002.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    new_arc = Arc.find_by_code("TA0002")
    arc.should == new_arc
  end

  it "extname を設定" do
    arc = Arc.create(:code => "TA0002")

    data = <<'EOT'
# zip 3ch\TA0002.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.reload
    arc.extname.should == "zip"
  end

  it "location を設定" do
    arc = Arc.create(:code => "TA0002")

    data = <<'EOT'
# zip 3ch\TA0002.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.reload
    arc.location.should == "3ch"
  end

  it "summary を設定" do
    arc = Arc.create(:code => "TA0026")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.reload
    arc.summary.should == "同時着用したいものを～。変更＆追加。"
  end

  it "origname を設定" do
    arc = Arc.create(:code => "TA0026")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.reload
    arc.origname.should == "眉項目へ追加"
  end

  it "指定 path を持つ既存の tah がない場合 tah を作成する" do
    arc = Arc.create(:code => "TA0026")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
# TAH in archive 眉項目へ追加/hoku.tah
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    tah = arc.tahs.find_by_path("眉項目へ追加/hoku.tah")
    tah.should_not be_nil
  end

  it "指定 path を持つ既存の tah がある場合 tah を更新する" do
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "眉項目へ追加/hoku.tah")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
# TAH in archive 眉項目へ追加/hoku.tah
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    new_tah = arc.tahs.find_by_path("眉項目へ追加/hoku.tah")
    tah.should == new_tah
  end

  it "tah 数が減少している場合 削除する" do
    arc = Arc.create(:code => "TA0026")
    tah_1 = arc.tahs.create(:path => "眉項目へ追加/hoku.tah")
    tah_2 = arc.tahs.create(:path => "眉項目へ追加/kiba.tah")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
# TAH in archive 眉項目へ追加/hoku.tah
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.tahs.should have(1).items
  end

  it "tah 数が増加している場合 追加する" do
    arc = Arc.create(:code => "TA0026")
    tah_1 = arc.tahs.create(:path => "眉項目へ追加/hoku.tah")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
# TAH in archive 眉項目へ追加/hoku.tah
# TAH in archive 眉項目へ追加/kiba.tah
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.tahs.should have(2).items
  end

  it "指定 path を持つ既存の tso がない場合 tso を作成する" do
    TAHHash.stub!(:calc).and_return(0xBC0EEF52)
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "眉項目へ追加/hoku.tah")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
# TAH in archive 眉項目へ追加/hoku.tah
6df59c0777761af5d8d55585b52c613d data/model/N005HOKU_200.tso
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    tso = tah.tsos.find_by_path("data/model/N005HOKU_200.tso")
    tso.should_not be_nil
  end

  it "指定 path を持つ既存の tso がある場合 tso を更新する" do
    TAHHash.stub!(:calc).and_return(0xBC0EEF52)
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "眉項目へ追加/hoku.tah")
    tso = tah.tsos.create(:path => "data/model/N005HOKU_200.tso")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
# TAH in archive 眉項目へ追加/hoku.tah
6df59c0777761af5d8d55585b52c613d data/model/N005HOKU_200.tso
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    new_tso = tah.tsos.find_by_path("data/model/N005HOKU_200.tso")
    tso.should == new_tso
  end

  it "tso 数が減少している場合 削除する" do
    TAHHash.stub!(:calc).and_return(0xBC0EEF52)
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "眉項目へ追加/hoku.tah")
    tso_1 = tah.tsos.create(:path => "data/model/N005HOKU_200.tso")
    tso_2 = tah.tsos.create(:path => "data/model/N005HOKU_201.tso")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
# TAH in archive 眉項目へ追加/hoku.tah
6df59c0777761af5d8d55585b52c613d data/model/N005HOKU_200.tso
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    tah.tsos.should have(1).items
  end

  it "tso 数が増加している場合 追加する" do
    TAHHash.stub!(:calc).and_return(0xBC0EEF52)
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "眉項目へ追加/hoku.tah")
    tso_1 = tah.tsos.create(:path => "data/model/N005HOKU_200.tso")

    data = <<'EOT'
# zip 3ch\TA0026_同時着用したいものを～。変更＆追加。@眉項目へ追加.zip
# TAH in archive 眉項目へ追加/hoku.tah
6df59c0777761af5d8d55585b52c613d data/model/N005HOKU_200.tso
7ad8006d091820d78962755b23fb6d5e data/model/N005HOKU_201.tso
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    tah.tsos.should have(2).items
  end
end
