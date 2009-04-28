class CreateTsos < ActiveRecord::Migration
  def self.up
    create_table :tsos do |t|
      t.references :tah
      t.string :path

      t.timestamps
    end
  end

  def self.down
    drop_table :tsos
  end
end
