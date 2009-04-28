require 'test_helper'

class ArcsControllerTest < ActionController::TestCase
  def test_should_get_index
    get :index
    assert_response :success
    assert_not_nil assigns(:arcs)
  end

  def test_should_get_new
    get :new
    assert_response :success
  end

  def test_should_create_arc
    assert_difference('Arc.count') do
      post :create, :arc => { }
    end

    assert_redirected_to arc_path(assigns(:arc))
  end

  def test_should_show_arc
    get :show, :id => arcs(:one).id
    assert_response :success
  end

  def test_should_get_edit
    get :edit, :id => arcs(:one).id
    assert_response :success
  end

  def test_should_update_arc
    put :update, :id => arcs(:one).id, :arc => { }
    assert_redirected_to arc_path(assigns(:arc))
  end

  def test_should_destroy_arc
    assert_difference('Arc.count', -1) do
      delete :destroy, :id => arcs(:one).id
    end

    assert_redirected_to arcs_path
  end
end
