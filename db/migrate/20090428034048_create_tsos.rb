class CreateTsos < ActiveRecord::Migration
  def self.up
    create_table :tsos do |t|
      t.references :tah
      t.string :path

      t.timestamps
    end
    add_index :tsos, :tah_id
  end

  def self.down
    remove_index :tsos, :tah_id
    drop_table :tsos
  end
end
