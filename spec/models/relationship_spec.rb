require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Relationship, "assign from_code" do
  fixtures :arcs, :relationships

  it "from_code を 'TA0583' にすると from_id は 1 になる" do
    rel = relationships(:one)
    rel.from_code = 'TA0583'
    rel.from_id.should == 1
  end

  it "from_code を 'TA0583 ' にすると from_id は 1 になる" do
    rel = relationships(:one)
    rel.from_code = 'TA0583 '
    rel.from_id.should == 1
  end

  it "from_code を 'TA0583　' にすると from_id は 1 になる" do
    rel = relationships(:one)
    rel.from_code = 'TA0583　'
    rel.from_id.should == 1
  end

  it "from_code を ' TA0583' にすると from_id は 1 になる" do
    rel = relationships(:one)
    rel.from_code = ' TA0583'
    rel.from_id.should == 1
  end

  it "from_code を '　TA0583' にすると from_id は 1 になる" do
    rel = relationships(:one)
    rel.from_code = '　TA0583'
    rel.from_id.should == 1
  end
end

describe Relationship, "assign to_code" do
  fixtures :arcs, :relationships

  it "to_code を 'TA0013' にすると to_id は 2 になる" do
    rel = relationships(:one)
    rel.to_code = 'TA0013'
    rel.to_id.should == 2
  end

  it "to_code を 'TA0013 ' にすると to_id は 2 になる" do
    rel = relationships(:one)
    rel.to_code = 'TA0013 '
    rel.to_id.should == 2
  end

  it "to_code を 'TA0013　' にすると to_id は 2 になる" do
    rel = relationships(:one)
    rel.to_code = 'TA0013　'
    rel.to_id.should == 2
  end

  it "to_code を ' TA0013' にすると to_id は 2 になる" do
    rel = relationships(:one)
    rel.to_code = ' TA0013'
    rel.to_id.should == 2
  end

  it "to_code を '　TA0013' にすると to_id は 2 になる" do
    rel = relationships(:one)
    rel.to_code = '　TA0013'
    rel.to_id.should == 2
  end
end

describe Relationship do
  fixtures :arcs, :relationships

  it "one は関係性を持つ" do
    arcs(:one).relationships.should == [ relationships(:one) ]
  end

  it "one は two と関係を持つ" do
    arcs(:one).relations.should == [ arcs(:two) ]
  end

  it "two は逆関係性を持つ" do
    arcs(:two).rev_relationships.should == [ relationships(:one) ]
  end

  it "two は one と逆関係を持つ" do
    arcs(:two).rev_relations.should == [ arcs(:one) ]
  end

  it "from と to が同じなら valid でない" do
    Relationship.new(:from_id => 1, :to_id => 1).should_not be_valid
  end

  it "同じ from, to の組み合わせを持つ rel があるなら valid でない" do
    Relationship.new(:from_id => 1, :to_id => 2).should_not be_valid
  end

  it "逆の from, to の組み合わせを持つ rel があるなら valid でない" do
    Relationship.new(:from_id => 2, :to_id => 1).should_not be_valid
  end

  it "one は valid である" do
    relationships(:one).should be_valid
  end
end
