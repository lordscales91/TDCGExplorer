class CardsController < ApplicationController
  layout 'welcome'
  before_filter :assign_player
  def assign_player
    @player = Player.find(params[:player_id])
  end

  # GET /cards
  # GET /cards.xml
  def index
    @cards = @player.cards.find(:all, :include => :character)

    respond_to do |format|
      format.html # index.html.erb
      format.xml  { render :xml => @cards }
    end
  end

  # GET /cards/1
  # GET /cards/1.xml
  def show
    @card = @player.cards.find(params[:id])

    respond_to do |format|
      format.html # show.html.erb
      format.xml  { render :xml => @card }
    end
  end

  # GET /cards/new
  # GET /cards/new.xml
  def new
    @card = @player.cards.build

    respond_to do |format|
      format.html # new.html.erb
      format.xml  { render :xml => @card }
    end
  end

  # GET /cards/1/edit
  def edit
    @card = @player.cards.find(params[:id])
  end

  # POST /cards
  # POST /cards.xml
  def create
    @card = @player.cards.build(params[:card])

    respond_to do |format|
      if @card.save
        flash[:notice] = 'Card was successfully created.'
        format.html { redirect_to([ @player, @card ]) }
        format.xml  { render :xml => @card, :status => :created, :location => [ @player, @card ] }
      else
        format.html { render :action => "new" }
        format.xml  { render :xml => @card.errors, :status => :unprocessable_entity }
      end
    end
  end

  # PUT /cards/1
  # PUT /cards/1.xml
  def update
    @card = @player.cards.find(params[:id])

    respond_to do |format|
      if @card.update_attributes(params[:card])
        flash[:notice] = 'Card was successfully updated.'
        format.html { redirect_to([ @player, @card ]) }
        format.xml  { head :ok }
      else
        format.html { render :action => "edit" }
        format.xml  { render :xml => @card.errors, :status => :unprocessable_entity }
      end
    end
  end

  # DELETE /cards/1
  # DELETE /cards/1.xml
  def destroy
    @card = @player.cards.find(params[:id])
    @card.destroy

    respond_to do |format|
      format.html { redirect_to(player_cards_url(@player)) }
      format.xml  { head :ok }
    end
  end
end
