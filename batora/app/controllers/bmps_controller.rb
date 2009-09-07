class BmpsController < ApplicationController
  layout 'welcome'

  def index
  end

  def show
    @bmp = Bmp.find(params[:id])
  end

end
