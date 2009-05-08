class CreateArcEquips < ActiveRecord::Migration
  def self.up
    create_table :arc_equips do |t|
      t.references :arc
      t.references :equip

      t.timestamps
    end
    add_index :arc_equips, :arc_id
    add_index :arc_equips, :equip_id
  end

  def self.down
    remove_index :arc_equips, :equip_id
    remove_index :arc_equips, :arc_id
    drop_table :arc_equips
  end
end
