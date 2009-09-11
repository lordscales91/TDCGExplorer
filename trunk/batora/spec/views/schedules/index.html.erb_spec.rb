require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/schedules/index.html.erb" do
  include SchedulesHelper
  
  before(:each) do
    assigns[:schedules] = [
      stub_model(Schedule,
        :home_player_nick => "konoa",
        :away_player_nick => "nomeu"
      )
    ]
  end

  it "renders a list of schedules" do
    render
    response.should have_tag("tr>td", "konoa", 1)
    response.should have_tag("tr>td", "nomeu", 1)
  end
end

