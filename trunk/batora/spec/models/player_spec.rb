require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Player, "user" do
  fixtures :players, :users

  it "players(:konoa).user �� users(:konoa) �ł���" do
    players(:konoa).user.should == users(:konoa)
  end
end
