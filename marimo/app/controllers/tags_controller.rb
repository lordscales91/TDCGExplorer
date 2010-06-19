class TagsController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'
  before_filter :login_required, :only => [ :new, :edit, :create, :update, :destroy ]
  skip_before_filter :verify_authenticity_token, :only => [ :auto_complete_for_tag_name ]

  def auto_complete_for_tag_name
    find_options = { :conditions => [ "name like ?", '%' + NKF.nkf('-Ws', params[:tag][:name]) + '%' ], :limit => 10 }
    @items = Tag.find(:all, find_options)
    render :inline => "<%= auto_complete_result @items, 'name' %>"
  end

  # GET /tags
  # GET /tags.xml
  def index
    @search = Tag::Search.new(params[:search])
    @tags = Tag.paginate(@search.find_options.merge(:page => params[:page]))

    respond_to do |format|
      format.html # index.html.erb
      format.xml  { render :xml => @tags }
    end
  end

  def recent
    @tags = Tag.find(:all, :conditions => ["updated_at > ?", 3.days.ago])
    render :xml => @tags
  end

  # GET /tags/1
  # GET /tags/1.xml
  def show
    @tag = Tag.find(params[:id])
    @tag_arcs = @tag.arcs.paginate(:page => params[:page])

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @tag }
    end
  end

  # GET /tags/new
  # GET /tags/new.xml
  def new
    @tag = Tag.new

    respond_to do |format|
      format.html # new.html.erb
      format.xml  { render :xml => @tag }
    end
  end

  # GET /tags/1/edit
  def edit
    @tag = Tag.find(params[:id])
  end

  # POST /tags
  # POST /tags.xml
  def create
    @tag = Tag.new(params[:tag])

    respond_to do |format|
      if @tag.save
        flash[:notice] = 'Tag was successfully created.'
        format.html { redirect_to(@tag) }
        format.xml  { render :xml => @tag, :status => :created, :location => @tag }
      else
        format.html { render :action => "new" }
        format.xml  { render :xml => @tag.errors, :status => :unprocessable_entity }
      end
    end
  end

  # PUT /tags/1
  # PUT /tags/1.xml
  def update
    @tag = Tag.find(params[:id])

    respond_to do |format|
      if @tag.update_attributes(params[:tag])
        flash[:notice] = 'Tag was successfully updated.'
        format.html { redirect_to(@tag) }
        format.xml  { head :ok }
      else
        format.html { render :action => "edit" }
        format.xml  { render :xml => @tag.errors, :status => :unprocessable_entity }
      end
    end
  end

  # DELETE /tags/1
  # DELETE /tags/1.xml
  def destroy
    @tag = Tag.find(params[:id])
    @tag.destroy

    respond_to do |format|
      format.html { redirect_to(tags_url) }
      format.xml  { head :ok }
    end
  end
end
