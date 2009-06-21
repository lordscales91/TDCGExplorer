class User < ActiveRecord::Base

  def self.authenticate(login, password)
    find_by_login_and_password(login, password)
  end
end
