require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Relationship do
  before(:each) do
    @valid_attributes = {
      :kind => 1,
      :note => "value for note"
    }
  end

  it "should create a new instance given valid attributes" do
    Relationship.create!(@valid_attributes)
  end
end
