class Good < ActiveRecord::Base
  belongs_to :character

  def sale_price
    ( price*sale_rate ).to_i
  end

  def sale_rate
    0.75
  end

  def down_rate
    0.85
  end

  def up_rate
    1.25
  end

  def be_bought_to(player)
    price = self.price
    raise "no stock" if self.stock < 1
    raise "less money" if player.money < price
    ActiveRecord::Base.transaction do
      self.stock -= 1
      self.price *= self.up_rate
      save!
      player.money -= price
      player.save!
      player.cards.create(:character => character)
    end
  end
end
