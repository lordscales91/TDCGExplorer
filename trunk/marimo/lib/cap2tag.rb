class Cap2tag

  CAP2TAG_NAMES = {}

  open(File.dirname(__FILE__) + "/../arc/summary-tag-uniq-mapping.txt") do |f|
    while line = f.gets
      row = line.chomp.split(/\t+/)
      caption = row.shift
      CAP2TAG_NAMES[caption] = row
    end
  end

  def self.caption_to_tag_names(caption)
    ret = []
    cap_names = caption.split(',')
    cap_names.each do |cap|
      ret.concat CAP2TAG_NAMES[cap] || []
    end
    ret
  end

  def self.summary_to_tag_names(summary)
    ret = []
    summary.scan(/【(.+?)】/) do |caption, |
      ret.concat caption_to_tag_names(caption)
    end
    ret
  end
end
