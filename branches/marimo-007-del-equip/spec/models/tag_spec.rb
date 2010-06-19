require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tag, "validate" do
  fixtures :tags

  it "同じ名前の tag があれば valid でない" do
    tags(:one).name = "tag2"
    tags(:one).should_not be_valid
  end
end

describe Tag, "arcs" do
  fixtures :arcs, :arc_tags, :tags

  it "one は arcs に arcs.one を含む" do
    tags(:one).arcs.should include( arcs(:one) )
  end

  it "two は arcs に arcs.two を含む" do
    tags(:two).arcs.should include( arcs(:one) )
  end
end

describe Tag, "search" do
  fixtures :tags

  it "name tag2 で検索すると two にマッチする" do
    @search = Tag::Search.new('name' => 'tag2')
    Tag.find(:all, @search.find_options).should == [ tags(:two) ]
  end
end
