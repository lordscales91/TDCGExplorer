require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tahdump do

  it "�w�� code ���������� arc ���Ȃ��ꍇ arc ���쐬����" do
    data = <<'EOT'
# zip 3ch\TA0002.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc = Arc.find_by_code("TA0002")
    arc.should_not be_nil
  end

  it "�w�� code ���������� arc ������ꍇ arc ���X�V����" do
    arc = Arc.create(:code => "TA0002")

    data = <<'EOT'
# zip 3ch\TA0002.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    new_arc = Arc.find_by_code("TA0002")
    arc.should == new_arc
  end

  it "extname ��ݒ�" do
    arc = Arc.create(:code => "TA0002")

    data = <<'EOT'
# zip 3ch\TA0002.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.reload
    arc.extname.should == "zip"
  end

  it "location ��ݒ�" do
    arc = Arc.create(:code => "TA0002")

    data = <<'EOT'
# zip 3ch\TA0002.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.reload
    arc.location.should == "3ch"
  end

  it "summary ��ݒ�" do
    arc = Arc.create(:code => "TA0026")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.reload
    arc.summary.should == "�������p���������̂��`�B�ύX���ǉ��B"
  end

  it "origname ��ݒ�" do
    arc = Arc.create(:code => "TA0026")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.reload
    arc.origname.should == "�����ڂ֒ǉ�"
  end

  it "�w�� path ���������� tah ���Ȃ��ꍇ tah ���쐬����" do
    arc = Arc.create(:code => "TA0026")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
# TAH in archive �����ڂ֒ǉ�/hoku.tah
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    tah = arc.tahs.find_by_path("�����ڂ֒ǉ�/hoku.tah")
    tah.should_not be_nil
  end

  it "�w�� path ���������� tah ������ꍇ tah ���X�V����" do
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "�����ڂ֒ǉ�/hoku.tah")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
# TAH in archive �����ڂ֒ǉ�/hoku.tah
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    new_tah = arc.tahs.find_by_path("�����ڂ֒ǉ�/hoku.tah")
    tah.should == new_tah
  end

  it "tah �����������Ă���ꍇ �폜����" do
    arc = Arc.create(:code => "TA0026")
    tah_1 = arc.tahs.create(:path => "�����ڂ֒ǉ�/hoku.tah")
    tah_2 = arc.tahs.create(:path => "�����ڂ֒ǉ�/kiba.tah")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
# TAH in archive �����ڂ֒ǉ�/hoku.tah
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.tahs.should have(1).items
  end

  it "tah �����������Ă���ꍇ �ǉ�����" do
    arc = Arc.create(:code => "TA0026")
    tah_1 = arc.tahs.create(:path => "�����ڂ֒ǉ�/hoku.tah")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
# TAH in archive �����ڂ֒ǉ�/hoku.tah
# TAH in archive �����ڂ֒ǉ�/kiba.tah
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    arc.tahs.should have(2).items
  end

  it "�w�� path ���������� tso ���Ȃ��ꍇ tso ���쐬����" do
    TAHHash.stub!(:calc).and_return(0xBC0EEF52)
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "�����ڂ֒ǉ�/hoku.tah")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
# TAH in archive �����ڂ֒ǉ�/hoku.tah
6df59c0777761af5d8d55585b52c613d data/model/N005HOKU_200.tso
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    tso = tah.tsos.find_by_path("data/model/N005HOKU_200.tso")
    tso.should_not be_nil
  end

  it "�w�� path ���������� tso ������ꍇ tso ���X�V����" do
    TAHHash.stub!(:calc).and_return(0xBC0EEF52)
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "�����ڂ֒ǉ�/hoku.tah")
    tso = tah.tsos.create(:path => "data/model/N005HOKU_200.tso")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
# TAH in archive �����ڂ֒ǉ�/hoku.tah
6df59c0777761af5d8d55585b52c613d data/model/N005HOKU_200.tso
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    new_tso = tah.tsos.find_by_path("data/model/N005HOKU_200.tso")
    tso.should == new_tso
  end

  it "tso �����������Ă���ꍇ �폜����" do
    TAHHash.stub!(:calc).and_return(0xBC0EEF52)
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "�����ڂ֒ǉ�/hoku.tah")
    tso_1 = tah.tsos.create(:path => "data/model/N005HOKU_200.tso")
    tso_2 = tah.tsos.create(:path => "data/model/N005HOKU_201.tso")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
# TAH in archive �����ڂ֒ǉ�/hoku.tah
6df59c0777761af5d8d55585b52c613d data/model/N005HOKU_200.tso
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    tah.tsos.should have(1).items
  end

  it "tso �����������Ă���ꍇ �ǉ�����" do
    TAHHash.stub!(:calc).and_return(0xBC0EEF52)
    arc = Arc.create(:code => "TA0026")
    tah = arc.tahs.create(:path => "�����ڂ֒ǉ�/hoku.tah")
    tso_1 = tah.tsos.create(:path => "data/model/N005HOKU_200.tso")

    data = <<'EOT'
# zip 3ch\TA0026_�������p���������̂��`�B�ύX���ǉ��B@�����ڂ֒ǉ�.zip
# TAH in archive �����ڂ֒ǉ�/hoku.tah
6df59c0777761af5d8d55585b52c613d data/model/N005HOKU_200.tso
7ad8006d091820d78962755b23fb6d5e data/model/N005HOKU_201.tso
EOT
    tahdump = Tahdump.new(data)
    tahdump.commit

    tah.tsos.should have(2).items
  end
end
