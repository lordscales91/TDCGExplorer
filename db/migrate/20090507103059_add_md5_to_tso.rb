class AddMd5ToTso < ActiveRecord::Migration
  def self.up
    add_column :tsos, :md5, :string, :limit => 32, :null => false
  end

  def self.down
    remove_column :tsos, :md5
  end
end
