require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe TsosController do
  before do
    controller.stub!(:current_user).and_return(mock_user)
  end

  def mock_user(stubs={})
    @_mock_user ||= mock_model(User, stubs)
  end

  def mock_tso(stubs={})
    @_mock_tso ||= mock_model(Tso, stubs)
  end

  describe "GET index" do

  end

  describe "GET show" do

    it "指定 tso を得る" do
      Tso.should_receive(:find).with("42").and_return(mock_tso)
      get :show, :id => "42"
      assigns[:tso].should == mock_tso
    end

  end

  describe "GET edit" do

    it "指定 tso を得る" do
      Tso.should_receive(:find).with("42").and_return(mock_tso)
      get :edit, :id => "42"
      assigns[:tso].should == mock_tso
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tso.stub!(:find).and_return(mock_tso)
      get :edit, :id => "1"
    end

  end

  describe "PUT update" do

    it "指定 tso を更新する" do
      Tso.should_receive(:find).with("42").and_return(mock_tso)
      mock_tso.should_receive(:update_attributes).with({'these' => 'params'}).and_return(true)
      put :update, :id => "42", :tso => {:these => 'params'}
      response.should redirect_to(tso_path(mock_tso))
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tso.stub!(:find).and_return(mock_tso)
      mock_tso.stub!(:update_attributes).and_return(true)
      put :update, :id => "1"
    end

  end

  describe "DELETE destroy" do

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tso.stub!(:find).and_return(mock_tso)
      mock_tso.stub!(:destroy).and_return(true)
      delete :destroy, :id => "1"
    end

  end
end
