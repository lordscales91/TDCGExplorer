class TahdumpsController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'

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
