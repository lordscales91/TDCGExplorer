require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Tbn do
  before(:each) do
    @valid_attributes = {
      :bmp => ,
      :name => "value for name",
      :position => 1
    }
  end

  it "should create a new instance given valid attributes" do
    Tbn.create!(@valid_attributes)
  end
end
