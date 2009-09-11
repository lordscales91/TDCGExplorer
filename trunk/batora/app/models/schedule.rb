class Schedule < ActiveRecord::Base
  belongs_to :home_player, :class_name => "Player"
  belongs_to :away_player, :class_name => "Player"
end
