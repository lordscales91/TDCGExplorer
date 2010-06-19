class CreateRelationships < ActiveRecord::Migration
  def self.up
    create_table :relationships do |t|
      t.references :from, :to
      t.integer :kind, :null => false, :default => 1
      t.string :note, :limit => 30

      t.timestamps
    end
    add_index :relationships, :from_id
    add_index :relationships, :to_id
  end

  def self.down
    remove_index :relationships, :to_id
    remove_index :relationships, :from_id
    drop_table :relationships
  end
end
