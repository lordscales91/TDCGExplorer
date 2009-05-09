class EquipsController < ApplicationController
  include AuthenticatedSystem
  layout 'anon'
  before_filter :login_required, :only => [ :new, :edit, :create, :update, :destroy ]

  # GET /equips
  # GET /equips.xml
  def index
    @equips = Equip.find(:all)

    respond_to do |format|
      format.html # index.html.erb
      format.xml  { render :xml => @equips }
    end
  end

  # GET /equips/1
  # GET /equips/1.xml
  def show
    @equip = Equip.find(params[:id])

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @equip }
    end
  end

  # GET /equips/new
  # GET /equips/new.xml
  def new
    @equip = Equip.new

    respond_to do |format|
      format.html # new.html.erb
      format.xml  { render :xml => @equip }
    end
  end

  # GET /equips/1/edit
  def edit
    @equip = Equip.find(params[:id])
  end

  # POST /equips
  # POST /equips.xml
  def create
    @equip = Equip.new(params[:equip])

    respond_to do |format|
      if @equip.save
        flash[:notice] = 'Equip was successfully created.'
        format.html { redirect_to(@equip) }
        format.xml  { render :xml => @equip, :status => :created, :location => @equip }
      else
        format.html { render :action => "new" }
        format.xml  { render :xml => @equip.errors, :status => :unprocessable_entity }
      end
    end
  end

  # PUT /equips/1
  # PUT /equips/1.xml
  def update
    @equip = Equip.find(params[:id])

    respond_to do |format|
      if @equip.update_attributes(params[:equip])
        flash[:notice] = 'Equip was successfully updated.'
        format.html { redirect_to(@equip) }
        format.xml  { head :ok }
      else
        format.html { render :action => "edit" }
        format.xml  { render :xml => @equip.errors, :status => :unprocessable_entity }
      end
    end
  end

  # DELETE /equips/1
  # DELETE /equips/1.xml
  def destroy
    @equip = Equip.find(params[:id])
    @equip.destroy

    respond_to do |format|
      format.html { redirect_to(equips_url) }
      format.xml  { head :ok }
    end
  end
end
