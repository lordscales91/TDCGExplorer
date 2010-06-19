require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/tags/edit.html.erb" do
  include TagsHelper
  
  before(:each) do
    assigns[:tag] = @tag = stub_model(Tag,
      :new_record? => false,
      :name => "value for name"
    )
  end

  it "renders the edit tag form" do
    render
    
    response.should have_tag("form[action=#{tag_path(@tag)}][method=post]") do
      with_tag('input#tag_name[name=?]', "tag[name]")
    end
  end
end


