class CreateSchedules < ActiveRecord::Migration
  def self.up
    create_table :schedules do |t|
      t.integer :home_player_id
      t.integer :away_player_id

      t.timestamps
    end
  end

  def self.down
    drop_table :schedules
  end
end
