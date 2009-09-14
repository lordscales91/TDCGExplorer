class User < ActiveRecord::Base
  has_one :player

  def self.authenticate(login, password)
    find_by_login_and_password(login, password)
  end

  def after_create
    player = build_player(:nick => login, :money => 500)
    player.save!
  end
end
