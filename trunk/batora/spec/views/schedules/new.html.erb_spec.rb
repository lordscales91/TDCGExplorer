require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/schedules/new.html.erb" do
  include SchedulesHelper
  
  before(:each) do
    assigns[:schedule] = stub_model(Schedule,
      :new_record? => true,
      :home_player_id => 1,
      :away_player_id => 1
    )
  end

  it "renders new schedule form" do
    render
    
    response.should have_tag("form[action=?][method=post]", schedules_path) do
      with_tag("input#schedule_home_player_id[name=?]", "schedule[home_player_id]")
      with_tag("input#schedule_away_player_id[name=?]", "schedule[away_player_id]")
    end
  end
end


