require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe TahdumpsController do

  #Delete these examples and add some real ones
  it "should use TahdumpsController" do
    controller.should be_an_instance_of(TahdumpsController)
  end


  describe "GET 'show'" do
    it "should be successful" do
      get 'show'
      response.should be_success
    end
  end
end
