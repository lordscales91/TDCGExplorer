require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe Arc, "tags" do
  fixtures :arcs, :arc_tags, :tags

  it "one �� tags �� tags.one ���܂�" do
    arcs(:one).tags.should include( tags(:one) )
  end

  it "one �� tags �� tags.two ���܂�" do
    arcs(:one).tags.should include( tags(:two) )
  end

  it "one �� valid �ł���" do
    arcs(:one).should be_valid
  end

  it "�d�� arc_tag ������Ȃ� one �� valid �łȂ�" do
    arc_tag = arcs(:one).arc_tags.detect { |at| at.tag_id == 1 }
    arc_tag.tag_id = 2
    arcs(:one).should_not be_valid
  end
end

describe Arc, "search" do
  fixtures :arcs

  it "text TA0013 �Ō�������� two �Ƀ}�b�`����" do
    @search = Arc::Search.new('text' => 'TA0013')
    Arc.find(:all, @search.find_options).should == [ arcs(:two) ]
  end

  it "text tattoo �Ō�������� two �Ƀ}�b�`����" do
    @search = Arc::Search.new('text' => 'tattoo')
    Arc.find(:all, @search.find_options).should == [ arcs(:two) ]
  end
end
