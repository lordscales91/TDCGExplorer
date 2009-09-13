class Card < ActiveRecord::Base
  belongs_to :player
  acts_as_list :scope => :player
  belongs_to :character

  def be_sold
    good = character.good
    sale_price = good.sale_price
    ActiveRecord::Base.transaction do
      destroy
      good.stock += 1
      good.price *= good.down_rate
      good.save!
      player.jewel += sale_price
      player.save!
    end
    true
  end
end
