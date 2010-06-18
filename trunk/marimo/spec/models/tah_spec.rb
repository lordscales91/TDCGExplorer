require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tah, "col_bases" do
  fixtures :tahs, :tsos, :tso_col_bases

  it "one" do
    tahs(:one).col_bases.should be_empty
  end

  it "two" do
    tahs(:two).col_bases.should == [ tahs(:one) ]
  end

  describe "tsos(:two).tah == tahs(:one) なら" do
    before do
      TAHHash.should_receive(:calc).with('data/model/N400BODY_A00.tso').and_return(0xBC0EEF52) # in update_col_base
      TAHHash.should_receive(:calc).with('data/model/N400BODY_A01.tso').and_return(0xBC0EFF52) # in before_save
      tsos(:two).tah_id = 1
      tsos(:two).save!
    end

    it "one: self は含まない" do
      tahs(:one).col_bases.should == [ ]
    end

    it "two" do
      tahs(:two).col_bases.should == [ ]
    end
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
