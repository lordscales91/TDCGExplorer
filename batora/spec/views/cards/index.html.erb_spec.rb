require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/cards/index.html.erb" do
  include CardsHelper

  before(:each) do
    assigns[:cards] = [
      stub_model(Card,
        :player => 1,
        :character => 1,
        :position => 1
      ),
      stub_model(Card,
        :player => 1,
        :character => 1,
        :position => 1
      )
    ]
  end

  it "renders a list of cards" do
    render
    response.should have_tag("tr>td", 1.to_s, 2)
    response.should have_tag("tr>td", 1.to_s, 2)
    response.should have_tag("tr>td", 1.to_s, 2)
  end
end
