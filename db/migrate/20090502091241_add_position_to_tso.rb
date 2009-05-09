class AddPositionToTso < ActiveRecord::Migration
  def self.up
    add_column :tsos, :position, :integer
  end

  def self.down
    remove_column :tsos, :position
  end
end
