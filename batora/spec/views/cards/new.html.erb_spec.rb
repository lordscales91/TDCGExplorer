require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/cards/new.html.erb" do
  include CardsHelper

  before(:each) do
    assigns[:player] = @player = stub_model(Player, :nick => "nomeu")
    assigns[:card] = stub_model(Card,
      :new_record? => true,
      :player => 1,
      :character => 1,
      :position => 1
    )
  end

  it "renders new card form" do
    render

    response.should have_tag("form[action=?][method=post]", player_cards_path(@player)) do
    end
  end
end
