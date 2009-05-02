class Relationship < ActiveRecord::Base
  belongs_to :from, :class_name => 'Arc'
  belongs_to :to, :class_name => 'Arc'

  def kind_caption
    case kind
    when 1
      '�d��'
    when 2
      '�V��'
    when 3
      '�O��'
    end
  end

  def rev_kind_caption
    case kind
    when 1
      '�d��'
    when 2
      '����'
    when 3
      '��'
    end
  end
end
