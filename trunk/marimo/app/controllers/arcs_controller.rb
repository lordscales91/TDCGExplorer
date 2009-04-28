class ArcsController < ApplicationController
  # GET /arcs
  # GET /arcs.xml
  def index
    @search = Arc::Search.new(params[:search])
    @arcs = Arc.paginate(@search.find_options.merge(:page => params[:page], :order => 'location, code'))

    respond_to do |format|
      format.html # index.html.erb
      format.xml  { render :xml => @arcs }
    end
  end

  # GET /arcs/1
  # GET /arcs/1.xml
  def show
    @arc = Arc.find(params[:id])

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @arc }
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
