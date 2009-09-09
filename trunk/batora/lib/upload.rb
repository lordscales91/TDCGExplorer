module Upload
module_function
  def upload_savefile(file)
    time = Time.now
    time_str = "%10d.%06d" % [ time.to_i, time.usec ]

    # bmp�Ƃ��ĕۑ�
    bmp_path = RAILS_ROOT + "/tmp/bitmaps/#{time_str}.bmp"
    ilist = Magick::ImageList.new
    ilist.from_blob(file.read)
    ilist.write(bmp_path)

    # bmp�����
    bitmap = Bitmap.new
    bitmap.load(bmp_path)
    # bitmap.tbn_names.each { |tbn| puts tbn }

    bmp = Bmp.new
    Bmp.transaction do
      bmp.path = bmp_path
      bmp.tbn_names = bitmap.tbn_names
      bmp.save!
    end

    # png�Ƃ��ĕۑ�
    png_path = RAILS_ROOT + "/public/images/bmps/#{bmp.id}.png"
    ilist.write(png_path)

    bmp
  end
end
