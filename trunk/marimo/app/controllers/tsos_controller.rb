class TsosController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'
  before_filter :login_required, :only => [ :new, :edit, :create, :update, :destroy ]

  # GET /tsos
  # GET /tsos.xml
  def index
    @search = Tso::Search.new(params[:search])
    @tsos = Tso.paginate(@search.find_options.merge(:page => params[:page]))

    respond_to do |format|
      format.html # index.html.erb
      format.xml  { render :xml => @tsos }
    end
  end

  # GET /tsos/1
  # GET /tsos/1.xml
  def show
    @tso = Tso.find(params[:id])

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @tso }
    end
  end

  # GET /tsos/new
  # GET /tsos/new.xml
  def new
    @tso = Tso.new

    respond_to do |format|
      format.html # new.html.erb
      format.xml  { render :xml => @tso }
    end
  end

  # GET /tsos/1/edit
  def edit
    @tso = Tso.find(params[:id])
  end

  # POST /tsos
  # POST /tsos.xml
  def create
    @tso = Tso.new(params[:tso])

    respond_to do |format|
      if @tso.save
        flash[:notice] = 'Tso was successfully created.'
        format.html { redirect_to(@tso) }
        format.xml  { render :xml => @tso, :status => :created, :location => @tso }
      else
        format.html { render :action => "new" }
        format.xml  { render :xml => @tso.errors, :status => :unprocessable_entity }
      end
    end
  end

  # PUT /tsos/1
  # PUT /tsos/1.xml
  def update
    @tso = Tso.find(params[:id])

    respond_to do |format|
      if @tso.update_attributes(params[:tso])
        flash[:notice] = 'Tso was successfully updated.'
        format.html { redirect_to(@tso) }
        format.xml  { head :ok }
      else
        format.html { render :action => "edit" }
        format.xml  { render :xml => @tso.errors, :status => :unprocessable_entity }
      end
    end
  end

  # DELETE /tsos/1
  # DELETE /tsos/1.xml
  def destroy
    @tso = Tso.find(params[:id])
    @tso.destroy

    respond_to do |format|
      format.html { redirect_to(tsos_url) }
      format.xml  { head :ok }
    end
  end
end
