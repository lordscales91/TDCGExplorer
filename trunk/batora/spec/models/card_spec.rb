require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Card, "player" do
  fixtures :cards, :players

  it "cards(:one).player �� players(:konoa) �ł���" do
    cards(:one).player.should == players(:konoa)
  end
end
