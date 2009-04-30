require 'test_helper'

class TahsControllerTest < ActionController::TestCase
  def test_should_get_index
    get :index
    assert_response :success
    assert_not_nil assigns(:tahs)
  end

  def test_should_get_new
    get :new
    assert_response :success
  end

  def test_should_create_tah
    assert_difference('Tah.count') do
      post :create, :tah => { }
    end

    assert_redirected_to tah_path(assigns(:tah))
  end

  def test_should_show_tah
    get :show, :id => tahs(:one).id
    assert_response :success
  end

  def test_should_get_edit
    get :edit, :id => tahs(:one).id
    assert_response :success
  end

  def test_should_update_tah
    put :update, :id => tahs(:one).id, :tah => { }
    assert_redirected_to tah_path(assigns(:tah))
  end

  def test_should_destroy_tah
    assert_difference('Tah.count', -1) do
      delete :destroy, :id => tahs(:one).id
    end

    assert_redirected_to tahs_path
  end
end
