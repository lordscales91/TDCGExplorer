class User < ActiveRecord::Base
  has_one :player

  def self.authenticate(login, password)
    find_by_login_and_password(login, password)
  end
end
