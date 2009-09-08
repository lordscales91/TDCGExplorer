require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe BmpsController do

  def mock_bmp(stubs={})
    @mock_bmp ||= mock_model(Bmp, stubs)
  end

  describe "GET 'index'" do
    it "should be successful" do
      Bmp.stub!(:find).with(:all).and_return([mock_bmp])
      get 'index'
      response.should be_success
    end
  end

  describe "GET 'show'" do
    it "should be successful" do
      Bmp.stub!(:find).with("37").and_return(mock_bmp)
      get 'show', :id => "37"
      response.should be_success
    end
  end
end
