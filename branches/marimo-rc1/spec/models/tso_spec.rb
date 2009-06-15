require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tso, "row" do
  fixtures :tsos

  it "one" do
    tsos(:one).row.should == 'A'
  end

  it "”wŒi‚È‚ç nil ‚ð“¾‚é" do
    tso = Tso.new(:path => 'data/bgmodel/HOTEL_C.tso')
    tso.row.should be_nil
  end
end

describe Tso, "col_basis_path" do
  fixtures :tsos, :tso_col_bases

  it "one" do
    tsos(:one).col_basis_path.should == 'data/model/N400BODY_A00.tso'
  end

  it "two" do
    tsos(:two).col_basis_path.should == 'data/model/N400BODY_A00.tso'
  end

  it "”wŒi‚È‚ç nil ‚ð“¾‚é" do
    tso = Tso.new(:path => 'data/bgmodel/HOTEL_C.tso')
    tso.col_basis_path.should be_nil
  end
end

describe Tso, "find_col_bases" do
  fixtures :tsos, :tso_col_bases

  it "one" do
    tsos(:one).find_col_bases.should be_empty
  end

  it "two" do
    tsos(:two).find_col_bases.should == [ tsos(:one) ]
  end

  it "”wŒi‚È‚ç nil ‚ð“¾‚é" do
    tso = Tso.new(:path => 'data/bgmodel/HOTEL_C.tso')
    tso.find_col_bases.should be_empty
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

  it "”wŒi‚È‚ç nil ‚ð“¾‚é" do
    tso = Tso.new(:path => 'data/bgmodel/HOTEL_C.tso')
    tso.col_bases.should be_empty
  end
end

describe Tso, "search" do
  fixtures :tsos

  it "path data/model/N400BODY_A00.tso ‚ÅŒŸõ‚·‚é‚Æ one ‚Éƒ}ƒbƒ`‚·‚é" do
    @search = Tso::Search.new('path' => 'data/model/N400BODY_A00.tso')
    Tso.find(:all, @search.find_options).should == [ tsos(:one) ]
  end

  it "path data/model/N400BODY_A00 ‚ÅŒŸõ‚·‚é‚Æ one ‚Éƒ}ƒbƒ`‚·‚é" do
    @search = Tso::Search.new('path' => 'data/model/N400BODY_A00')
    Tso.find(:all, @search.find_options).should == [ tsos(:one) ]
  end

  it "path data/model/N400BODY ‚ÅŒŸõ‚·‚é‚Æ one two ‚Éƒ}ƒbƒ`‚·‚é" do
    @search = Tso::Search.new('path' => 'data/model/N400BODY_A')
    Tso.find(:all, @search.find_options).should == [ tsos(:one), tsos(:two) ]
  end
end
