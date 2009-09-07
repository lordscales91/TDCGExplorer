class CreateTbns < ActiveRecord::Migration
  def self.up
    create_table :tbns do |t|
      t.references :bmp
      t.string :name
      t.integer :position

      t.timestamps
    end
  end

  def self.down
    drop_table :tbns
  end
end
