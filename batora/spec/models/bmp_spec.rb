require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Bmp do
  before(:each) do
    @valid_attributes = {
      :path => "value for path"
    }
  end

  it "should create a new instance given valid attributes" do
    Bmp.create!(@valid_attributes)
  end
end
