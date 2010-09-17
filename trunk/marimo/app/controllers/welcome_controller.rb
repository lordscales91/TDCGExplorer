class WelcomeController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'

  def index
    @search = Welcome::Search.new(params[:search])
    if @search.text.blank?
      @tags = []
      @arcs = []
      @tahs = []
    else
      @tag_search = Tag::Search.new(params[:search])
      @tags = Tag.paginate(@tag_search.find_options.merge(:page => nil, :include => :arc_tags, :per_page => 15))
      @arc_search = Arc::Search.new(params[:search])
      @arcs = Arc.paginate(@arc_search.find_options.merge(:page => nil, :order => 'location, code', :per_page => 15))
      @tah_search = Tah::Search.new(params[:search])
      @tahs = Tah.paginate(@tah_search.find_options.merge(:page => nil, :per_page => 15))
    end
  end

end
