require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

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
end
