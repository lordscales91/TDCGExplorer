/* param.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "tdcg.h"
#include "param.h"

Param *create_param()
{
    Param *param = (Param *)malloc(sizeof(Param));
    return param;
}

void free_param(Param *param)
{
    free(param->value);
    free(param->name);
    free(param->type);
    free(param);
}

void param_read(Param *param, char *line)
{
    char *p = line;
    char *q = p;
    int len = strlen(p);
    int pos = 0;

    /* type */
    for (; pos<len; pos++, p++)
    {
	if (*p == ' ')
	    break;
    }
    param->type = (char *)malloc(sizeof(char)*pos+1);
    memcpy(param->type, q, pos);
    param->type[pos] = '\0';

    /* pass space */
    for (;;)
    {
	if (*p == ' ')
	    p++;
	else
	    break;
    }

    q = p;
    len = strlen(p);
    pos = 0;

    /* name */
    for (; pos<len; pos++, p++)
    {
	if (*p == ' ')
	    break;
	else if (*p == '=')
	    break;
    }
    param->name = (char *)malloc(sizeof(char)*pos+1);
    memcpy(param->name, q, pos);
    param->name[pos] = '\0';

    /* pass space and equal */
    for (;;)
    {
	if (*p == ' ')
	    p++;
	else if (*p == '=')
	    p++;
	else
	    break;
    }

    /* value */
    len = strlen(p);
    pos = 0;
    param->value = (char *)malloc(sizeof(char)*len);
    q = param->value;
    int quote = 0;
    int paren = 0;
    for (; pos<len; pos++, p++)
    {
	if (*p == ' ')
	{
	    if (quote)
		*q++ = *p;
	    else if (paren)
		;
	    else
	        break;
	}
	else if (*p == '"')
	    quote = !quote;
	else if (*p == '[')
	    paren = 1;
	else if (*p == ']')
	    paren = 0;
	else
	    *q++ = *p;
    }
    *q = '\0';
}

void param_dump(Param *param)
{
    printf("type %s name %s value %s\n", param->type, param->name, param->value);
}
