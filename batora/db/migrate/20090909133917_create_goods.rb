class CreateGoods < ActiveRecord::Migration
  def self.up
    create_table :goods do |t|
      t.references :character
      t.integer :stock
      t.integer :price

      t.timestamps
    end
  end

  def self.down
    drop_table :goods
  end
end
