# Methods added to this helper will be available to all templates in the application.
module ApplicationHelper

  def content_for(name, content = nil, &block)
    @has_content ||= {}
    @has_content[name] = true
    super(name, content, &block)
  end

  def has_content?(name)
    (@has_content && @has_content[name]) || false
  end

  def current_headmenu?(options)
    url = url_for(options)
    re = Regexp.new(Regexp.escape(url) + '($|[/?])')
    re.match(@controller.request.request_uri)
  end

  def link_menu_to(name, options = {}, html_options = {}, *parameters_for_method_reference, &block)
    cp_p = current_headmenu?(options)
    if cp_p
      html_options.update(:class => 'selected')
    end
    link_to name, options, html_options, *parameters_for_method_reference, &block
  end
end
