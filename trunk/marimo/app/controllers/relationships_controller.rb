class RelationshipsController < ApplicationController
  def index
  end

  def recent
    @relationships = Relationship.find(:all, :conditions => ["updated_at > ?", 3.days.ago])
    render :xml => @relationships
  end

  def show
  end

end
