require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Relationship, "assign from_code" do
  fixtures :arcs, :relationships

  it "from_code ‚ð 'TA0583' ‚É‚·‚é‚Æ from_id ‚Í 1 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.from_code = 'TA0583'
    rel.from_id.should == 1
  end

  it "from_code ‚ð 'TA0583 ' ‚É‚·‚é‚Æ from_id ‚Í 1 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.from_code = 'TA0583 '
    rel.from_id.should == 1
  end

  it "from_code ‚ð 'TA0583@' ‚É‚·‚é‚Æ from_id ‚Í 1 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.from_code = 'TA0583@'
    rel.from_id.should == 1
  end

  it "from_code ‚ð ' TA0583' ‚É‚·‚é‚Æ from_id ‚Í 1 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.from_code = ' TA0583'
    rel.from_id.should == 1
  end

  it "from_code ‚ð '@TA0583' ‚É‚·‚é‚Æ from_id ‚Í 1 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.from_code = '@TA0583'
    rel.from_id.should == 1
  end
end

describe Relationship, "assign to_code" do
  fixtures :arcs, :relationships

  it "to_code ‚ð 'TA0013' ‚É‚·‚é‚Æ to_id ‚Í 2 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.to_code = 'TA0013'
    rel.to_id.should == 2
  end

  it "to_code ‚ð 'TA0013 ' ‚É‚·‚é‚Æ to_id ‚Í 2 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.to_code = 'TA0013 '
    rel.to_id.should == 2
  end

  it "to_code ‚ð 'TA0013@' ‚É‚·‚é‚Æ to_id ‚Í 2 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.to_code = 'TA0013@'
    rel.to_id.should == 2
  end

  it "to_code ‚ð ' TA0013' ‚É‚·‚é‚Æ to_id ‚Í 2 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.to_code = ' TA0013'
    rel.to_id.should == 2
  end

  it "to_code ‚ð '@TA0013' ‚É‚·‚é‚Æ to_id ‚Í 2 ‚É‚È‚é" do
    rel = relationships(:one)
    rel.to_code = '@TA0013'
    rel.to_id.should == 2
  end
end

describe Relationship do
  fixtures :arcs, :relationships

  it "one ‚ÍŠÖŒW«‚ðŽ‚Â" do
    arcs(:one).relationships.should == [ relationships(:one) ]
  end

  it "one ‚Í two ‚ÆŠÖŒW‚ðŽ‚Â" do
    arcs(:one).relations.should == [ arcs(:two) ]
  end

  it "two ‚Í‹tŠÖŒW«‚ðŽ‚Â" do
    arcs(:two).rev_relationships.should == [ relationships(:one) ]
  end

  it "two ‚Í one ‚Æ‹tŠÖŒW‚ðŽ‚Â" do
    arcs(:two).rev_relations.should == [ arcs(:one) ]
  end

  it "from ‚Æ to ‚ª“¯‚¶‚È‚ç valid ‚Å‚È‚¢" do
    Relationship.new(:from_id => 1, :to_id => 1).should_not be_valid
  end

  it "“¯‚¶ from, to ‚Ì‘g‚Ý‡‚í‚¹‚ðŽ‚Â rel ‚ª‚ ‚é‚È‚ç valid ‚Å‚È‚¢" do
    Relationship.new(:from_id => 1, :to_id => 2).should_not be_valid
  end

  it "‹t‚Ì from, to ‚Ì‘g‚Ý‡‚í‚¹‚ðŽ‚Â rel ‚ª‚ ‚é‚È‚ç valid ‚Å‚È‚¢" do
    Relationship.new(:from_id => 2, :to_id => 1).should_not be_valid
  end

  it "one ‚Í valid ‚Å‚ ‚é" do
    relationships(:one).should be_valid
  end
end
