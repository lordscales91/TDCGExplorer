#
# Added for Ruby-GetText-Package
#

desc "Create mo-files for L10n"
task :makemo do
  require 'gettext/utils'
  GetText.create_mofiles(true, "po", "locale")
end

desc "Update pot/po files to match new version."
task :updatepo do
  require 'gettext/utils'
  GetText.update_pofiles("marimo", Dir.glob("{app,lib}/**/*.{rb,erb}"),
			 "marimo 0.0.1")
end
