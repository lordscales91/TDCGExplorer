class TahdumpsController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'
  before_filter :login_required

  def show
  end

  def create
    tahdump = Tahdump.new(params[:tahdump])
    tahdump.commit
    flash[:notice] = 'TAHDump was successfully committed.'
    redirect_to tahdump_path
  rescue RuntimeError
    flash[:error] = $!.message
    render :action => "show"
  end
end
