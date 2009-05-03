class TahsController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'
  before_filter :login_required, :only => [ :new, :edit, :create, :update ]

  # GET /tahs
  # GET /tahs.xml
  def index
    @search = Tah::Search.new(params[:search])
    @tahs = Tah.paginate(@search.find_options.merge(:page => params[:page]))

    respond_to do |format|
      format.html # index.html.erb
      format.xml  { render :xml => @tahs }
    end
  end

  # GET /tahs/1
  # GET /tahs/1.xml
  def show
    @tah = Tah.find(params[:id])

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @tah }
    end
  end

  # GET /tahs/new
  # GET /tahs/new.xml
  def new
    @tah = Tah.new

    respond_to do |format|
      format.html # new.html.erb
      format.xml  { render :xml => @tah }
    end
  end

  # GET /tahs/1/edit
  def edit
    @tah = Tah.find(params[:id])
  end

  # POST /tahs
  # POST /tahs.xml
  def create
    @tah = Tah.new(params[:tah])

    respond_to do |format|
      if @tah.save
        flash[:notice] = 'Tah was successfully created.'
        format.html { redirect_to(@tah) }
        format.xml  { render :xml => @tah, :status => :created, :location => @tah }
      else
        format.html { render :action => "new" }
        format.xml  { render :xml => @tah.errors, :status => :unprocessable_entity }
      end
    end
  end

  # PUT /tahs/1
  # PUT /tahs/1.xml
  def update
    @tah = Tah.find(params[:id])

    respond_to do |format|
      if @tah.update_attributes(params[:tah])
        flash[:notice] = 'Tah was successfully updated.'
        format.html { redirect_to(@tah) }
        format.xml  { head :ok }
      else
        format.html { render :action => "edit" }
        format.xml  { render :xml => @tah.errors, :status => :unprocessable_entity }
      end
    end
  end

  # DELETE /tahs/1
  # DELETE /tahs/1.xml
  def destroy
    @tah = Tah.find(params[:id])
    @tah.destroy

    respond_to do |format|
      format.html { redirect_to(tahs_url) }
      format.xml  { head :ok }
    end
  end
end
