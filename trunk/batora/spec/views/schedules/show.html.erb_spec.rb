require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/schedules/show.html.erb" do
  include SchedulesHelper
  before(:each) do
    assigns[:schedule] = @schedule = stub_model(Schedule,
      :home_player_nick => "konoa",
      :away_player_nick => "nomeu"
    )
  end

  it "renders attributes in <p>" do
    render
    response.should have_text(/konoa/)
    response.should have_text(/nomeu/)
  end
end

