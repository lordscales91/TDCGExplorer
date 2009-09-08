class CreateCards < ActiveRecord::Migration
  def self.up
    create_table :cards do |t|
      t.references :player
      t.references :character
      t.integer :position

      t.timestamps
    end
  end

  def self.down
    drop_table :cards
  end
end
