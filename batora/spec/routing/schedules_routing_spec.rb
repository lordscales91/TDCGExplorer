require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe SchedulesController do
  describe "route generation" do
    it "maps #index" do
      route_for(:controller => "schedules", :action => "index").should == "/schedules"
    end
  
    it "maps #new" do
      route_for(:controller => "schedules", :action => "new").should == "/schedules/new"
    end
  
    it "maps #show" do
      route_for(:controller => "schedules", :action => "show", :id => "1").should == "/schedules/1"
    end
  
    it "maps #edit" do
      route_for(:controller => "schedules", :action => "edit", :id => "1").should == "/schedules/1/edit"
    end

  it "maps #create" do
    route_for(:controller => "schedules", :action => "create").should == {:path => "/schedules", :method => :post}
  end

  it "maps #update" do
    route_for(:controller => "schedules", :action => "update", :id => "1").should == {:path =>"/schedules/1", :method => :put}
  end
  
    it "maps #destroy" do
      route_for(:controller => "schedules", :action => "destroy", :id => "1").should == {:path =>"/schedules/1", :method => :delete}
    end
  end

  describe "route recognition" do
    it "generates params for #index" do
      params_from(:get, "/schedules").should == {:controller => "schedules", :action => "index"}
    end
  
    it "generates params for #new" do
      params_from(:get, "/schedules/new").should == {:controller => "schedules", :action => "new"}
    end
  
    it "generates params for #create" do
      params_from(:post, "/schedules").should == {:controller => "schedules", :action => "create"}
    end
  
    it "generates params for #show" do
      params_from(:get, "/schedules/1").should == {:controller => "schedules", :action => "show", :id => "1"}
    end
  
    it "generates params for #edit" do
      params_from(:get, "/schedules/1/edit").should == {:controller => "schedules", :action => "edit", :id => "1"}
    end
  
    it "generates params for #update" do
      params_from(:put, "/schedules/1").should == {:controller => "schedules", :action => "update", :id => "1"}
    end
  
    it "generates params for #destroy" do
      params_from(:delete, "/schedules/1").should == {:controller => "schedules", :action => "destroy", :id => "1"}
    end
  end
end
