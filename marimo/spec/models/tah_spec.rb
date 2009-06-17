require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tah, "col_bases" do
  fixtures :tahs, :tsos, :tso_col_bases

  it "one" do
    tahs(:one).col_bases.should be_empty
  end

  it "two" do
    tahs(:two).col_bases.should == [ tahs(:one) ]
  end
end

describe Tah, "search" do
  fixtures :tahs

  it "path tattoo/tattoo.tah で検索すると two にマッチする" do
    @search = Tah::Search.new('path' => 'tattoo/tattoo.tah')
    Tah.find(:all, @search.find_options).should == [ tahs(:two) ]
  end

  it "path tattoo で検索すると two にマッチする" do
    @search = Tah::Search.new('path' => 'tattoo')
    Tah.find(:all, @search.find_options).should == [ tahs(:two) ]
  end
end
