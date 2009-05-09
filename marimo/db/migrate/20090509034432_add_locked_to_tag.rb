class AddLockedToTag < ActiveRecord::Migration
  def self.up
    add_column :tags, :locked, :boolean, :null => false, :default => false
  end

  def self.down
    remove_column :tags, :locked
  end
end
