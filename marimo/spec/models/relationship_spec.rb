require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Relationship do
  fixtures :arcs, :relationships

  it "one ‚ÍŠÖŒW«‚ğ‚Â" do
    arcs(:one).relationships.should == [ relationships(:one) ]
  end

  it "one ‚Í two ‚ÆŠÖŒW‚ğ‚Â" do
    arcs(:one).relations.should == [ arcs(:two) ]
  end

  it "two ‚Í‹tŠÖŒW«‚ğ‚Â" do
    arcs(:two).rev_relationships.should == [ relationships(:one) ]
  end

  it "two ‚Í one ‚Æ‹tŠÖŒW‚ğ‚Â" do
    arcs(:two).rev_relations.should == [ arcs(:one) ]
  end
end
