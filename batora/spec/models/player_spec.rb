require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Player do
  before(:each) do
    @valid_attributes = {
      :nick => "value for nick"
    }
  end

  it "should create a new instance given valid attributes" do
    Player.create!(@valid_attributes)
  end
end
