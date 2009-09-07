require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/characters/index.html.erb" do
  include CharactersHelper
  
  before(:each) do
    assigns[:characters] = [
      stub_model(Character,
        :off => 1,
        :def => 1,
        :agi => 1,
        :life => 1
      ),
      stub_model(Character,
        :off => 1,
        :def => 1,
        :agi => 1,
        :life => 1
      )
    ]
  end

  it "renders a list of characters" do
    render
    response.should have_tag("tr>td", 1.to_s, 2)
    response.should have_tag("tr>td", 1.to_s, 2)
    response.should have_tag("tr>td", 1.to_s, 2)
    response.should have_tag("tr>td", 1.to_s, 2)
  end
end

