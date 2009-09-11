require File.expand_path(File.dirname(__FILE__) + '/../../spec_helper')

describe "/schedules/show.html.erb" do
  include SchedulesHelper
  before(:each) do
    assigns[:schedule] = @schedule = stub_model(Schedule,
      :home_player_id => 1,
      :away_player_id => 1
    )
  end

  it "renders attributes in <p>" do
    render
    response.should have_text(/1/)
    response.should have_text(/1/)
  end
end

