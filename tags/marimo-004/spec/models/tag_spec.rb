require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tag, "arcs" do
  fixtures :arcs, :arc_tags, :tags

  it "one ‚Í arcs ‚É arcs.one ‚ðŠÜ‚Þ" do
    tags(:one).arcs.should include( arcs(:one) )
  end

  it "two ‚Í arcs ‚É arcs.two ‚ðŠÜ‚Þ" do
    tags(:two).arcs.should include( arcs(:one) )
  end
end
