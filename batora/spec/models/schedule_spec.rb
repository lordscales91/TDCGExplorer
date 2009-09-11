require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Schedule do
  before(:each) do
    @valid_attributes = {
      :home_player_id => 1,
      :away_player_id => 1
    }
  end

  it "should create a new instance given valid attributes" do
    Schedule.create!(@valid_attributes)
  end
end
