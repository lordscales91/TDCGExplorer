class ArcTag < ActiveRecord::Base
  belongs_to :arc
  belongs_to :tag

  # def validate_on_create
  #   errors.add_to_base("this arc_tag has been taken") if ArcTag.find(:first, :conditions => ["arc_id = ? and tag_id = ?", arc_id, tag_id])
  # end

  def trim(string)
    string.sub(/^[ 　]+/, '').sub(/[ 　]+$/, '')
  end

  def tag_name
    tag ? tag.name : nil
  end

  def tag_name=(tag_name)
    tag_name = trim(tag_name)
    if tag_name.blank?
      self.tag_id = nil
    else
      tag = Tag.find_or_create_by_name(tag_name)
      self.tag_id = tag.id
    end
  end

  def should_destroy?
    tag_id.nil?
  end
end
