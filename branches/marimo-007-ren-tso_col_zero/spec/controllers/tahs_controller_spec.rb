require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe TahsController do
  before do
    controller.stub!(:current_user).and_return(mock_user)
  end

  def mock_user(stubs={})
    @_mock_user ||= mock_model(User, stubs)
  end

  def mock_tah(stubs={})
    @_mock_tah ||= mock_model(Tah, stubs)
  end

  def mock_tah_tsos(stubs={})
    @_mock_tah_tsos ||= mock("tah tsos", stubs)
  end

  def mock_tso(stubs={})
    @_mock_tso ||= mock_model(Tso, stubs)
  end

  describe "GET index" do

    it "tahs を得る" do
      Tah.should_receive(:paginate).and_return([ mock_tah ])
      get :index
      assigns[:tahs].should == [ mock_tah ]
    end

  end

  describe "GET show" do

    it "指定 tah を得る" do
      Tah.should_receive(:find).with("42").and_return(mock_tah(:tsos => mock_tah_tsos(:paginate => [ mock_tso ])))
      get :show, :id => "42"
      assigns[:tah].should == mock_tah
    end

  end

  describe "GET edit" do

    it "指定 tah を得る" do
      Tah.should_receive(:find).with("42").and_return(mock_tah)
      get :edit, :id => "42"
      assigns[:tah].should == mock_tah
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tah.stub!(:find).and_return(mock_tah)
      get :edit, :id => "1"
    end

  end

  describe "PUT update" do

    it "指定 tah を更新する" do
      Tah.should_receive(:find).with("42").and_return(mock_tah)
      mock_tah.should_receive(:update_attributes).with({'these' => 'params'}).and_return(true)
      put :update, :id => "42", :tah => {:these => 'params'}
      response.should redirect_to(tah_path(mock_tah))
    end

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tah.stub!(:find).and_return(mock_tah)
      mock_tah.stub!(:update_attributes).and_return(true)
      put :update, :id => "1"
    end

  end

  describe "DELETE destroy" do

    it "要 user 認証" do
      controller.should_receive(:current_user).and_return(mock_user)
      Tah.stub!(:find).and_return(mock_tah)
      mock_tah.stub!(:destroy).and_return(true)
      delete :destroy, :id => "1"
    end

  end
end
