require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tag, "arcs" do
  fixtures :arcs, :arc_tags, :tags

  it "one �� arcs �� arcs.one ���܂�" do
    tags(:one).arcs.should include( arcs(:one) )
  end

  it "two �� arcs �� arcs.two ���܂�" do
    tags(:two).arcs.should include( arcs(:one) )
  end
end
