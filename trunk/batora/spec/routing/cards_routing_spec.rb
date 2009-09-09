require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe CardsController do
  describe "route generation" do
    it "maps #index" do
      route_for(:controller => "cards", :action => "index", :player_id => "1").should == "/players/1/cards"
    end

    it "maps #new" do
      route_for(:controller => "cards", :action => "new", :player_id => "1").should == "/players/1/cards/new"
    end

    it "maps #show" do
      route_for(:controller => "cards", :action => "show", :player_id => "1", :id => "1").should == "/players/1/cards/1"
    end

    it "maps #edit" do
      route_for(:controller => "cards", :action => "edit", :player_id => "1", :id => "1").should == "/players/1/cards/1/edit"
    end

    it "maps #create" do
      route_for(:controller => "cards", :action => "create", :player_id => "1").should == {:path => "/players/1/cards", :method => :post}
    end

    it "maps #update" do
      route_for(:controller => "cards", :action => "update", :player_id => "1", :id => "1").should == {:path =>"/players/1/cards/1", :method => :put}
    end

    it "maps #destroy" do
      route_for(:controller => "cards", :action => "destroy", :player_id => "1", :id => "1").should == {:path =>"/players/1/cards/1", :method => :delete}
    end
  end

  describe "route recognition" do
    it "generates params for #index" do
      params_from(:get, "/players/1/cards").should == {:controller => "cards", :action => "index", :player_id => "1"}
    end

    it "generates params for #new" do
      params_from(:get, "/players/1/cards/new").should == {:controller => "cards", :action => "new", :player_id => "1"}
    end

    it "generates params for #create" do
      params_from(:post, "/players/1/cards").should == {:controller => "cards", :action => "create", :player_id => "1"}
    end

    it "generates params for #show" do
      params_from(:get, "/players/1/cards/1").should == {:controller => "cards", :action => "show", :player_id => "1", :id => "1"}
    end

    it "generates params for #edit" do
      params_from(:get, "/players/1/cards/1/edit").should == {:controller => "cards", :action => "edit", :player_id => "1", :id => "1"}
    end

    it "generates params for #update" do
      params_from(:put, "/players/1/cards/1").should == {:controller => "cards", :action => "update", :player_id => "1", :id => "1"}
    end

    it "generates params for #destroy" do
      params_from(:delete, "/players/1/cards/1").should == {:controller => "cards", :action => "destroy", :player_id => "1", :id => "1"}
    end
  end
end
