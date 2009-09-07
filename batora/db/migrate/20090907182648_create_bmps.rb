class CreateBmps < ActiveRecord::Migration
  def self.up
    create_table :bmps do |t|
      t.string :path

      t.timestamps
    end
  end

  def self.down
    drop_table :bmps
  end
end
