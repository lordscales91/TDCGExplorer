class ArcTag < ActiveRecord::Base
  belongs_to :arc
  belongs_to :tag

  def tag_name
    tag ? tag.name : nil
  end

  def tag_name=(tag_name)
    tag = Tag.find_or_create_by_name(tag_name)
    self.tag_id = tag.id
  end

  def should_destroy?
    tag_id.nil?
  end
end
