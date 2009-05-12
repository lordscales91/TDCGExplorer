class AddIndexLocationAndCodeOnArc < ActiveRecord::Migration
  def self.up
    add_index :arcs, [:location, :code], :unique => true
  end

  def self.down
    remove_index :arcs, :column => [:location, :code]
  end
end
