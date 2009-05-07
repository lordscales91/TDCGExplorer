require 'test_helper'

class EquipsControllerTest < ActionController::TestCase
  def test_should_get_index
    get :index
    assert_response :success
    assert_not_nil assigns(:equips)
  end

  def test_should_get_new
    get :new
    assert_response :success
  end

  def test_should_create_equip
    assert_difference('Equip.count') do
      post :create, :equip => { :name => 'name' }
    end

    assert_redirected_to equip_path(assigns(:equip))
  end

  def test_should_show_equip
    get :show, :id => equips(:one).id
    assert_response :success
  end

  def test_should_get_edit
    get :edit, :id => equips(:one).id
    assert_response :success
  end

  def test_should_update_equip
    put :update, :id => equips(:one).id, :equip => { }
    assert_redirected_to equip_path(assigns(:equip))
  end

  def test_should_destroy_equip
    assert_difference('Equip.count', -1) do
      delete :destroy, :id => equips(:one).id
    end

    assert_redirected_to equips_path
  end
end
