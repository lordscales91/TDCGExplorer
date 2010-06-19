class RenameToTsoColZero < ActiveRecord::Migration
  def self.up
    remove_index :tso_col_bases, :column => :col_basis_id
    remove_index :tso_col_bases, :column => :tso_id
    rename_column :tso_col_bases, :col_basis_id, :col_zero_id
    rename_table :tso_col_bases, :tso_col_zeros
    add_index :tso_col_zeros, :col_zero_id
    add_index :tso_col_zeros, :tso_id
  end

  def self.down
    remove_index :tso_col_zeros, :column => :tso_id
    remove_index :tso_col_zeros, :column => :col_zero_id
    rename_table :tso_col_zeros, :tso_col_bases
    rename_column :tso_col_bases, :col_zero_id, :col_basis_id
    add_index :tso_col_bases, :tso_id
    add_index :tso_col_bases, :col_basis_id
  end
end
