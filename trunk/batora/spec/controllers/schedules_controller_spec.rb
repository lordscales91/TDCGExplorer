require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe SchedulesController do

  def mock_schedule(stubs={})
    @mock_schedule ||= mock_model(Schedule, stubs)
  end
  
  describe "GET index" do
    it "assigns all schedules as @schedules" do
      Schedule.stub!(:find).with(:all).and_return([mock_schedule])
      get :index
      assigns[:schedules].should == [mock_schedule]
    end
  end

  describe "GET show" do
    it "assigns the requested schedule as @schedule" do
      Schedule.stub!(:find).with("37").and_return(mock_schedule)
      get :show, :id => "37"
      assigns[:schedule].should equal(mock_schedule)
    end
  end

  describe "GET new" do
    it "assigns a new schedule as @schedule" do
      Schedule.stub!(:new).and_return(mock_schedule)
      get :new
      assigns[:schedule].should equal(mock_schedule)
    end
  end

  describe "GET edit" do
    it "assigns the requested schedule as @schedule" do
      Schedule.stub!(:find).with("37").and_return(mock_schedule)
      get :edit, :id => "37"
      assigns[:schedule].should equal(mock_schedule)
    end
  end

  describe "POST create" do
    
    describe "with valid params" do
      it "assigns a newly created schedule as @schedule" do
        Schedule.stub!(:new).with({'these' => 'params'}).and_return(mock_schedule(:save => true))
        post :create, :schedule => {:these => 'params'}
        assigns[:schedule].should equal(mock_schedule)
      end

      it "redirects to the created schedule" do
        Schedule.stub!(:new).and_return(mock_schedule(:save => true))
        post :create, :schedule => {}
        response.should redirect_to(schedule_url(mock_schedule))
      end
    end
    
    describe "with invalid params" do
      it "assigns a newly created but unsaved schedule as @schedule" do
        Schedule.stub!(:new).with({'these' => 'params'}).and_return(mock_schedule(:save => false))
        post :create, :schedule => {:these => 'params'}
        assigns[:schedule].should equal(mock_schedule)
      end

      it "re-renders the 'new' template" do
        Schedule.stub!(:new).and_return(mock_schedule(:save => false))
        post :create, :schedule => {}
        response.should render_template('new')
      end
    end
    
  end

  describe "PUT update" do
    
    describe "with valid params" do
      it "updates the requested schedule" do
        Schedule.should_receive(:find).with("37").and_return(mock_schedule)
        mock_schedule.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :schedule => {:these => 'params'}
      end

      it "assigns the requested schedule as @schedule" do
        Schedule.stub!(:find).and_return(mock_schedule(:update_attributes => true))
        put :update, :id => "1"
        assigns[:schedule].should equal(mock_schedule)
      end

      it "redirects to the schedule" do
        Schedule.stub!(:find).and_return(mock_schedule(:update_attributes => true))
        put :update, :id => "1"
        response.should redirect_to(schedule_url(mock_schedule))
      end
    end
    
    describe "with invalid params" do
      it "updates the requested schedule" do
        Schedule.should_receive(:find).with("37").and_return(mock_schedule)
        mock_schedule.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :schedule => {:these => 'params'}
      end

      it "assigns the schedule as @schedule" do
        Schedule.stub!(:find).and_return(mock_schedule(:update_attributes => false))
        put :update, :id => "1"
        assigns[:schedule].should equal(mock_schedule)
      end

      it "re-renders the 'edit' template" do
        Schedule.stub!(:find).and_return(mock_schedule(:update_attributes => false))
        put :update, :id => "1"
        response.should render_template('edit')
      end
    end
    
  end

  describe "DELETE destroy" do
    it "destroys the requested schedule" do
      Schedule.should_receive(:find).with("37").and_return(mock_schedule)
      mock_schedule.should_receive(:destroy)
      delete :destroy, :id => "37"
    end
  
    it "redirects to the schedules list" do
      Schedule.stub!(:find).and_return(mock_schedule(:destroy => true))
      delete :destroy, :id => "1"
      response.should redirect_to(schedules_url)
    end
  end

end
