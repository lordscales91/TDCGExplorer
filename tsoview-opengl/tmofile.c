/* tmofile.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "tdcg.h"
#include "tmofile.h"

Tmofile *create_tmo()
{
    Tmofile *tmo = (Tmofile *)malloc(sizeof(Tmofile));
    return tmo;
}

void tmo_read(Tmofile *tmo, FILE *file)
{
    char header[8];
    fread(header, sizeof(char), 8, file);
    int opt0;
    opt0 = read_int(file);
    int opt1;
    opt1 = read_int(file);

    tmo->nnodes = read_int(file);
    tmo->nodes = (char **)malloc(sizeof(char *)*tmo->nnodes);
    int i;
    for (i=0; i<tmo->nnodes; i++)
    {
	char *string = tmo->nodes[i] = (char *)malloc(sizeof(char)*256);
	read_string(string, file);
    }

    tmo->nframes = read_int(file);
    tmo->frames = (Frame **)malloc(sizeof(Frame *)*tmo->nframes);
    for (i=0; i<tmo->nframes; i++)
    {
	Frame *frame = tmo->frames[i] = create_frame();
	frame_read(frame, file);
    }

    char footer[4];
    fread(footer, sizeof(char), 4, file);
}

int tmo_find_node_idx(Tmofile *tmo, char *name)
{
    int i;
    for (i=0; i<tmo->nnodes; i++)
    {
	char *node = tmo->nodes[i];
	if (!strcmp(node, name))
	    return i;
    }
    return -1;
}

void tmo_dump(Tmofile *tmo)
{
    printf("nnodes %d\n", tmo->nnodes);
    printf("%s\n", tmo->nodes[tmo->nnodes-1]);
    printf("nframes %d\n", tmo->nframes);
    int i;
    for (i=0; i<tmo->nframes; i++)
    {
	Frame *frame = tmo->frames[i];
	frame_dump(frame);
    }
}

void free_tmo(Tmofile *tmo)
{
    int i;
    for (i=0; i<tmo->nframes; i++)
    {
	Frame *frame = tmo->frames[i];
	free_frame(frame);
    }
    free(tmo->frames);
    for (i=0; i<tmo->nnodes; i++)
    {
	char *string = tmo->nodes[i];
	free(string);
    }
    free(tmo->nodes);
    free(tmo);
}
