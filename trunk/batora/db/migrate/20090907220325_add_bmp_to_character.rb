class AddBmpToCharacter < ActiveRecord::Migration
  def self.up
    add_column :characters, :bmp_id, :integer
  end

  def self.down
    remove_column :characters, :bmp_id
  end
end
