require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/players/index.html.erb" do
  include PlayersHelper

  before(:each) do
    assigns[:players] = [
      stub_model(Player,
        :nick => "value for nick"
      ),
      stub_model(Player,
        :nick => "value for nick"
      )
    ]
  end

  it "renders a list of players" do
    render
    response.should have_tag("tr>td", "value for nick".to_s, 2)
  end
end
