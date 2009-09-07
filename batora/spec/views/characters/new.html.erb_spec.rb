require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/characters/new.html.erb" do
  include CharactersHelper
  
  before(:each) do
    assigns[:character] = stub_model(Character,
      :new_record? => true,
      :off => 1,
      :def => 1,
      :agi => 1,
      :life => 1
    )
  end

  it "renders new character form" do
    render
    
    response.should have_tag("form[action=?][method=post]", characters_path) do
      with_tag("input#character_off[name=?]", "character[off]")
      with_tag("input#character_def[name=?]", "character[def]")
      with_tag("input#character_agi[name=?]", "character[agi]")
      with_tag("input#character_life[name=?]", "character[life]")
    end
  end
end


