require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe GoodsController do
  describe "route generation" do
    it "maps #index" do
      route_for(:controller => "goods", :action => "index").should == "/goods"
    end
  
    it "maps #new" do
      route_for(:controller => "goods", :action => "new").should == "/goods/new"
    end
  
    it "maps #show" do
      route_for(:controller => "goods", :action => "show", :id => "1").should == "/goods/1"
    end
  
    it "maps #edit" do
      route_for(:controller => "goods", :action => "edit", :id => "1").should == "/goods/1/edit"
    end

  it "maps #create" do
    route_for(:controller => "goods", :action => "create").should == {:path => "/goods", :method => :post}
  end

  it "maps #update" do
    route_for(:controller => "goods", :action => "update", :id => "1").should == {:path =>"/goods/1", :method => :put}
  end
  
    it "maps #destroy" do
      route_for(:controller => "goods", :action => "destroy", :id => "1").should == {:path =>"/goods/1", :method => :delete}
    end
  end

  describe "route recognition" do
    it "generates params for #index" do
      params_from(:get, "/goods").should == {:controller => "goods", :action => "index"}
    end
  
    it "generates params for #new" do
      params_from(:get, "/goods/new").should == {:controller => "goods", :action => "new"}
    end
  
    it "generates params for #create" do
      params_from(:post, "/goods").should == {:controller => "goods", :action => "create"}
    end
  
    it "generates params for #show" do
      params_from(:get, "/goods/1").should == {:controller => "goods", :action => "show", :id => "1"}
    end
  
    it "generates params for #edit" do
      params_from(:get, "/goods/1/edit").should == {:controller => "goods", :action => "edit", :id => "1"}
    end
  
    it "generates params for #update" do
      params_from(:put, "/goods/1").should == {:controller => "goods", :action => "update", :id => "1"}
    end
  
    it "generates params for #destroy" do
      params_from(:delete, "/goods/1").should == {:controller => "goods", :action => "destroy", :id => "1"}
    end
  end
end
