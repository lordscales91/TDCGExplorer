# This file is auto-generated from the current state of the database. Instead of editing this file, 
# please use the migrations feature of Active Record to incrementally modify your database, and
# then regenerate this schema definition.
#
# Note that this schema.rb definition is the authoritative source for your database schema. If you need
# to create the application database on another system, you should be using db:schema:load, not running
# all the migrations from scratch. The latter is a flawed and unsustainable approach (the more migrations
# you'll amass, the slower it'll run and the greater likelihood for issues).
#
# It's strongly recommended to check this file into your version control system.

ActiveRecord::Schema.define(:version => 20090512184837) do

  create_table "arc_equips", :force => true do |t|
    t.integer  "arc_id",     :limit => 11
    t.integer  "equip_id",   :limit => 11
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  add_index "arc_equips", ["arc_id"], :name => "index_arc_equips_on_arc_id"
  add_index "arc_equips", ["equip_id"], :name => "index_arc_equips_on_equip_id"

  create_table "arc_tags", :force => true do |t|
    t.integer  "arc_id",     :limit => 11
    t.integer  "tag_id",     :limit => 11
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  add_index "arc_tags", ["arc_id"], :name => "index_arc_tags_on_arc_id"
  add_index "arc_tags", ["tag_id"], :name => "index_arc_tags_on_tag_id"

  create_table "arcs", :force => true do |t|
    t.string   "code",       :limit => 10
    t.string   "extname",    :limit => 10
    t.string   "location",   :limit => 15
    t.string   "summary"
    t.string   "origname"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  add_index "arcs", ["location", "code"], :name => "index_arcs_on_location_and_code", :unique => true

  create_table "equips", :force => true do |t|
    t.string   "name",       :limit => 15, :null => false
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "relationships", :force => true do |t|
    t.integer  "from_id",    :limit => 11
    t.integer  "to_id",      :limit => 11
    t.integer  "kind",       :limit => 11, :default => 1, :null => false
    t.string   "note",       :limit => 30
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  add_index "relationships", ["from_id"], :name => "index_relationships_on_from_id"
  add_index "relationships", ["to_id"], :name => "index_relationships_on_to_id"

  create_table "tags", :force => true do |t|
    t.string   "name",       :limit => 15,                    :null => false
    t.datetime "created_at"
    t.datetime "updated_at"
    t.boolean  "locked",                   :default => false, :null => false
  end

  create_table "tahs", :force => true do |t|
    t.integer  "arc_id",     :limit => 11
    t.string   "path"
    t.datetime "created_at"
    t.datetime "updated_at"
    t.integer  "position",   :limit => 11
  end

  add_index "tahs", ["arc_id"], :name => "index_tahs_on_arc_id"

  create_table "tsos", :force => true do |t|
    t.integer  "tah_id",     :limit => 11
    t.string   "path"
    t.datetime "created_at"
    t.datetime "updated_at"
    t.string   "tah_hash",   :limit => 8,  :null => false
    t.integer  "position",   :limit => 11
    t.string   "md5",        :limit => 32, :null => false
  end

  add_index "tsos", ["tah_id"], :name => "index_tsos_on_tah_id"
  add_index "tsos", ["tah_hash"], :name => "index_tsos_on_tah_hash"
  add_index "tsos", ["md5"], :name => "index_tsos_on_md5"

  create_table "users", :force => true do |t|
    t.string   "login"
    t.string   "password"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

end
