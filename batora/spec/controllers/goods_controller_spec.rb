require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe GoodsController do

  def mock_good(stubs={})
    @mock_good ||= mock_model(Good, stubs)
  end
  
  describe "GET index" do
    it "assigns all goods as @goods" do
      Good.stub!(:find).with(:all, :include => :character).and_return([mock_good])
      get :index
      assigns[:goods].should == [mock_good]
    end
  end

  describe "GET show" do
    it "assigns the requested good as @good" do
      Good.stub!(:find).with("37").and_return(mock_good)
      get :show, :id => "37"
      assigns[:good].should equal(mock_good)
    end
  end

  describe "GET new" do
    it "assigns a new good as @good" do
      Good.stub!(:new).and_return(mock_good)
      get :new
      assigns[:good].should equal(mock_good)
    end
  end

  describe "GET edit" do
    it "assigns the requested good as @good" do
      Good.stub!(:find).with("37").and_return(mock_good)
      get :edit, :id => "37"
      assigns[:good].should equal(mock_good)
    end
  end

  describe "POST create" do
    
    describe "with valid params" do
      it "assigns a newly created good as @good" do
        Good.stub!(:new).with({'these' => 'params'}).and_return(mock_good(:save => true))
        post :create, :good => {:these => 'params'}
        assigns[:good].should equal(mock_good)
      end

      it "redirects to the created good" do
        Good.stub!(:new).and_return(mock_good(:save => true))
        post :create, :good => {}
        response.should redirect_to(good_url(mock_good))
      end
    end
    
    describe "with invalid params" do
      it "assigns a newly created but unsaved good as @good" do
        Good.stub!(:new).with({'these' => 'params'}).and_return(mock_good(:save => false))
        post :create, :good => {:these => 'params'}
        assigns[:good].should equal(mock_good)
      end

      it "re-renders the 'new' template" do
        Good.stub!(:new).and_return(mock_good(:save => false))
        post :create, :good => {}
        response.should render_template('new')
      end
    end
    
  end

  describe "PUT update" do
    
    describe "with valid params" do
      it "updates the requested good" do
        Good.should_receive(:find).with("37").and_return(mock_good)
        mock_good.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :good => {:these => 'params'}
      end

      it "assigns the requested good as @good" do
        Good.stub!(:find).and_return(mock_good(:update_attributes => true))
        put :update, :id => "1"
        assigns[:good].should equal(mock_good)
      end

      it "redirects to the good" do
        Good.stub!(:find).and_return(mock_good(:update_attributes => true))
        put :update, :id => "1"
        response.should redirect_to(good_url(mock_good))
      end
    end
    
    describe "with invalid params" do
      it "updates the requested good" do
        Good.should_receive(:find).with("37").and_return(mock_good)
        mock_good.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :good => {:these => 'params'}
      end

      it "assigns the good as @good" do
        Good.stub!(:find).and_return(mock_good(:update_attributes => false))
        put :update, :id => "1"
        assigns[:good].should equal(mock_good)
      end

      it "re-renders the 'edit' template" do
        Good.stub!(:find).and_return(mock_good(:update_attributes => false))
        put :update, :id => "1"
        response.should render_template('edit')
      end
    end
    
  end

  describe "DELETE destroy" do
    it "destroys the requested good" do
      Good.should_receive(:find).with("37").and_return(mock_good)
      mock_good.should_receive(:destroy)
      delete :destroy, :id => "37"
    end
  
    it "redirects to the goods list" do
      Good.stub!(:find).and_return(mock_good(:destroy => true))
      delete :destroy, :id => "1"
      response.should redirect_to(goods_url)
    end
  end

end
