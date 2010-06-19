require 'spec_helper'

describe RelationshipsController do

  #Delete these examples and add some real ones
  it "should use RelationshipsController" do
    controller.should be_an_instance_of(RelationshipsController)
  end


  describe "GET 'index'" do
    it "should be successful" do
      get 'index'
      response.should be_success
    end
  end

  describe "GET 'show'" do
    it "should be successful" do
      get 'show'
      response.should be_success
    end
  end
end
