class ArcTagsController < ApplicationController
  def index
  end

  def recent
    @arc_tags = ArcTag.find(:all, :conditions => ["updated_at > ?", 3.days.ago])
    render :xml => @arc_tags
  end

  def show
  end

end
