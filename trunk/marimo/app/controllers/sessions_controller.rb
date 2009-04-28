class SessionsController < ApplicationController
  include AuthenticatedSystem
  layout 'sessions'

  def new
  end

  def create
    user = User.authenticate("admin", params[:password])
    if user
      self.current_user = user
      flash[:notice] = "Logged in successfully."
      redirect_back_or_default root_path
    else
      flash[:error] = "Couldn't log you in"
      logger.warn "Failed login for '#{params[:login]}' from #{request.remote_ip} at #{Time.now.utc}"
      render :action => 'new'
    end
  end

  def destroy
    logout_killing_session!
    flash[:notice] = "You have been logged out."
    redirect_to :action => 'new'
  end
end
