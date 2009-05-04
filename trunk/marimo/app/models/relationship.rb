class Relationship < ActiveRecord::Base
  belongs_to :from, :class_name => 'Arc'
  belongs_to :to, :class_name => 'Arc'

  def kind_caption
    case kind
    when 1
      '重複'
    when 2
      '新版'
    when 3
      '前提'
    end
  end

  def rev_kind_caption
    case kind
    when 1
      '重複'
    when 2
      '旧版'
    when 3
      '提供'
    end
  end

  def to_code
    to ? to.code : nil
  end

  def to_code=(to_code)
    to = Arc.find_by_code(to_code)
    self.to_id = to ? to.id : nil
  end

  def should_destroy?
    to_id.nil?
  end
end
