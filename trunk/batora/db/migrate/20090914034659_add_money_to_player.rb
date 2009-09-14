class AddMoneyToPlayer < ActiveRecord::Migration
  def self.up
    add_column :players, :money, :integer, :null => false
  end

  def self.down
    remove_column :players, :money
  end
end
