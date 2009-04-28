class CreateTahs < ActiveRecord::Migration
  def self.up
    create_table :tahs do |t|
      t.references :arc
      t.string :path

      t.timestamps
    end
  end

  def self.down
    drop_table :tahs
  end
end
