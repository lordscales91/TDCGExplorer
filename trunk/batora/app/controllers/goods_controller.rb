class GoodsController < ApplicationController
  # GET /goods
  # GET /goods.xml
  def index
    @goods = Good.find(:all, :include => :character)

    respond_to do |format|
      format.html # index.html.erb
      format.xml  { render :xml => @goods }
    end
  end

  # GET /goods/1
  # GET /goods/1.xml
  def show
    @good = Good.find(params[:id])

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @good }
    end
  end

  # GET /goods/new
  # GET /goods/new.xml
  def new
    @good = Good.new

    respond_to do |format|
      format.html # new.html.erb
      format.xml  { render :xml => @good }
    end
  end

  # GET /goods/1/edit
  def edit
    @good = Good.find(params[:id])
  end

  # POST /goods
  # POST /goods.xml
  def create
    @good = Good.new(params[:good])

    respond_to do |format|
      if @good.save
        flash[:notice] = 'Good was successfully created.'
        format.html { redirect_to(@good) }
        format.xml  { render :xml => @good, :status => :created, :location => @good }
      else
        format.html { render :action => "new" }
        format.xml  { render :xml => @good.errors, :status => :unprocessable_entity }
      end
    end
  end

  # PUT /goods/1
  # PUT /goods/1.xml
  def update
    @good = Good.find(params[:id])

    respond_to do |format|
      if @good.update_attributes(params[:good])
        flash[:notice] = 'Good was successfully updated.'
        format.html { redirect_to(@good) }
        format.xml  { head :ok }
      else
        format.html { render :action => "edit" }
        format.xml  { render :xml => @good.errors, :status => :unprocessable_entity }
      end
    end
  end

  # DELETE /goods/1
  # DELETE /goods/1.xml
  def destroy
    @good = Good.find(params[:id])
    @good.destroy

    respond_to do |format|
      format.html { redirect_to(goods_url) }
      format.xml  { head :ok }
    end
  end
end
