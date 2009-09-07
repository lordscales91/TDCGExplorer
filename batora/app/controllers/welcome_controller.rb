class WelcomeController < ApplicationController
  def index
  end

  def upload
    Upload.upload_savefile(params[:file])
    redirect_to :action => 'index'
  end
end
