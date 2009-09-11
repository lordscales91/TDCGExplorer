class Schedule < ActiveRecord::Base
  belongs_to :home_player, :class_name => "Player"
  belongs_to :away_player, :class_name => "Player"
  has_many :games, :order => "day"

  def home_player_nick
    home_player ? home_player.nick : nil
  end

  def away_player_nick
    away_player ? away_player.nick : nil
  end
end
