require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/players/new.html.erb" do
  include PlayersHelper

  before(:each) do
    assigns[:player] = stub_model(Player,
      :new_record? => true,
      :nick => "value for nick"
    )
  end

  it "renders new player form" do
    render

    response.should have_tag("form[action=?][method=post]", players_path) do
      with_tag("input#player_nick[name=?]", "player[nick]")
    end
  end
end
