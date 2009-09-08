require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/players/edit.html.erb" do
  include PlayersHelper

  before(:each) do
    assigns[:player] = @player = stub_model(Player,
      :new_record? => false,
      :nick => "value for nick"
    )
  end

  it "renders the edit player form" do
    render

    response.should have_tag("form[action=#{player_path(@player)}][method=post]") do
      with_tag('input#player_nick[name=?]', "player[nick]")
    end
  end
end
