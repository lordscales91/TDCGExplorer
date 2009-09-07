require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe CharactersController do

  def mock_character(stubs={})
    @mock_character ||= mock_model(Character, stubs)
  end
  
  describe "GET index" do
    it "assigns all characters as @characters" do
      Character.stub!(:find).with(:all).and_return([mock_character])
      get :index
      assigns[:characters].should == [mock_character]
    end
  end

  describe "GET show" do
    it "assigns the requested character as @character" do
      Character.stub!(:find).with("37").and_return(mock_character)
      get :show, :id => "37"
      assigns[:character].should equal(mock_character)
    end
  end

  describe "GET new" do
    it "assigns a new character as @character" do
      Character.stub!(:new).and_return(mock_character)
      get :new
      assigns[:character].should equal(mock_character)
    end
  end

  describe "GET edit" do
    it "assigns the requested character as @character" do
      Character.stub!(:find).with("37").and_return(mock_character)
      get :edit, :id => "37"
      assigns[:character].should equal(mock_character)
    end
  end

  describe "POST create" do
    
    describe "with valid params" do
      it "assigns a newly created character as @character" do
        Character.stub!(:new).with({'these' => 'params'}).and_return(mock_character(:save => true))
        post :create, :character => {:these => 'params'}
        assigns[:character].should equal(mock_character)
      end

      it "redirects to the created character" do
        Character.stub!(:new).and_return(mock_character(:save => true))
        post :create, :character => {}
        response.should redirect_to(character_url(mock_character))
      end
    end
    
    describe "with invalid params" do
      it "assigns a newly created but unsaved character as @character" do
        Character.stub!(:new).with({'these' => 'params'}).and_return(mock_character(:save => false))
        post :create, :character => {:these => 'params'}
        assigns[:character].should equal(mock_character)
      end

      it "re-renders the 'new' template" do
        Character.stub!(:new).and_return(mock_character(:save => false))
        post :create, :character => {}
        response.should render_template('new')
      end
    end
    
  end

  describe "PUT update" do
    
    describe "with valid params" do
      it "updates the requested character" do
        Character.should_receive(:find).with("37").and_return(mock_character)
        mock_character.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :character => {:these => 'params'}
      end

      it "assigns the requested character as @character" do
        Character.stub!(:find).and_return(mock_character(:update_attributes => true))
        put :update, :id => "1"
        assigns[:character].should equal(mock_character)
      end

      it "redirects to the character" do
        Character.stub!(:find).and_return(mock_character(:update_attributes => true))
        put :update, :id => "1"
        response.should redirect_to(character_url(mock_character))
      end
    end
    
    describe "with invalid params" do
      it "updates the requested character" do
        Character.should_receive(:find).with("37").and_return(mock_character)
        mock_character.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :character => {:these => 'params'}
      end

      it "assigns the character as @character" do
        Character.stub!(:find).and_return(mock_character(:update_attributes => false))
        put :update, :id => "1"
        assigns[:character].should equal(mock_character)
      end

      it "re-renders the 'edit' template" do
        Character.stub!(:find).and_return(mock_character(:update_attributes => false))
        put :update, :id => "1"
        response.should render_template('edit')
      end
    end
    
  end

  describe "DELETE destroy" do
    it "destroys the requested character" do
      Character.should_receive(:find).with("37").and_return(mock_character)
      mock_character.should_receive(:destroy)
      delete :destroy, :id => "37"
    end
  
    it "redirects to the characters list" do
      Character.stub!(:find).and_return(mock_character(:destroy => true))
      delete :destroy, :id => "1"
      response.should redirect_to(characters_url)
    end
  end

end
