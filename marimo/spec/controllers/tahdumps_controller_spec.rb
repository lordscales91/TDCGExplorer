require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe TahdumpsController do
  before do
    controller.stub!(:current_user).and_return(mock_user)
  end

  def mock_user(stubs={})
    @_mock_user ||= mock_model(User, stubs)
  end

  describe "GET 'show'" do
    it "should be successful" do
      get 'show'
      response.should be_success
    end
  end
end
