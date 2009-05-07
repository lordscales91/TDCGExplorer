require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tag, "arcs" do
  fixtures :arcs, :arc_tags, :tags

  it "one は arcs に arcs.one を含む" do
    tags(:one).arcs.should include( arcs(:one) )
  end

  it "two は arcs に arcs.two を含む" do
    tags(:two).arcs.should include( arcs(:one) )
  end
end
