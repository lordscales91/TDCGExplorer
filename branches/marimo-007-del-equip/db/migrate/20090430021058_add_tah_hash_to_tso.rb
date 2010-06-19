class AddTahHashToTso < ActiveRecord::Migration
  def self.up
    add_column :tsos, :tah_hash, :string, :limit => 8, :null => false
    add_index :tsos, :tah_hash
  end

  def self.down
    remove_index :tsos, :tah_hash
    remove_column :tsos, :tah_hash
  end
end
