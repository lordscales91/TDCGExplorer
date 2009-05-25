/* tmofile.h */
/* vim: set shiftwidth=4 cindent : */

#include "frame.h"

typedef struct {
    int nnodes;
    char **nodes;
    int nframes;
    Frame **frames;
} Tmofile;

Tmofile *create_tmo();
void tmo_read(Tmofile *tmo, FILE *file);
int tmo_find_node_idx(Tmofile *tmo, char *name);
void tmo_dump(Tmofile *tmo);
void free_tmo(Tmofile *tmo);
