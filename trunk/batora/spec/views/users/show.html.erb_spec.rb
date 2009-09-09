require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/users/show.html.erb" do
  include UsersHelper
  before(:each) do
    assigns[:user] = @user = stub_model(User,
      :login => "value for login",
      :password => "value for password"
    )
  end

  it "renders attributes in <p>" do
    render
    response.should have_text(/value\ for\ login/)
    response.should have_text(/value\ for\ password/)
  end
end
