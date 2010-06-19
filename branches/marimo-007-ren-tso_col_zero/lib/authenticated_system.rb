module AuthenticatedSystem
  protected
    def logged_in?
      !!current_user
    end

    def current_user
      @current_user ||= User.find(session[:user_id]) if session[:user_id]
    end

    def current_user=(user)
      session[:user_id] = user ? user.id : nil
      @current_user = user
    end

    def login_required
      logged_in? || access_denied
    end

    def access_denied
      store_location
      redirect_to new_session_path
    end

    def store_location
      session[:return_to] = request.request_uri
    end

    def redirect_back_or_default(default)
      redirect_to(session[:return_to] || default)
      session[:return_to] = nil
    end

    def logout_killing_session!
      @current_user = nil
      session[:user_id] = nil
    end

    def self.included(base)
      base.send :helper_method, :current_user, :logged_in? if base.respond_to? :helper_method
    end
end
