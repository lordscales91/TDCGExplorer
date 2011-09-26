$LOAD_PATH.unshift File.expand_path(File.dirname(__FILE__) + "/../../lib")
require 'cap2tag'

describe Cap2tag do
  it do
    Cap2tag.summary_to_tag_names("【修正版】foo").should == %w[ ]
  end
  it do
    Cap2tag.summary_to_tag_names("【上服】foo").should == %w[ 上服 ]
  end
  it do
    Cap2tag.summary_to_tag_names("【下服】foo").should == %w[ 下服 ]
  end
  it do
    Cap2tag.summary_to_tag_names("【上服,下服】foo").should == %w[ 上服 下服 ]
  end
  it do
    Cap2tag.summary_to_tag_names("【全服】foo").should == %w[ 全身衣服 ]
  end
  it do
    Cap2tag.summary_to_tag_names("【手足】foo").should == %w[ 手袋 靴下 ]
  end
end
