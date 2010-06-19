class AddPositionToTah < ActiveRecord::Migration
  def self.up
    add_column :tahs, :position, :integer
  end

  def self.down
    remove_column :tahs, :position
  end
end
