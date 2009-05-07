require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

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
