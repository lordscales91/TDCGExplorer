class WelcomeController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'

  def index
    @search = Welcome::Search.new(params[:search])
    if @search.text.blank?
      @arcs = []
      @tahs = []
    else
      @arc_search = Arc::Search.new(params[:search])
      @arcs = Arc.paginate(@arc_search.find_options.merge(:page => nil, :order => 'location, code'))
      @tah_search = Tah::Search.new(params[:search])
      @tahs = Tah.paginate(@tah_search.find_options.merge(:page => nil))
    end
  end

end
