class CreateTsoColBases < ActiveRecord::Migration
  def self.up
    create_table :tso_col_bases do |t|
      t.references :tso
      t.references :col_base

      t.timestamps
    end
    add_index :tso_col_bases, :tso_id
    add_index :tso_col_bases, :col_base_id
  end

  def self.down
    drop_table :tso_col_bases
  end
end
