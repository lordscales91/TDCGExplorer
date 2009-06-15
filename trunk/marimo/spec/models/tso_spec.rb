require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tso, "row" do
  fixtures :tsos

  it "one" do
    tsos(:one).row.should == 'A'
  end

  it "背景なら nil を得る" do
    tso = Tso.new(:path => 'data/bgmodel/HOTEL_C.tso')
    tso.row.should be_nil
  end
end

describe Tso, "col_base_path" do
  fixtures :tsos, :tso_col_bases

  it "one" do
    tsos(:one).col_base_path.should == 'data/model/N400BODY_A00.tso'
  end

  it "two" do
    tsos(:two).col_base_path.should == 'data/model/N400BODY_A00.tso'
  end

  it "背景なら nil を得る" do
    tso = Tso.new(:path => 'data/bgmodel/HOTEL_C.tso')
    tso.col_base_path.should be_nil
  end
end

describe Tso, "col_bases" do
  fixtures :tsos, :tso_col_bases

  it "one" do
    tsos(:one).col_bases.should be_empty
  end

  it "two" do
    tsos(:two).col_bases.should == [ tsos(:one) ]
  end

  it "背景なら nil を得る" do
    tso = Tso.new(:path => 'data/bgmodel/HOTEL_C.tso')
    tso.col_bases.should be_empty
  end
end

describe Tso, "search" do
  fixtures :tsos

  it "path data/model/N400BODY_A00.tso で検索すると one にマッチする" do
    @search = Tso::Search.new('path' => 'data/model/N400BODY_A00.tso')
    Tso.find(:all, @search.find_options).should == [ tsos(:one) ]
  end

  it "path data/model/N400BODY_A00 で検索すると one にマッチする" do
    @search = Tso::Search.new('path' => 'data/model/N400BODY_A00')
    Tso.find(:all, @search.find_options).should == [ tsos(:one) ]
  end

  it "path data/model/N400BODY で検索すると one two にマッチする" do
    @search = Tso::Search.new('path' => 'data/model/N400BODY_A')
    Tso.find(:all, @search.find_options).should == [ tsos(:one), tsos(:two) ]
  end
end
