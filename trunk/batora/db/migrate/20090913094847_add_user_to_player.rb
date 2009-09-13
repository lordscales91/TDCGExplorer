class AddUserToPlayer < ActiveRecord::Migration
  def self.up
    add_column :players, :user_id, :integer
  end

  def self.down
    remove_column :players, :user_id
  end
end
