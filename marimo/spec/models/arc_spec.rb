require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Arc, "col_bases" do
  fixtures :arcs, :tahs, :tsos, :tso_col_bases

  it "one" do
    arcs(:one).col_bases.should be_empty
  end

  it "two" do
    arcs(:two).col_bases.should == [ arcs(:one) ]
  end

  describe "tsos(:two).tah == tahs(:one) なら" do
    before do
      TAHHash.should_receive(:calc).with('data/model/N400BODY_A00.tso').and_return(0xBC0EEF52) # in update_col_base
      TAHHash.should_receive(:calc).with('data/model/N400BODY_A01.tso').and_return(0xBC0EFF52) # in before_save
      tsos(:two).tah_id = 1
      tsos(:two).save!
    end

    it "one: self は含まない" do
      arcs(:one).col_bases.should == [ ]
    end

    it "two" do
      arcs(:two).col_bases.should == [ ]
    end
  end

  describe "tahs(:two).arc == arcs(:one) なら" do
    before do
      tahs(:two).arc_id = 1
      tahs(:two).save!
    end

    it "one" do
      arcs(:one).col_bases.should == [ arcs(:one) ]
    end

    it "two" do
      arcs(:two).col_bases.should == [ ]
    end
  end
end

describe Arc, "tags" do
  fixtures :arcs, :arc_tags, :tags

  it "one は tags に tags.one を含む" do
    arcs(:one).tags.should include( tags(:one) )
  end

  it "one は tags に tags.two を含む" do
    arcs(:one).tags.should include( tags(:two) )
  end

  it "one は valid である" do
    arcs(:one).should be_valid
  end

  it "重複 arc_tag があるなら one は valid でない" do
    arc_tag = arcs(:one).arc_tags.detect { |at| at.tag_id == 1 }
    arc_tag.tag_id = 2
    arcs(:one).should_not be_valid
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
