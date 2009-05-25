/* tmodump.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>

#include "matrix.h"
#include "tdcg.h"
#include "frame.h"

Frame *create_frame()
{
    Frame *frame = (Frame *)malloc(sizeof(Frame));
    frame->nmatrices = 0;
    frame->matrices = NULL;
    return frame;
}

void frame_read(Frame *frame, FILE *file)
{
    frame->nmatrices = read_int(file);
    frame->matrices = (Matrix *)malloc(sizeof(Matrix)*frame->nmatrices);
    Matrix *m = frame->matrices;
    int j;
    for (j=0; j<frame->nmatrices; j++, m++)
    {
	read_matrix(m, file);
    }
}

void frame_dump(Frame *frame)
{
    printf("nmatrices %d\n", frame->nmatrices);
}

void free_frame(Frame *frame)
{
    if (frame->matrices)
	free(frame->matrices);
    free(frame);
}

