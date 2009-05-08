require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Arc, "tags" do
  fixtures :arcs, :arc_tags, :tags

  it "one ‚Í tags ‚É tags.one ‚ðŠÜ‚Þ" do
    arcs(:one).tags.should include( tags(:one) )
  end

  it "one ‚Í tags ‚É tags.two ‚ðŠÜ‚Þ" do
    arcs(:one).tags.should include( tags(:two) )
  end
end
