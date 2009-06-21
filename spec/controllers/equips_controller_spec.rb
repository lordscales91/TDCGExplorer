require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe EquipsController do
  before do
    controller.stub!(:current_user).and_return(mock_user)
  end

  def mock_user(stubs={})
    @_mock_user ||= mock_model(User, stubs)
  end

  def mock_equip(stubs={})
    @_mock_equip ||= mock_model(Equip, stubs)
  end

  describe "GET index" do

    it "equips を得る" do
      Equip.should_receive(:find).with(:all).and_return([ mock_equip ])
      get :index
      assigns[:equips].should == [ mock_equip ]
    end

  end

  describe "GET show" do

    it "指定 equip を得る" do
      Equip.should_receive(:find).with("42").and_return(mock_equip)
      get :show, :id => "42"
      assigns[:equip].should == mock_equip
    end

  end

  describe "GET edit" do

    it "指定 equip を得る" do
      Equip.should_receive(:find).with("42").and_return(mock_equip)
      get :edit, :id => "42"
      assigns[:equip].should == mock_equip
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Equip.stub!(:find).and_return(mock_equip)
      get :edit, :id => "1"
    end

  end

  describe "PUT update" do

    it "指定 equip を更新する" do
      Equip.should_receive(:find).with("42").and_return(mock_equip)
      mock_equip.should_receive(:update_attributes).with({'these' => 'params'}).and_return(true)
      put :update, :id => "42", :equip => {:these => 'params'}
      response.should redirect_to(equip_path(mock_equip))
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Equip.stub!(:find).and_return(mock_equip)
      mock_equip.stub!(:update_attributes).and_return(true)
      put :update, :id => "1"
    end

  end

  describe "DELETE destroy" do

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Equip.stub!(:find).and_return(mock_equip)
      mock_equip.stub!(:destroy).and_return(true)
      delete :destroy, :id => "1"
    end

  end
end
