require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe ArcsController do

  def mock_arc(stubs={})
    @_mock_arc ||= mock_model(Arc, stubs)
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

  describe "GET edit" do

    it "指定 arc を得る" do
      Arc.should_receive(:find).with("42").and_return(mock_arc)
      get :edit, :id => "42"
      assigns[:arc].should == mock_arc
    end

  end

  describe "PUT update" do

    it "指定 arc を更新する" do
      Arc.should_receive(:find).with("42").and_return(mock_arc)
      mock_arc.should_receive(:update_attributes).with({'these' => 'params'}).and_return(true)
      put :update, :id => "42", :arc => {:these => 'params'}
      response.should redirect_to(arc_path(mock_arc))
    end

  end
end
