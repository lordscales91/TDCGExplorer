class WelcomeController < ApplicationController
  def index
  end

  def upload
    @bmp = Upload.upload_savefile(params[:file])
    redirect_to bmp_path(@bmp)
  end
end
