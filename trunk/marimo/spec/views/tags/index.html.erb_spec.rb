require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/tags/index.html.erb" do
  include TagsHelper
  
  before(:each) do
    assigns[:tags] = [
      stub_model(Tag,
        :name => "value for name", :arc_tags => [ 1,2,3 ]
      ),
      stub_model(Tag,
        :name => "value for name", :arc_tags => [ 1,2,3 ]
      )
    ]
    template.stub!(:logged_in?).and_return(false)
    assigns[:tags].stub!(:total_entries).and_return(1)
    assigns[:tags].stub!(:total_pages).and_return(0)
  end

  it "renders a list of tags" do
    render
    response.should have_tag("tr>td", "value for name ( 3 )".to_s, 2)
  end
end

