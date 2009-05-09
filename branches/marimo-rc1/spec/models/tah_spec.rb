require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tah, "search" do
  fixtures :tahs

  it "path tattoo/tattoo.tah �Ō�������� two �Ƀ}�b�`����" do
    @search = Tah::Search.new('path' => 'tattoo/tattoo.tah')
    Tah.find(:all, @search.find_options).should == [ tahs(:two) ]
  end

  it "path tattoo �Ō�������� two �Ƀ}�b�`����" do
    @search = Tah::Search.new('path' => 'tattoo')
    Tah.find(:all, @search.find_options).should == [ tahs(:two) ]
  end
end
