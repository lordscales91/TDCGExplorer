require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe CharactersController do
  describe "route generation" do
    it "maps #index" do
      route_for(:controller => "characters", :action => "index").should == "/characters"
    end
  
    it "maps #new" do
      route_for(:controller => "characters", :action => "new").should == "/characters/new"
    end
  
    it "maps #show" do
      route_for(:controller => "characters", :action => "show", :id => "1").should == "/characters/1"
    end
  
    it "maps #edit" do
      route_for(:controller => "characters", :action => "edit", :id => "1").should == "/characters/1/edit"
    end

  it "maps #create" do
    route_for(:controller => "characters", :action => "create").should == {:path => "/characters", :method => :post}
  end

  it "maps #update" do
    route_for(:controller => "characters", :action => "update", :id => "1").should == {:path =>"/characters/1", :method => :put}
  end
  
    it "maps #destroy" do
      route_for(:controller => "characters", :action => "destroy", :id => "1").should == {:path =>"/characters/1", :method => :delete}
    end
  end

  describe "route recognition" do
    it "generates params for #index" do
      params_from(:get, "/characters").should == {:controller => "characters", :action => "index"}
    end
  
    it "generates params for #new" do
      params_from(:get, "/characters/new").should == {:controller => "characters", :action => "new"}
    end
  
    it "generates params for #create" do
      params_from(:post, "/characters").should == {:controller => "characters", :action => "create"}
    end
  
    it "generates params for #show" do
      params_from(:get, "/characters/1").should == {:controller => "characters", :action => "show", :id => "1"}
    end
  
    it "generates params for #edit" do
      params_from(:get, "/characters/1/edit").should == {:controller => "characters", :action => "edit", :id => "1"}
    end
  
    it "generates params for #update" do
      params_from(:put, "/characters/1").should == {:controller => "characters", :action => "update", :id => "1"}
    end
  
    it "generates params for #destroy" do
      params_from(:delete, "/characters/1").should == {:controller => "characters", :action => "destroy", :id => "1"}
    end
  end
end
