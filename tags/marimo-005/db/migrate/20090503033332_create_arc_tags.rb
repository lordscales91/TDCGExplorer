class CreateArcTags < ActiveRecord::Migration
  def self.up
    create_table :arc_tags do |t|
      t.references :arc
      t.references :tag

      t.timestamps
    end
    add_index :arc_tags, :arc_id
    add_index :arc_tags, :tag_id
  end

  def self.down
    remove_index :arc_tags, :tag_id
    remove_index :arc_tags, :arc_id
    drop_table :arc_tags
  end
end
