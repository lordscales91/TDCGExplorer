require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/characters/show.html.erb" do
  include CharactersHelper
  before(:each) do
    assigns[:character] = @character = stub_model(Character,
      :off => 1,
      :def => 1,
      :agi => 1,
      :life => 1
    )
  end

  it "renders attributes in <p>" do
    render
    response.should have_text(/1/)
    response.should have_text(/1/)
    response.should have_text(/1/)
    response.should have_text(/1/)
  end
end

