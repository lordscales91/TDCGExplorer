module Upload
module_function
  def upload_savefile(file)
    # bmp�Ƃ��ĕۑ�
    bmp_path = RAILS_ROOT + "/tmp/bitmaps/1.bmp"
    ilist = Magick::ImageList.new
    ilist.from_blob(file.read)
    ilist.write(bmp_path)

    # bmp�����
    bitmap = Bitmap.new
    bitmap.load(bmp_path)
    # bitmap.tbn_names.each { |tbn| puts tbn }

    Bmp.transaction do
      bmp = Bmp.new
      bmp.path = bmp_path
      bmp.tbn_names = bitmap.tbn_names
      bmp.save!
    end
  end
end
