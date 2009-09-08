require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/cards/new.html.erb" do
  include CardsHelper

  before(:each) do
    assigns[:card] = stub_model(Card,
      :new_record? => true,
      :player => 1,
      :character => 1,
      :position => 1
    )
  end

  it "renders new card form" do
    render

    response.should have_tag("form[action=?][method=post]", cards_path) do
      with_tag("input#card_player[name=?]", "card[player]")
      with_tag("input#card_character[name=?]", "card[character]")
      with_tag("input#card_position[name=?]", "card[position]")
    end
  end
end
