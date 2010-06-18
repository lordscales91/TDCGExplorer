require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe ArcsController do
  describe "route generation" do
    it "maps #index" do
      route_for(:controller => "arcs", :action => "index").should == "/arcs"
    end
  
    it "maps #show" do
      route_for(:controller => "arcs", :action => "show", :id => "1").should == "/arcs/1"
    end
  
    it "maps #code" do
      route_for(:controller => "arcs", :action => "code", :code => "TA0001").should == "/arcs/code/TA0001"
    end
  
    it "maps #code .xml" do
      route_for(:controller => "arcs", :action => "code", :code => "TA0001", :format => "xml").should == "/arcs/code/TA0001.xml"
    end
  
    it "maps #code_rels" do
      route_for(:controller => "arcs", :action => "code_rels", :code => "TA0001").should == "/arcs/code/TA0001/rels"
    end
  
    it "maps #code_rels .xml" do
      route_for(:controller => "arcs", :action => "code_rels", :code => "TA0001", :format => "xml").should == "/arcs/code/TA0001/rels.xml"
    end
  
    it "maps #code_revs" do
      route_for(:controller => "arcs", :action => "code_revs", :code => "TA0001").should == "/arcs/code/TA0001/revs"
    end
  
    it "maps #code_revs .xml" do
      route_for(:controller => "arcs", :action => "code_revs", :code => "TA0001", :format => "xml").should == "/arcs/code/TA0001/revs.xml"
    end
  end

  describe "route recognition" do
    it "generates params for #index" do
      params_from(:get, "/arcs").should == {:controller => "arcs", :action => "index"}
    end
  
    it "generates params for #show" do
      params_from(:get, "/arcs/1").should == {:controller => "arcs", :action => "show", :id => "1"}
    end
  
    it "generates params for #code" do
      params_from(:get, "/arcs/code/TA0001").should == {:controller => "arcs", :action => "code", :code => "TA0001"}
    end
  
    it "generates params for #code .xml" do
      params_from(:get, "/arcs/code/TA0001.xml").should == {:controller => "arcs", :action => "code", :code => "TA0001", :format => "xml"}
    end
  
    it "generates params for #code_rels" do
      params_from(:get, "/arcs/code/TA0001/rels").should == {:controller => "arcs", :action => "code_rels", :code => "TA0001"}
    end
  
    it "generates params for #code_rels" do
      params_from(:get, "/arcs/code/TA0001/rels.xml").should == {:controller => "arcs", :action => "code_rels", :code => "TA0001", :format => "xml"}
    end
  
    it "generates params for #code_revs" do
      params_from(:get, "/arcs/code/TA0001/revs").should == {:controller => "arcs", :action => "code_revs", :code => "TA0001"}
    end
  
    it "generates params for #code_revs" do
      params_from(:get, "/arcs/code/TA0001/revs.xml").should == {:controller => "arcs", :action => "code_revs", :code => "TA0001", :format => "xml"}
    end
  end
end
