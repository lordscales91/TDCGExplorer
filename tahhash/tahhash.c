#include <ruby.h>

static VALUE
calc(self, str)
    VALUE self;
    VALUE str;
{
    unsigned int key = 0xC8A4E57A;
    char *p, *pend;
    long idx;

    str = rb_str_dup(str);
    p = RSTRING(str)->ptr;
    pend = p + RSTRING(str)->len;
    while (p < pend) {
        *p = tolower(*p);
        p++;
    }
    p = RSTRING(str)->ptr;
    pend = p + RSTRING(str)->len;
    while (p < pend) {
        key = key << 19 | key >> 13;
        key = key ^ (unsigned int)*p;
        p++;
    }
    idx = RSTRING(str)->len - 1;
    key = key ^ ((RSTRING(str)->ptr[idx] & 0x1A) != 0x00 ? -1 : 0);
    return UINT2NUM(key);
}

void Init_tahhash()
{
    VALUE rb_cTAHHash;
    rb_cTAHHash = rb_define_class("TAHHash", rb_cObject);
    rb_define_singleton_method(rb_cTAHHash, "calc", calc, 1);
}
