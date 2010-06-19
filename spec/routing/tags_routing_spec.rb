require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe TagsController do
  describe "route generation" do
    it "maps #index" do
      route_for(:controller => "tags", :action => "index").should == "/tags"
    end
  
    it "maps #new" do
      route_for(:controller => "tags", :action => "new").should == "/tags/new"
    end
  
    it "maps #show" do
      route_for(:controller => "tags", :action => "show", :id => "1").should == "/tags/1"
    end
  
    it "maps #edit" do
      route_for(:controller => "tags", :action => "edit", :id => "1").should == "/tags/1/edit"
    end

  it "maps #create" do
    route_for(:controller => "tags", :action => "create").should == {:path => "/tags", :method => :post}
  end

  it "maps #update" do
    route_for(:controller => "tags", :action => "update", :id => "1").should == {:path =>"/tags/1", :method => :put}
  end
  
    it "maps #destroy" do
      route_for(:controller => "tags", :action => "destroy", :id => "1").should == {:path =>"/tags/1", :method => :delete}
    end
  end

  describe "route recognition" do
    it "generates params for #index" do
      params_from(:get, "/tags").should == {:controller => "tags", :action => "index"}
    end
  
    it "generates params for #new" do
      params_from(:get, "/tags/new").should == {:controller => "tags", :action => "new"}
    end
  
    it "generates params for #create" do
      params_from(:post, "/tags").should == {:controller => "tags", :action => "create"}
    end
  
    it "generates params for #show" do
      params_from(:get, "/tags/1").should == {:controller => "tags", :action => "show", :id => "1"}
    end
  
    it "generates params for #edit" do
      params_from(:get, "/tags/1/edit").should == {:controller => "tags", :action => "edit", :id => "1"}
    end
  
    it "generates params for #update" do
      params_from(:put, "/tags/1").should == {:controller => "tags", :action => "update", :id => "1"}
    end
  
    it "generates params for #destroy" do
      params_from(:delete, "/tags/1").should == {:controller => "tags", :action => "destroy", :id => "1"}
    end
  end
end
