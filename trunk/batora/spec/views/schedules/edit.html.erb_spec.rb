require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/schedules/edit.html.erb" do
  include SchedulesHelper
  
  before(:each) do
    assigns[:schedule] = @schedule = stub_model(Schedule,
      :new_record? => false,
      :home_player_id => 1,
      :away_player_id => 1
    )
  end

  it "renders the edit schedule form" do
    render
    
    response.should have_tag("form[action=#{schedule_path(@schedule)}][method=post]") do
      with_tag('input#schedule_home_player_id[name=?]', "schedule[home_player_id]")
      with_tag('input#schedule_away_player_id[name=?]', "schedule[away_player_id]")
    end
  end
end


