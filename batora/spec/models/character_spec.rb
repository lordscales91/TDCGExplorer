require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Character do
  before(:each) do
    @valid_attributes = {
      :off => 1,
      :def => 1,
      :agi => 1,
      :life => 1
    }
  end

  it "should create a new instance given valid attributes" do
    Character.create!(@valid_attributes)
  end
end
