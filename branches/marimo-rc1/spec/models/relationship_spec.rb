require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Relationship, "assign from_code" do
  fixtures :arcs, :relationships

  it "from_code �� 'TA0583' �ɂ���� from_id �� 1 �ɂȂ�" do
    rel = relationships(:one)
    rel.from_code = 'TA0583'
    rel.from_id.should == 1
  end

  it "from_code �� 'TA0583 ' �ɂ���� from_id �� 1 �ɂȂ�" do
    rel = relationships(:one)
    rel.from_code = 'TA0583 '
    rel.from_id.should == 1
  end

  it "from_code �� 'TA0583�@' �ɂ���� from_id �� 1 �ɂȂ�" do
    rel = relationships(:one)
    rel.from_code = 'TA0583�@'
    rel.from_id.should == 1
  end

  it "from_code �� ' TA0583' �ɂ���� from_id �� 1 �ɂȂ�" do
    rel = relationships(:one)
    rel.from_code = ' TA0583'
    rel.from_id.should == 1
  end

  it "from_code �� '�@TA0583' �ɂ���� from_id �� 1 �ɂȂ�" do
    rel = relationships(:one)
    rel.from_code = '�@TA0583'
    rel.from_id.should == 1
  end
end

describe Relationship, "assign to_code" do
  fixtures :arcs, :relationships

  it "to_code �� 'TA0013' �ɂ���� to_id �� 2 �ɂȂ�" do
    rel = relationships(:one)
    rel.to_code = 'TA0013'
    rel.to_id.should == 2
  end

  it "to_code �� 'TA0013 ' �ɂ���� to_id �� 2 �ɂȂ�" do
    rel = relationships(:one)
    rel.to_code = 'TA0013 '
    rel.to_id.should == 2
  end

  it "to_code �� 'TA0013�@' �ɂ���� to_id �� 2 �ɂȂ�" do
    rel = relationships(:one)
    rel.to_code = 'TA0013�@'
    rel.to_id.should == 2
  end

  it "to_code �� ' TA0013' �ɂ���� to_id �� 2 �ɂȂ�" do
    rel = relationships(:one)
    rel.to_code = ' TA0013'
    rel.to_id.should == 2
  end

  it "to_code �� '�@TA0013' �ɂ���� to_id �� 2 �ɂȂ�" do
    rel = relationships(:one)
    rel.to_code = '�@TA0013'
    rel.to_id.should == 2
  end
end

describe Relationship do
  fixtures :arcs, :relationships

  it "one �͊֌W��������" do
    arcs(:one).relationships.should == [ relationships(:one) ]
  end

  it "one �� two �Ɗ֌W������" do
    arcs(:one).relations.should == [ arcs(:two) ]
  end

  it "two �͋t�֌W��������" do
    arcs(:two).rev_relationships.should == [ relationships(:one) ]
  end

  it "two �� one �Ƌt�֌W������" do
    arcs(:two).rev_relations.should == [ arcs(:one) ]
  end

  it "from �� to �������Ȃ� valid �łȂ�" do
    Relationship.new(:from_id => 1, :to_id => 1).should_not be_valid
  end

  it "���� from, to �̑g�ݍ��킹������ rel ������Ȃ� valid �łȂ�" do
    Relationship.new(:from_id => 1, :to_id => 2).should_not be_valid
  end

  it "�t�� from, to �̑g�ݍ��킹������ rel ������Ȃ� valid �łȂ�" do
    Relationship.new(:from_id => 2, :to_id => 1).should_not be_valid
  end

  it "one �� valid �ł���" do
    relationships(:one).should be_valid
  end
end
