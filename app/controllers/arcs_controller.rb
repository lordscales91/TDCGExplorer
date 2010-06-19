class ArcsController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'
  before_filter :login_required, :only => [ :new, :edit, :create, :update, :destroy ]
  skip_before_filter :verify_authenticity_token, :only => [ :auto_complete_for_tag_name ]

  def auto_complete_for_tag_name
    find_options = { :conditions => [ "name like ?", '%' + NKF.nkf('-Ws', params[:arc][:arc_tag_attributes][0][:tag_name]) + '%' ], :limit => 10 }
    @items = Tag.find(:all, find_options)
    render :inline => "<%= auto_complete_result @items, 'name' %>"
  end

  # GET /arcs
  # GET /arcs.xml
  def index
    @search = Arc::Search.new(params[:search])
    @arcs = Arc.paginate(@search.find_options.merge(:page => params[:page], :order => 'location, code'))

    respond_to do |format|
      format.html # index.html.erb
      format.xml  { render :xml => @arcs.to_xml(:except => [ :created_at, :updated_at ]) }
    end
  end

  def recent
    @arcs = Arc.find(:all, :conditions => ["updated_at > ?", 3.days.ago])
    render :xml => @arcs
  end

  # GET /arcs/1
  # GET /arcs/1.xml
  def show
    @arc = Arc.find(params[:id])

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @arc.to_xml(:except => [ :created_at, :updated_at ], :include => :tahs) }
    end
  end

  # GET /arcs/code/TA0001
  # GET /arcs/code/TA0001.xml
  def code
    @arc = Arc.find_by_code(params[:code])

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @arc.to_xml(:except => [ :created_at, :updated_at ], :include => :tahs) }
    end
  end

  # GET /arcs/code/TA0001/rels
  # GET /arcs/code/TA0001/rels.xml
  def code_rels
    @arc = Arc.find_by_code(params[:code])
    @arc_rels = @arc.relationships

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @arc_rels.to_xml(:root => 'relationships', :except => [ :created_at, :updated_at ]) }
    end
  end

  # GET /arcs/code/TA0001/revs
  # GET /arcs/code/TA0001/revs.xml
  def code_revs
    @arc = Arc.find_by_code(params[:code])
    @arc_revs = @arc.rev_relationships

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @arc_revs.to_xml(:root => 'relationships', :except => [ :created_at, :updated_at ]) }
    end
  end

  # GET /arcs/new
  # GET /arcs/new.xml
  def new
    @arc = Arc.new

    respond_to do |format|
      format.html # new.html.erb
      format.xml  { render :xml => @arc }
    end
  end

  # GET /arcs/1/edit
  def edit
    @arc = Arc.find(params[:id])
  end

  def tags
    @arc = Arc.find(params[:id])
    if request.put?
      if @arc.update_attributes(params[:arc])
        redirect_to(@arc)
      end
    end
  end

  # POST /arcs
  # POST /arcs.xml
  def create
    @arc = Arc.new(params[:arc])

    respond_to do |format|
      if @arc.save
        flash[:notice] = 'Arc was successfully created.'
        format.html { redirect_to(@arc) }
        format.xml  { render :xml => @arc, :status => :created, :location => @arc }
      else
        format.html { render :action => "new" }
        format.xml  { render :xml => @arc.errors, :status => :unprocessable_entity }
      end
    end
  end

  # PUT /arcs/1
  # PUT /arcs/1.xml
  def update
    @arc = Arc.find(params[:id])

    respond_to do |format|
      if @arc.update_attributes(params[:arc])
        flash[:notice] = 'Arc was successfully updated.'
        format.html { redirect_to(@arc) }
        format.xml  { head :ok }
      else
        format.html { render :action => "edit" }
        format.xml  { render :xml => @arc.errors, :status => :unprocessable_entity }
      end
    end
  end

  # DELETE /arcs/1
  # DELETE /arcs/1.xml
  def destroy
    @arc = Arc.find(params[:id])
    @arc.destroy

    respond_to do |format|
      format.html { redirect_to(arcs_url) }
      format.xml  { head :ok }
    end
  end
end
