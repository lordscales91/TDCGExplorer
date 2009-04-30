require 'test_helper'

class TsosControllerTest < ActionController::TestCase
  def test_should_get_index
    get :index
    assert_response :success
    assert_not_nil assigns(:search)
  end

  def test_should_get_new
    get :new
    assert_response :success
  end

  def test_should_create_tso
    assert_difference('Tso.count') do
      post :create, :tso => { }
    end

    assert_redirected_to tso_path(assigns(:tso))
  end

  def test_should_show_tso
    get :show, :id => tsos(:one).id
    assert_response :success
  end

  def test_should_get_edit
    get :edit, :id => tsos(:one).id
    assert_response :success
  end

  def test_should_update_tso
    put :update, :id => tsos(:one).id, :tso => { }
    assert_redirected_to tso_path(assigns(:tso))
  end

  def test_should_destroy_tso
    assert_difference('Tso.count', -1) do
      delete :destroy, :id => tsos(:one).id
    end

    assert_redirected_to tsos_path
  end
end
