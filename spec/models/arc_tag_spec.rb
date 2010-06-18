require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe ArcTag, "validate" do
  fixtures :arcs, :arc_tags, :tags

  # it "同じ arc, tag の組み合わせを持つ arc_tag があるなら valid でない" do
  #   ArcTag.new(:arc_id => 1, :tag_id => 1).should_not be_valid
  # end

  it "one は valid である" do
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

  it "one.tag_name は tag1 である" do
    arc_tags(:one).tag_name.should == "tag1"
  end
end

describe ArcTag, "assign tag_name" do
  fixtures :arcs, :arc_tags, :tags

  it "one.tag_name を '' に設定すると one.tag_id は nil になる" do
    arc_tags(:one).tag_name = ''
    arc_tags(:one).tag_id.should be_nil
  end

  it "one.tag_name を ' ' に設定すると one.tag_id は nil になる" do
    arc_tags(:one).tag_name = ' '
    arc_tags(:one).tag_id.should be_nil
  end

  it "one.tag_name を 'tag1' に設定すると one.tag_id は 1 になる" do
    arc_tags(:one).tag_name = 'tag1'
    arc_tags(:one).tag_id.should == 1
  end

  it "one.tag_name を 'tag1 ' に設定すると one.tag_id は 1 になる" do
    arc_tags(:one).tag_name = 'tag1 '
    arc_tags(:one).tag_id.should == 1
  end

  it "one.tag_name を 'tag1　' に設定すると one.tag_id は 1 になる" do
    arc_tags(:one).tag_name = 'tag1　'
    arc_tags(:one).tag_id.should == 1
  end

  it "one.tag_name を ' tag1' に設定すると one.tag_id は 1 になる" do
    arc_tags(:one).tag_name = ' tag1'
    arc_tags(:one).tag_id.should == 1
  end

  it "one.tag_name を '　tag1' に設定すると one.tag_id は 1 になる" do
    arc_tags(:one).tag_name = '　tag1'
    arc_tags(:one).tag_id.should == 1
  end
end
