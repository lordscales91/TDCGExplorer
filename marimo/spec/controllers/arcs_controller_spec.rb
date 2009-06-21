require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe ArcsController do
  before do
    controller.stub!(:current_user).and_return(mock_user)
  end

  def mock_user(stubs={})
    @_mock_user ||= mock_model(User, stubs)
  end

  def mock_arc(stubs={})
    @_mock_arc ||= mock_model(Arc, stubs)
  end

  def mock_rel(stubs={})
    @_mock_rel ||= mock_model(Relationship, stubs)
  end

  def mock_rev(stubs={})
    @_mock_rev ||= mock_model(Relationship, stubs)
  end

  describe "GET index" do

    it "arcs を得る" do
      Arc.should_receive(:paginate).and_return([ mock_arc ])
      get :index
      assigns[:arcs].should == [ mock_arc ]
    end

  end

  describe "GET show" do

    it "指定 arc を得る" do
      Arc.should_receive(:find).with("42").and_return(mock_arc)
      get :show, :id => "42"
      assigns[:arc].should == mock_arc
    end

  end

  describe "GET code" do

    it "指定 arc を得る" do
      Arc.should_receive(:find_by_code).with("TA0042").and_return(mock_arc)
      get :code, :code => "TA0042"
      assigns[:arc].should == mock_arc
    end

  end

  describe "GET code_rels" do

    it "指定 arc rels を得る" do
      Arc.should_receive(:find_by_code).with("TA0042").and_return(mock_arc)
      mock_arc.should_receive(:relationships).and_return([ mock_rel ])
      get :code_rels, :code => "TA0042"
      assigns[:arc].should == mock_arc
      assigns[:arc_rels].should == [ mock_rel ]
    end

  end

  describe "GET code_revs" do

    it "指定 arc revs を得る" do
      Arc.should_receive(:find_by_code).with("TA0042").and_return(mock_arc)
      mock_arc.should_receive(:rev_relationships).and_return([ mock_rev ])
      get :code_revs, :code => "TA0042"
      assigns[:arc].should == mock_arc
      assigns[:arc_revs].should == [ mock_rev ]
    end

  end

  describe "GET edit" do

    it "指定 arc を得る" do
      Arc.should_receive(:find).with("42").and_return(mock_arc)
      get :edit, :id => "42"
      assigns[:arc].should == mock_arc
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Arc.stub!(:find).and_return(mock_arc)
      get :edit, :id => "1"
    end

  end

  describe "PUT update" do

    it "指定 arc を更新する" do
      Arc.should_receive(:find).with("42").and_return(mock_arc)
      mock_arc.should_receive(:update_attributes).with({'these' => 'params'}).and_return(true)
      put :update, :id => "42", :arc => {:these => 'params'}
      response.should redirect_to(arc_path(mock_arc))
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Arc.stub!(:find).and_return(mock_arc)
      mock_arc.stub!(:update_attributes).and_return(true)
      put :update, :id => "1"
    end

  end

  describe "DELETE destroy" do

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Arc.stub!(:find).and_return(mock_arc)
      mock_arc.stub!(:destroy).and_return(true)
      delete :destroy, :id => "1"
    end

  end
end
