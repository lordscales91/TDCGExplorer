require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe PlayersController do
  describe "route generation" do
    it "maps #index" do
      route_for(:controller => "players", :action => "index").should == "/players"
    end

    it "maps #new" do
      route_for(:controller => "players", :action => "new").should == "/players/new"
    end

    it "maps #show" do
      route_for(:controller => "players", :action => "show", :id => "1").should == "/players/1"
    end

    it "maps #edit" do
      route_for(:controller => "players", :action => "edit", :id => "1").should == "/players/1/edit"
    end

    it "maps #create" do
      route_for(:controller => "players", :action => "create").should == {:path => "/players", :method => :post}
    end

    it "maps #update" do
      route_for(:controller => "players", :action => "update", :id => "1").should == {:path =>"/players/1", :method => :put}
    end

    it "maps #destroy" do
      route_for(:controller => "players", :action => "destroy", :id => "1").should == {:path =>"/players/1", :method => :delete}
    end
  end

  describe "route recognition" do
    it "generates params for #index" do
      params_from(:get, "/players").should == {:controller => "players", :action => "index"}
    end

    it "generates params for #new" do
      params_from(:get, "/players/new").should == {:controller => "players", :action => "new"}
    end

    it "generates params for #create" do
      params_from(:post, "/players").should == {:controller => "players", :action => "create"}
    end

    it "generates params for #show" do
      params_from(:get, "/players/1").should == {:controller => "players", :action => "show", :id => "1"}
    end

    it "generates params for #edit" do
      params_from(:get, "/players/1/edit").should == {:controller => "players", :action => "edit", :id => "1"}
    end

    it "generates params for #update" do
      params_from(:put, "/players/1").should == {:controller => "players", :action => "update", :id => "1"}
    end

    it "generates params for #destroy" do
      params_from(:delete, "/players/1").should == {:controller => "players", :action => "destroy", :id => "1"}
    end
  end
end
