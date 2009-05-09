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

describe Arc, "search" do
  fixtures :arcs

  it "text TA0013 で検索すると two にマッチする" do
    @search = Arc::Search.new('text' => 'TA0013')
    Arc.find(:all, @search.find_options).should == [ arcs(:two) ]
  end

  it "text tattoo で検索すると two にマッチする" do
    @search = Arc::Search.new('text' => 'tattoo')
    Arc.find(:all, @search.find_options).should == [ arcs(:two) ]
  end
end
