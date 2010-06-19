require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/tags/show.html.erb" do
  include TagsHelper
  before(:each) do
    assigns[:tag] = @tag = stub_model(Tag,
      :name => "value for name"
    )
    @arc = stub_model(Arc)
    assigns[:tag_arcs] = [ @arc ]
    assigns[:tag_arcs].stub!(:total_pages).and_return(0)
    template.stub!(:logged_in?).and_return(false)
  end

  it "renders attributes in <p>" do
    render
    response.should have_text(/value\ for\ name/)
  end
end
