require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/cards/show.html.erb" do
  include CardsHelper
  before(:each) do
    assigns[:player] = stub_model(Player, :nick => "nomeu")
    assigns[:card] = @card = stub_model(Card,
      :player => 1,
      :character => 1,
      :position => 1
    )
  end

  it "renders attributes in <p>" do
    render
    response.should have_text(/1/)
    response.should have_text(/1/)
    response.should have_text(/1/)
  end
end
