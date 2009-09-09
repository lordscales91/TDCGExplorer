require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/cards/index.html.erb" do
  include CardsHelper

  before(:each) do
    assigns[:player] = @player = stub_model(Player, :nick => "nomeu")
    @character = stub_model(Character, :bmp_id => 1)
    assigns[:cards] = [
      stub_model(Card,
        :player => @player,
        :character => @character,
        :position => 1
      ),
    ]
  end

  it "renders a list of cards" do
    render
  end
end
