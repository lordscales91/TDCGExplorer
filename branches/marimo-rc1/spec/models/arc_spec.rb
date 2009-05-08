require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Arc, "tags" do
  fixtures :arcs, :arc_tags, :tags

  it "one �� tags �� tags.one ���܂�" do
    arcs(:one).tags.should include( tags(:one) )
  end

  it "one �� tags �� tags.two ���܂�" do
    arcs(:one).tags.should include( tags(:two) )
  end
end
