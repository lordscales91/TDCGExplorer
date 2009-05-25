/* tdcg.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>

#include "tdcg.h"

int read_int(FILE* file)
{
    int ret;
    fread(&ret, sizeof(int), 1, file);
    return ret;
}

float read_float(FILE* file)
{
    float ret;
    fread(&ret, sizeof(float), 1, file);
    return ret;
}

void read_matrix(Matrix* ret, FILE* file)
{
    Matrix m;
    fread(m.m, sizeof(Matrix), 1, file);

    int i;
    for (i=0; i<4; i++)
    {
	ret->m[i*4+0] = m.m[i+0*4];
	ret->m[i*4+1] = m.m[i+1*4];
	ret->m[i*4+2] = m.m[i+2*4];
	ret->m[i*4+3] = m.m[i+3*4];
    }
}

typedef struct { float w; int i; } Skin;

int skin_cmp(const void *a, const void *b)
{
    Skin *sa = (Skin *)a;
    Skin *sb = (Skin *)b;
    return sa->w < sb->w;
}

void read_vertex(Vertex* ret, FILE* file)
{
    int nweights;
    Skin skin[5];
    int i;
    for (i=0; i<5; i++)
    {
	skin[i].i = 0;
	skin[i].w = 0.0;
    }
    fread(ret->position, sizeof(float), 3, file);
    fread(ret->normal, sizeof(float), 3, file);
    ret->u = read_float(file);
    ret->v = read_float(file);
    nweights = read_int(file);
    assert(nweights >= 0);
    assert(nweights <= 5);
    for (i=0; i<nweights; i++)
    {
	skin[i].i = read_int(file);
	skin[i].w = read_float(file);
    }
    qsort(skin, 5, sizeof(Skin), skin_cmp);
    for (i=0; i<4; i++)
    {
	//printf("skin %d i %d w %f\n", i, skin[i].i, skin[i].w);
	ret->indices[i] = skin[i].i;
	ret->weights[i] = skin[i].w;
    }
}

void read_string(char* ret, FILE* file)
{
    int c;
    while ((c = fgetc(file)) != EOF)
    {
	*ret++ = (char)c;
	if (c == 0)
	    break;
    }
}
