require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe PlayersController do

  def mock_player(stubs={})
    @mock_player ||= mock_model(Player, stubs)
  end

  def mock_user(stubs={})
    @mock_user ||= mock_model(User, stubs)
  end

  before do
    controller.stub!(:current_user).and_return(mock_user)
  end

  describe "GET index" do
    it "assigns all players as @players" do
      Player.stub!(:find).with(:all).and_return([mock_player])
      get :index
      assigns[:players].should == [mock_player]
    end
  end

  describe "GET show" do
    before do
      mock_player(:user => mock_user)
    end

    it "assigns the requested player as @player" do
      Player.stub!(:find).with("37").and_return(mock_player)
      get :show, :id => "37"
      assigns[:player].should equal(mock_player)
    end
  end

  describe "GET new" do
    it "assigns a new player as @player" do
      Player.stub!(:new).and_return(mock_player)
      get :new
      assigns[:player].should equal(mock_player)
    end
  end

  describe "GET edit" do
    before do
      mock_player(:user => mock_user)
    end

    it "assigns the requested player as @player" do
      Player.stub!(:find).with("37").and_return(mock_player)
      get :edit, :id => "37"
      assigns[:player].should equal(mock_player)
    end
  end

  describe "POST create" do

    describe "with valid params" do
      it "assigns a newly created player as @player" do
        Player.stub!(:new).with({'these' => 'params'}).and_return(mock_player(:save => true))
        post :create, :player => {:these => 'params'}
        assigns[:player].should equal(mock_player)
      end

      it "redirects to the created player" do
        Player.stub!(:new).and_return(mock_player(:save => true))
        post :create, :player => {}
        response.should redirect_to(player_url(mock_player))
      end
    end

    describe "with invalid params" do
      it "assigns a newly created but unsaved player as @player" do
        Player.stub!(:new).with({'these' => 'params'}).and_return(mock_player(:save => false))
        post :create, :player => {:these => 'params'}
        assigns[:player].should equal(mock_player)
      end

      it "re-renders the 'new' template" do
        Player.stub!(:new).and_return(mock_player(:save => false))
        post :create, :player => {}
        response.should render_template('new')
      end
    end

  end

  describe "PUT update" do

    describe "with valid params" do
      before do
        mock_player(:user => mock_user, :update_attributes => true)
      end

      it "updates the requested player" do
        Player.should_receive(:find).with("37").and_return(mock_player)
        mock_player.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :player => {:these => 'params'}
      end

      it "assigns the requested player as @player" do
        Player.stub!(:find).and_return(mock_player)
        put :update, :id => "1"
        assigns[:player].should equal(mock_player)
      end

      it "redirects to the player" do
        Player.stub!(:find).and_return(mock_player)
        put :update, :id => "1"
        response.should redirect_to(player_url(mock_player))
      end
    end

    describe "with invalid params" do
      before do
        mock_player(:user => mock_user, :update_attributes => false)
      end

      it "updates the requested player" do
        Player.should_receive(:find).with("37").and_return(mock_player)
        mock_player.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :player => {:these => 'params'}
      end

      it "assigns the player as @player" do
        Player.stub!(:find).and_return(mock_player)
        put :update, :id => "1"
        assigns[:player].should equal(mock_player)
      end

      it "re-renders the 'edit' template" do
        Player.stub!(:find).and_return(mock_player)
        put :update, :id => "1"
        response.should render_template('edit')
      end
    end

  end

  describe "DELETE destroy" do
    before do
      mock_player(:user => mock_user, :destroy => true)
    end

    it "destroys the requested player" do
      Player.should_receive(:find).with("37").and_return(mock_player)
      mock_player.should_receive(:destroy)
      delete :destroy, :id => "37"
    end

    it "redirects to the players list" do
      Player.stub!(:find).and_return(mock_player)
      delete :destroy, :id => "1"
      response.should redirect_to(players_url)
    end
  end

end
