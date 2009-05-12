class Relationship < ActiveRecord::Base
  belongs_to :from, :class_name => 'Arc'
  belongs_to :to, :class_name => 'Arc'

  def validate_on_create
    errors.add_to_base("self reference") if from_id == to_id
    errors.add_to_base("this relationship has been taken") if Relationship.find(:first, :conditions => ["from_id = ? and to_id = ?", from.id, to.id])
    errors.add_to_base("this relationship has been taken") if Relationship.find(:first, :conditions => ["from_id = ? and to_id = ?", to.id, from.id])
  end

  def self.kind_collection
    [['������e', 1], ['�V��', 2], ['�O��', 3]]
  end

  def kind_caption
    case kind
    when 1
      '������e'
    when 2
      '�V��'
    when 3
      '�O��'
    end
  end

  def self.rev_kind_collection
    [['������e', 1], ['����', 2], ['��', 3]]
  end

  def rev_kind_caption
    case kind
    when 1
      '������e'
    when 2
      '����'
    when 3
      '��'
    end
  end

  def from_code
    from ? from.code : nil
  end

  def from_code=(from_code)
    from = Arc.find_by_code(from_code)
    self.from_id = from ? from.id : nil
  end

  def to_code
    to ? to.code : nil
  end

  def to_code=(to_code)
    to = Arc.find_by_code(to_code)
    self.to_id = to ? to.id : nil
  end

  def should_destroy?
    from_id.nil? || to_id.nil?
  end
end
