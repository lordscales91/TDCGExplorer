require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/schedules/index.html.erb" do
  include SchedulesHelper
  
  before(:each) do
    assigns[:schedules] = [
      stub_model(Schedule,
        :home_player_id => 1,
        :away_player_id => 1
      ),
      stub_model(Schedule,
        :home_player_id => 1,
        :away_player_id => 1
      )
    ]
  end

  it "renders a list of schedules" do
    render
    response.should have_tag("tr>td", 1.to_s, 2)
    response.should have_tag("tr>td", 1.to_s, 2)
  end
end

