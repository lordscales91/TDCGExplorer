require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/players/show.html.erb" do
  include PlayersHelper
  before(:each) do
    assigns[:player] = @player = stub_model(Player,
      :nick => "value for nick"
    )
  end

  it "renders attributes in <p>" do
    render
    response.should have_text(/value\ for\ nick/)
  end
end
