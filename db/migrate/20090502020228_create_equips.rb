class CreateEquips < ActiveRecord::Migration
  def self.up
    create_table :equips do |t|
      t.string :name, :limit => 15, :null => false

      t.timestamps
    end
  end

  def self.down
    drop_table :equips
  end
end
