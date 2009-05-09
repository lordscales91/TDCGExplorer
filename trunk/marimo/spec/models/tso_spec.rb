require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tso, "search" do
  fixtures :tsos

  it "path N001BODY_A00.tso �Ō�������� one �Ƀ}�b�`����" do
    @search = Tso::Search.new('path' => 'N001BODY_A00.tso')
    Tso.find(:all, @search.find_options).should == [ tsos(:one) ]
  end

  it "path N001BODY_A00 �Ō�������� one �Ƀ}�b�`����" do
    @search = Tso::Search.new('path' => 'N001BODY_A00')
    Tso.find(:all, @search.find_options).should == [ tsos(:one) ]
  end

  it "path N001BODY �Ō�������� one two �Ƀ}�b�`����" do
    @search = Tso::Search.new('path' => 'N001BODY')
    Tso.find(:all, @search.find_options).should == [ tsos(:one), tsos(:two) ]
  end
end
