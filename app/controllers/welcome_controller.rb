class WelcomeController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'

  def index
  end

end
