class AddIndexMd5OnTso < ActiveRecord::Migration
  def self.up
    add_index :tsos, :md5
  end

  def self.down
    remove_index :tsos, :md5
  end
end
