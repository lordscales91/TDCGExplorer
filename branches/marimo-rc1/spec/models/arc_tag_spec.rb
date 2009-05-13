require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe ArcTag, "validate" do
  fixtures :arcs, :arc_tags, :tags

  it "���� arc, tag �̑g�ݍ��킹������ arc_tag ������Ȃ� valid �łȂ�" do
    ArcTag.new(:arc_id => 1, :tag_id => 1).should_not be_valid
  end

  it "one �� valid �ł���" do
    arc_tags(:one).should be_valid
  end
end

describe ArcTag do
  fixtures :arcs, :arc_tags, :tags

  it "one arc should eq arcs.one" do
    arc_tags(:one).arc.should == arcs(:one)
  end

  it "one tag should eq tags.one" do
    arc_tags(:one).tag.should == tags(:one)
  end
end

describe ArcTag, "tag_name" do
  fixtures :arcs, :arc_tags, :tags

  it "one.tag_name �� tag1 �ł���" do
    arc_tags(:one).tag_name.should == "tag1"
  end
end

describe ArcTag, "assign tag_name" do
  fixtures :arcs, :arc_tags, :tags

  it "one.tag_name �� '' �ɐݒ肷��� one.tag_id �� nil �ɂȂ�" do
    arc_tags(:one).tag_name = ''
    arc_tags(:one).tag_id.should be_nil
  end

  it "one.tag_name �� ' ' �ɐݒ肷��� one.tag_id �� nil �ɂȂ�" do
    arc_tags(:one).tag_name = ' '
    arc_tags(:one).tag_id.should be_nil
  end

  it "one.tag_name �� 'tag1' �ɐݒ肷��� one.tag_id �� 1 �ɂȂ�" do
    arc_tags(:one).tag_name = 'tag1'
    arc_tags(:one).tag_id.should == 1
  end

  it "one.tag_name �� 'tag1 ' �ɐݒ肷��� one.tag_id �� 1 �ɂȂ�" do
    arc_tags(:one).tag_name = 'tag1 '
    arc_tags(:one).tag_id.should == 1
  end

  it "one.tag_name �� 'tag1�@' �ɐݒ肷��� one.tag_id �� 1 �ɂȂ�" do
    arc_tags(:one).tag_name = 'tag1�@'
    arc_tags(:one).tag_id.should == 1
  end

  it "one.tag_name �� ' tag1' �ɐݒ肷��� one.tag_id �� 1 �ɂȂ�" do
    arc_tags(:one).tag_name = ' tag1'
    arc_tags(:one).tag_id.should == 1
  end

  it "one.tag_name �� '�@tag1' �ɐݒ肷��� one.tag_id �� 1 �ɂȂ�" do
    arc_tags(:one).tag_name = '�@tag1'
    arc_tags(:one).tag_id.should == 1
  end
end
