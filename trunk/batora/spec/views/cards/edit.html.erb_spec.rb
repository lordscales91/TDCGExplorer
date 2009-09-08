require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/cards/edit.html.erb" do
  include CardsHelper

  before(:each) do
    assigns[:card] = @card = stub_model(Card,
      :new_record? => false,
      :player => 1,
      :character => 1,
      :position => 1
    )
  end

  it "renders the edit card form" do
    render

    response.should have_tag("form[action=#{card_path(@card)}][method=post]") do
      with_tag('input#card_player[name=?]', "card[player]")
      with_tag('input#card_character[name=?]', "card[character]")
      with_tag('input#card_position[name=?]', "card[position]")
    end
  end
end
