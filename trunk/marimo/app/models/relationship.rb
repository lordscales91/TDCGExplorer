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
end
