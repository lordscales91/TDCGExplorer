require 'RMagick'

module Upload
module_function
  def upload_savefile_0(file)
    time = Time.now
    time_str = "%10d.%06d" % [ time.to_i, time.usec ]

    # bmp‚Æ‚µ‚Ä•Û‘¶
    bmp_path = RAILS_ROOT + "/tmp/bitmaps/#{time_str}.bmp"
    ilist = Magick::ImageList.new
    ilist.from_blob(file.read)
    ilist.write(bmp_path)

    # bmp‚ğ‰ğÍ
    bitmap = Bitmap.new
    bitmap.load(bmp_path)
    # bitmap.tbn_names.each { |tbn| puts tbn }

    bmp = Bmp.new
    Bmp.transaction do
      bmp.path = bmp_path
      bmp.tbn_names = bitmap.tbn_names
      bmp.save!
    end

    # png‚Æ‚µ‚Ä•Û‘¶
    png_path = RAILS_ROOT + "/public/images/bmps/#{bmp.id}.png"
    ilist.write(png_path)

    image = ilist.first
    image_1 = image.crop(0, 0, 128, 128)
    image_1_path = RAILS_ROOT + "/public/images/bmps/128x128/#{bmp.id}.png"
    image_1.write(image_1_path)
    image_2 = image_1.resize(64, 64)
    image_2_path = RAILS_ROOT + "/public/images/bmps/64x64/#{bmp.id}.png"
    image_2.write(image_2_path)

    bmp
  end

  def upload_savefile(file)
    bmp = upload_savefile_0(file)
    GC.start
    bmp
  end
end
