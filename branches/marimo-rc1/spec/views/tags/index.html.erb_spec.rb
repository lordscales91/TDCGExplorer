require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/tags/index.html.erb" do
  include TagsHelper
  
  before(:each) do
    assigns[:tags] = [
      stub_model(Tag,
        :name => "value for name"
      ),
      stub_model(Tag,
        :name => "value for name"
      )
    ]
    template.stub!(:logged_in?).and_return(false)
  end

  it "renders a list of tags" do
    render
    response.should have_tag("tr>td", "value for name".to_s, 2)
  end
end

