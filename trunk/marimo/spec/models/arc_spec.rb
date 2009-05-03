require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Arc, "tags" do
  fixtures :arcs, :arc_tags, :tags

  it "one は tags に tags.one を含む" do
    arcs(:one).tags.should include( tags(:one) )
  end

  it "one は tags に tags.two を含む" do
    arcs(:one).tags.should include( tags(:two) )
  end
end
