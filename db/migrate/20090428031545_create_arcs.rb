class CreateArcs < ActiveRecord::Migration
  def self.up
    create_table :arcs do |t|
      t.string :code, :limit => 10
      t.string :extname, :limit => 10
      t.string :location, :limit => 15
      t.string :summary
      t.string :origname

      t.timestamps
    end
  end

  def self.down
    drop_table :arcs
  end
end
