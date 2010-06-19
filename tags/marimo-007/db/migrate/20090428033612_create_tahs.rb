class CreateTahs < ActiveRecord::Migration
  def self.up
    create_table :tahs do |t|
      t.references :arc
      t.string :path

      t.timestamps
    end
    add_index :tahs, :arc_id
  end

  def self.down
    remove_index :tahs, :arc_id
    drop_table :tahs
  end
end
