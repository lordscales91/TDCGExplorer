class TahdumpsController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'

  def show
  end

  def create
    tahdump = TAHDump.new(params[:tahdump])
    if tahdump.commit
      flash[:notice] = 'TAHDump was successfully committed.'
      redirect_to tahdump_path
    else
      render :action => "show"
    end
  end
end
