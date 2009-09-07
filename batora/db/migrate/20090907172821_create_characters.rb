class CreateCharacters < ActiveRecord::Migration
  def self.up
    create_table :characters do |t|
      t.integer :off, :null => false
      t.integer :def, :null => false
      t.integer :agi, :null => false
      t.integer :life, :null => false

      t.timestamps
    end
  end

  def self.down
    drop_table :characters
  end
end
