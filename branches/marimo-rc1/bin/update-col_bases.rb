#!/usr/bin/ruby -KS
require File.dirname(__FILE__) + "/../config/environment"

Tso.find(:all).each do |tso|
  tso.update_col_bases
end
