require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/cards/edit.html.erb" do
  include CardsHelper

  before(:each) do
    assigns[:player] = @player = stub_model(Player, :nick => "nomeu")
    assigns[:card] = @card = stub_model(Card,
      :new_record? => false,
      :player => 1,
      :character => 1,
      :position => 1
    )
  end

  it "renders the edit card form" do
    render

    response.should have_tag("form[action=#{player_card_path(@player, @card)}][method=post]") do
    end
  end
end
