class BmpsController < ApplicationController
  layout 'welcome'

  def index
    @bmps = Bmp.find(:all)
  end

  def show
    @bmp = Bmp.find(params[:id])
  end

end
