require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe CardsController do
  describe "route generation" do
    it "maps #index" do
      route_for(:controller => "cards", :action => "index").should == "/cards"
    end

    it "maps #new" do
      route_for(:controller => "cards", :action => "new").should == "/cards/new"
    end

    it "maps #show" do
      route_for(:controller => "cards", :action => "show", :id => "1").should == "/cards/1"
    end

    it "maps #edit" do
      route_for(:controller => "cards", :action => "edit", :id => "1").should == "/cards/1/edit"
    end

    it "maps #create" do
      route_for(:controller => "cards", :action => "create").should == {:path => "/cards", :method => :post}
    end

    it "maps #update" do
      route_for(:controller => "cards", :action => "update", :id => "1").should == {:path =>"/cards/1", :method => :put}
    end

    it "maps #destroy" do
      route_for(:controller => "cards", :action => "destroy", :id => "1").should == {:path =>"/cards/1", :method => :delete}
    end
  end

  describe "route recognition" do
    it "generates params for #index" do
      params_from(:get, "/cards").should == {:controller => "cards", :action => "index"}
    end

    it "generates params for #new" do
      params_from(:get, "/cards/new").should == {:controller => "cards", :action => "new"}
    end

    it "generates params for #create" do
      params_from(:post, "/cards").should == {:controller => "cards", :action => "create"}
    end

    it "generates params for #show" do
      params_from(:get, "/cards/1").should == {:controller => "cards", :action => "show", :id => "1"}
    end

    it "generates params for #edit" do
      params_from(:get, "/cards/1/edit").should == {:controller => "cards", :action => "edit", :id => "1"}
    end

    it "generates params for #update" do
      params_from(:put, "/cards/1").should == {:controller => "cards", :action => "update", :id => "1"}
    end

    it "generates params for #destroy" do
      params_from(:delete, "/cards/1").should == {:controller => "cards", :action => "destroy", :id => "1"}
    end
  end
end
